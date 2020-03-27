using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;


/// <summary>
/// 
/// </summary>
public class State {
	/// <summary>
	/// 
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// 
	/// </summary>
	protected readonly List<Func<bool>> pool = new List<Func<bool>>();


	/// <summary>
	/// Construct a state
	/// </summary>
	/// <param name="name">The name of the state</param>
	/// <param name="state">Bool function</param>
	public State(string name, Func<bool> state) {
		Name = name;
		pool.Add(state);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public bool Resolve() => pool.All((x => x()));


	/// <summary>
	/// 
	/// </summary>
	/// <param name="f"></param>
	public void Add(Func<bool> f) => pool.Add(f);


	/// <summary>
	/// 
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <returns></returns>
	public static State operator +(State left, State right) {
		return new State(left.Name + right.Name, () => left.Resolve() && right.Resolve());
	}


	/// <summary>
	/// Implicitly convert a tupel to a state
	/// </summary>
	/// <param name="tupel"></param>
	/// <returns></returns>
	public static implicit operator State((string, Func<bool>) tupel) {
		return new State(tupel.Item1, tupel.Item2);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="f"></param>
	/// <returns></returns>
	public static implicit operator State(Func<bool> f) {
		return new State(f.GetInvocationList()[0].ToString(), f);
	}


	/// <summary>
	/// Implicitly convert state to bool
	/// </summary>
	/// <param name="state"></param>
	/// <returns></returns>
	public static implicit operator bool(State state) => state.Resolve();
}


/// <summary>
/// Custom Attribute to bind a local bool property to a native state
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class NativeStateAttribute : Attribute {
	/// <summary>
	/// Name (and path) of the target note)
	/// </summary>
	public string Name { get; set; } = "";


	/// <summary>
	/// Standard constructor
	/// </summary>
	public NativeStateAttribute() {
	}


	/// <summary>
	/// Construct NoteBindingAttribute
	/// </summary>
	/// <param name="bindTo">Name (and path) of the target note.</param>
	public NativeStateAttribute(string name) {
		Name = name;
	}
}


/// <summary>
/// Manage NativeStates 
/// </summary>
public class GlobalState {
	/// <summary>
	/// Global NativeState dictionary
	/// </summary>
	public static Dictionary<string, State> NativeStates { get; }
		= new Dictionary<string, State>();


	/// <summary>
	/// Add a native state
	/// </summary>
	/// <param name="name"></param>
	/// <param name="f"></param>
	public static void AddNativeState(string name, Func<bool> f) {
		if (NativeStates.ContainsKey(name)) NativeStates.Remove(name);
		NativeStates.Add(name, new State(name, f));
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="state"></param>
	public static void AddNativeState(State state)
		=> NativeStates.Add(state.Name, state);


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public static State State(string name) => NativeStates[name];
}


/// <summary>
/// Static helper class for state extensions
/// </summary>
public static class NativeStateExtension {
	/// <summary>
	/// Look for native states at the current instance and add
	/// it via a function wrapper to a global dictionary. 
	/// </summary>
	/// <param name="clazz">The Object instance</param>
	/// <returns></returns>
	public static void SetupNativeStates(this System.Object clazz) {
		/* get properties of class */
		foreach (var propertyInfo in clazz.GetType().GetProperties()) {
			/* get properties information */
			foreach (var ca in propertyInfo.GetCustomAttributes()) {
				/* if a native state custom attribute found */
				if (ca is NativeStateAttribute nativeState) {
					var name = nativeState.Name.Trim().Length > 0 ? nativeState.Name : propertyInfo.Name;

					if (propertyInfo.GetValue(clazz) is bool b) {
						if (GlobalState.NativeStates.ContainsKey(name))
							GlobalState.NativeStates.Remove(name);
						GlobalState.AddNativeState(name, () => (bool) propertyInfo.GetValue(clazz));
					}
					else {
						throw new NativeStateTypeException(
							$"Type constraint violation at: '{name}' <- boolean expected, but {propertyInfo.PropertyType.Name} found.");
					}
				}
			}
		}
	}
}


/// <summary>
/// Type constraint Exception
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
