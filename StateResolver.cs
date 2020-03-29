using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;


/// <summary>
/// 
/// </summary>
public class NativeState : State {
	protected Func<bool> Logic { get; set; } = () => true;


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="logic"></param>
	public NativeState(string name, Func<bool> logic) {
		Name = name;
		Logic = logic;
	}


	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public override bool Resolve() => Logic();
}

/// <summary>
/// 
/// </summary>
public class PolyState : State {
	/// <summary>
	/// 
	/// </summary>
	protected readonly List<Func<bool>> pool = new List<Func<bool>>();


	/// <summary>
	/// Construct a state
	/// </summary>
	/// <param name="name">The name of the state</param>
	/// <param name="state">Bool function</param>
	public PolyState(string name, Func<bool> state) {
		Name = name;
		pool.Add(state);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public override bool Resolve() => pool.All((x => x()));
}

/// <summary>
/// 
/// </summary>
public abstract partial class State {
	/// <summary>
	/// 
	/// </summary>
	public string Name { get; set; }


	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public abstract bool Resolve();


	/// <summary>
	/// 
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <returns></returns>
	public static PolyState operator +(State left, State right) {
		return new PolyState(left.Name + right.Name, () => left.Resolve() && right.Resolve());
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <returns></returns>
	public static PolyState operator +(State left, Func<bool> right) {
		return new PolyState(left.Name, () => left.Resolve() && right());
	}


	/// <summary>
	/// Implicitly convert state to bool
	/// </summary>
	/// <param name="state"></param>
	/// <returns></returns>
	public static implicit operator bool(State state) => state.Resolve();


	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public override string ToString() {
		return $"{GetType().Name}({Resolve()})";
	}
}


/// <summary>
/// Custom Attribute to bind a local bool property to a native state
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
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
public partial class State {
	/// <summary>
	/// Global NativeState dictionary
	/// </summary>
	public static Dictionary<string, NativeState> NativeStates { get; }
		= new Dictionary<string, NativeState>();


	/// <summary>
	/// Add a native state
	/// </summary>
	/// <param name="name"></param>
	/// <param name="f"></param>
	public static void AddNativeState(string name, Func<bool> f) {
		if (NativeStates.ContainsKey(name)) NativeStates.Remove(name);
		NativeStates.Add(name, new NativeState(name, f));
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="state"></param>
	public static void AddNativeState(NativeState state)
		=> NativeStates.Add(state.Name, state);


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public static NativeState NativeState(string name) => NativeStates[name];
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
						if (State.NativeStates.ContainsKey(name))
							State.NativeStates.Remove(name);
						State.AddNativeState(name, () => (bool) propertyInfo.GetValue(clazz));
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