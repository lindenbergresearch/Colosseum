using System;
using System.IO;
using Godot;
using System.Text.Json;
using System.Text.Json.Serialization;
using File = Godot.File;
using Object = System.Object;
using static Godot.Input;

/// <summary>
/// Base class for serializing data classes
/// </summary>
public abstract class SerializableDataClass {
	public abstract int VERSION_MAJOR { get; }
	public abstract int VERSION_MINOR { get; }
	public abstract int VERSION_PATCH { get; }

	/// <summary>
	/// Timestamp of serialization
	/// </summary>
	public DateTimeOffset Timestamp { get; set; }

	/// <summary>
	/// Type of serialized class
	/// </summary>
	public string Type { get; set; }


	/// <summary>
	/// Returns the version as string
	/// </summary>
	/// <returns></returns>
	public string GetVersionStr() => $"{VERSION_MAJOR}.{VERSION_MINOR}.{VERSION_PATCH}";


	/// <summary>
	/// Standard Constructor
	/// </summary>
	protected SerializableDataClass() {
		Timestamp = DateTime.Now;
		Type = GetType().FullName;
	}
}


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


	/// <summary>
	/// Serializes an object to a json file.
	/// </summary>
	/// <param name="clazz">The class instance to serialize</param>
	/// <param name="filename">The target filename (including .json)</param>
	public static void SerializeObject<T>(T clazz, string filename) where T : SerializableDataClass {
		var options = new JsonSerializerOptions {
			WriteIndented = true,
			MaxDepth = 200
		};

		var jsonString = JsonSerializer.Serialize<T>(clazz, options);
		System.IO.File.WriteAllText(filename, jsonString);
	}


	/// <summary>
	/// Deserialize an object based on a json file.
	/// </summary>
	/// <param name="filename">The filename of the json file</param>
	/// <typeparam name="T">The target type to be deserialized</typeparam>
	/// <returns>And instance of the deserialized file</returns>
	public static T DeserializeObject<T>(string filename) where T : SerializableDataClass {
		var jsonString = System.IO.File.ReadAllText(filename);
		return JsonSerializer.Deserialize<T>(jsonString);
	}


	/// <summary>
	/// Dump all fields from an object and it's references.
	/// For debugging issues.
	/// </summary>
	/// <param name="clazz">The object reference to dump</param>
	/// <param name="prefix">Prefix to the output</param>
	/// <param name="sopen">The formatting open character</param>
	/// <param name="sclose">THe formatting close character</param>
	/// <returns></returns>
	public static string Dump(System.Object clazz, string prefix = "", string sopen = "[", string sclose = "]") {
		var info = $"{prefix}{sopen}type=";
		var t = clazz.GetType();

		info += t.Name;

		foreach (var fieldInfo in t.GetFields()) {
			var name = fieldInfo.Name;
			var type = fieldInfo.FieldType.Name;
			var value = fieldInfo.GetValue(null);

			info += $"{name}={type}({value}) ";
		}

		foreach (var propertyInfo in t.GetProperties()) {
			var name = propertyInfo.Name;
			var type = propertyInfo.PropertyType;
			dynamic value = null;
			try {
				value = propertyInfo.GetValue(clazz);
			}
			catch (Exception e) {
				value = "?";
			}

			if (!type.IsPrimitive && !type.IsValueType && type != typeof(String) && type != typeof(Decimal)) {
				info += name + "=" + Dump(value, "*" + prefix) + " ";
			}

			info += $"{name}={type.Name}({value}) ";
		}

		return info.TrimEnd() + sclose;
	}


	/// <summary>
	/// Functor as key-binding
	/// </summary>
	/// <param name="name">The godot name of the key</param>
	/// <returns>A function which returns true if that key is pressed</returns>
	public static Func<bool> GetActionPressedBinding(string name)
		=> () => IsActionPressed(name);
}
