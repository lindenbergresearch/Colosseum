using Godot;

/// <summary>
///     Utility method compilation.
/// </summary>
public static class Util {
	/// <summary>
	///     Returns the value of a class property by name.
	///     Uses reflection in case of dynamic cast problems.
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="clazz">The class instance where the property is bound to.</param>
	/// <returns></returns>
	public static dynamic GetPropertyByName(string name, object clazz) {
		return clazz.GetType().GetProperty(name).GetValue(clazz);
	}


	/// <summary>
	///     Invokes a method of a class by name and reference.
	///     Uses reflection in case of dynamic cast problems.
	/// </summary>
	/// <param name="name">The methods name.</param>
	/// <param name="args">List of args to be passed.</param>
	/// <param name="clazz">The class instance where the method is located.</param>
	/// <returns></returns>
	public static dynamic InvokeMethodByName(string name, object[] args, object clazz) {
		return clazz.GetType().GetMethod(name).Invoke(clazz, args);
	}


	/// <summary>
	///     Shortcut for Vector2 creation.
	/// </summary>
	/// <param name="x">The x value.</param>
	/// <param name="y">The y value.</param>
	/// <returns>A new instance of a Vector2</returns>
	public static Vector2 Vec(float x, float y) {
		return new Vector2(x, y);
	}


	/// <summary>
	///     Shortcut for Color RGB(A)
	/// </summary>
	/// <param name="r">Red</param>
	/// <param name="g">Green</param>
	/// <param name="b">Blue</param>
	/// <param name="a">Alpha (default set to 1.0)</param>
	/// <returns></returns>
	public static Color Color(float r, float g, float b, float a = 1.0f) {
		return new Color(r, g, b, a);
	}
}
