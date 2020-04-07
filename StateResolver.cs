using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

/// <summary>
/// </summary>
public class NativeState : State {
    /// <summary>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="logic"></param>
    public NativeState(string name, Func<bool> logic) {
        Name = name;
        Logic = logic;
    }


    protected Func<bool> Logic { get; set; }


    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override bool Resolve() {
        return Logic();
    }


    public static implicit operator NativeState(Func<bool> f) {
        return new NativeState(f.GetType().Name, f);
    }
}


/// <summary>
/// </summary>
public class PolyState : State {
    /// <summary>
    /// </summary>
    protected readonly List<Func<bool>> pool = new List<Func<bool>>();


    /// <summary>
    ///     Construct a state
    /// </summary>
    /// <param name="name">The name of the state</param>
    /// <param name="state">Bool function</param>
    public PolyState(string name, Func<bool> state) {
        Name = name;
        pool.Add(state);
    }


    /// <summary>
    /// </summary>
    /// <param name="logic"></param>
    /// <returns></returns>
    public static implicit operator PolyState(bool logic) {
        return new PolyState("", () => logic);
    }


    /// <summary>
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static implicit operator PolyState(Func<bool> f) {
        return new PolyState("", f);
    }


    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override bool Resolve() {
        return pool.All(x => x());
    }
}


/// <summary>
/// </summary>
public abstract partial class State {
    /// <summary>
    /// </summary>
    public string Name { get; set; }


    /// <summary>
    /// </summary>
    /// <returns></returns>
    public abstract bool Resolve();


    /// <summary>
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static PolyState operator +(State left, State right) {
        return new PolyState(left.Name + right.Name, () => left.Resolve() && right.Resolve());
    }


    /// <summary>
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static PolyState operator +(State left, Func<bool> right) {
        return new PolyState(left.Name, () => left.Resolve() && right());
    }


    /// <summary>
    ///     Implicitly convert state to bool
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public static implicit operator bool(State state) {
        return state.Resolve();
    }


    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override string ToString() {
        return $"{GetType().Name}({Resolve()})";
    }
}


/// <summary>
///     Custom Attribute to bind a local bool property to a native state
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class NativeStateAttribute : Attribute {
    /// <summary>
    ///     Standard constructor
    /// </summary>
    public NativeStateAttribute() {
    }


    /// <summary>
    /// </summary>
    /// <param name="bind"></param>
    public NativeStateAttribute(string bind) {
        Bind = bind;
        Name = $"{bind}";
    }


    /// <summary>
    ///     Name (and path) of the target note)
    /// </summary>
    public string Name { get; set; } = "";

    private string Bind { get; }
}


/// <summary>
///     Manage NativeStates
/// </summary>
public partial class State {
    /// <summary>
    ///     Global NativeState dictionary
    /// </summary>
    public static Dictionary<string, NativeState> NativeStates { get; }
        = new Dictionary<string, NativeState>();


    /// <summary>
    ///     Add a native state
    /// </summary>
    /// <param name="name"></param>
    /// <param name="f"></param>
    public static void AddNativeState(string name, Func<bool> f) {
        if (NativeStates.ContainsKey(name)) NativeStates.Remove(name);
        NativeStates.Add(name, new NativeState(name, f));
    }


    /// <summary>
    /// </summary>
    /// <param name="state"></param>
    public static void AddNativeState(NativeState state) {
        NativeStates.Add(state.Name, state);
    }


    /// <summary>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static NativeState NativeState(string name) {
        return NativeStates[name];
    }
}


/// <summary>
///     Static helper class for state extensions
/// </summary>
public static class NativeStateExtension {
    /// <summary>
    ///     Look for native states at the current instance and add
    ///     it via a function wrapper to a global dictionary.
    /// </summary>
    /// <param name="obj">The Object instance</param>
    /// <returns></returns>
    public static void SetupNativeStates(this object obj) {
        var t = obj.GetType();

        /* get properties of class */
        foreach (var propertyInfo in t.GetProperties()) //Console.WriteLine($"Property: {propertyInfo.Name} Type: {propertyInfo.GetType().Name}");
            /* get properties information */
        foreach (var customAttribute in propertyInfo.GetCustomAttributes()) /* if a native state custom attribute found */
            if (customAttribute is NativeStateAttribute nativeState) {
                var name = nativeState.Name.Trim().Length > 0 ? nativeState.Name : propertyInfo.Name;

                if (propertyInfo.GetValue(obj) is bool) {
                    if (State.NativeStates.ContainsKey(name)) State.NativeStates.Remove(name);
                    State.AddNativeState(name, () => (bool) propertyInfo.GetValue(obj));
                }
                else if (propertyInfo.GetValue(obj) is Func<bool>) {
                    if (State.NativeStates.ContainsKey(name)) State.NativeStates.Remove(name);
                    State.AddNativeState(name, (Func<bool>) propertyInfo.GetValue(obj));
                }
                else {
                    throw new NativeStateTypeException(
                        $"Type constraint violation at: '{name}' <- boolean expected, but {propertyInfo.PropertyType.Name} found.");
                }
            }

        //Console.WriteLine($"Examining fields of: {obj.GetType().Name}");

        /* tricky */
        foreach (var fieldInfo in t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
        ) //Console.WriteLine($"Field: {fieldInfo.Name} {fieldInfo.FieldType}");

        foreach (var customAttribute in fieldInfo.GetCustomAttributes())
            if (customAttribute is NativeStateAttribute nativeState && nativeState.Name.Trim().Length > 1) {
                var m = t.GetMethods().Where(info => info.Name == nativeState.Name).ToList();

                if (m.Count == 1) {
                    bool Resolver() {
                        return (bool) m[0].Invoke(obj, null);
                    }

                    fieldInfo.SetValue(obj, new NativeState(nativeState.Name, Resolver));
                }
            }

        /* get all methods */
        foreach (var methodInfo in obj.GetType().GetMethods()) /* get all custom attributes from the current method */
        foreach (var customAttribute in methodInfo.GetCustomAttributes()) /* check for native state */
            if (customAttribute is NativeStateAttribute nativeState) {
                var name = nativeState.Name.Trim().Length > 0 ? nativeState.Name : methodInfo.Name;

                if (methodInfo.ReturnType == typeof(bool)) {
                    if (State.NativeStates.ContainsKey(name)) State.NativeStates.Remove(name);
                    Func<bool> f = () => (bool) methodInfo.Invoke(obj, null);
                    State.AddNativeState(name, f);
                }
            }
    }
}


/// <summary>
///     Type constraint Exception
/// </summary>
public class NativeStateTypeException : Exception {
    public NativeStateTypeException() {
    }


    protected NativeStateTypeException(SerializationInfo? info, StreamingContext context) : base(info, context) {
    }


    public NativeStateTypeException(string? message) : base(message) {
    }


    public NativeStateTypeException(string? message, Exception? innerException) : base(message, innerException) {
    }
}