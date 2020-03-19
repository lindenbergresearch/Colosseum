/// <summary>
/// Utility method compilation.
/// </summary>
public static class Util {
	/// <summary>
	/// Returns the value of a class property by name.
	/// Uses reflection in case of dynamic cast problems.  
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="clazz">The class instance where the property is bound to.</param>
	/// <returns></returns>
	public static dynamic GetPropertyByName(string name, object clazz)
		=> clazz.GetType().GetProperty(name).GetValue(clazz);


	/// <summary>
	/// Invokes a method of a class by name and reference.
	/// Uses reflection in case of dynamic cast problems.
	/// </summary>
	/// <param name="name">The methods name.</param>
	/// <param name="args">List of args to be passed.</param>
	/// <param name="clazz">The class instance where the method is located.</param>
	/// <returns></returns>
	public static dynamic InvokeMethodByName(string name, object[] args, object clazz)
		=> clazz.GetType().GetMethod(name).Invoke(clazz, args);
}
