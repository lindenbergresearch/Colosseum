using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Renoir {

	/// <summary>
	///     Property Extensions
	/// </summary>
	public static class PropertyExtensions {
		/// <summary>
		/// </summary>
		/// <param name="obj"></param>
		public static void SetupGlobalProperties(this object obj) {
			var type = obj.GetType();

			Logger.trace($"Setup global properties for type: {type.Name}");

			foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
				if (propertyInfo.PropertyType.Name.StartsWith("Property"))
					foreach (var customAttribute in propertyInfo.GetCustomAttributes())
						if (customAttribute is RegisterAttribute sa) {
							if (PropertyPool.Exists(sa.Name)) {
								propertyInfo.SetValue(obj, PropertyPool.Get(sa.Name));
								continue;
							}

							try {
								var pType = propertyInfo.PropertyType;
								if (pType.GetGenericArguments().Length != 1)
									throw new RuntimeTypeException($"Unexpected length of type arguments at property: {propertyInfo.Name}");

								var types = new[] {pType.GetGenericArguments()[0]};
								var pBasicType = typeof(Property<>);
								var genericType = pBasicType.MakeGenericType(types);
								var property = (BaseProperty) Activator.CreateInstance(genericType);

								property.Name = sa.Name;
								property.Group = sa.Group;
								property.Format = sa.Format;

								PropertyPool.Register(sa.Name, property);

								propertyInfo.SetValue(obj, PropertyPool.Get(sa.Name));
							} catch (Exception e) {
								throw new RuntimeTypeException($"Unable to create property '{propertyInfo.Name}' : {e.Message}");
							}
						}
		}
	}


	/// <summary>
	///     Custom attribute to specify a global property
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class RegisterAttribute : Attribute {


		/// <summary>
		///     Attribute constructor.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="format"></param>
		/// <param name="group"></param>
		public RegisterAttribute(string name, string group = "", string format = "") {
			Name = name;
			Format = format;
			Group = group;
		}


		public string Name { get; set; }
		public string Group { get; set; }
		public string Format { get; set; }
	}


	/// <summary>
	///     Raised if some runtime type conversion fails.
	/// </summary>
	public class RuntimeTypeException : Exception {
		public RuntimeTypeException() {
		}


		public RuntimeTypeException(string message) : base(message) {
		}


		public RuntimeTypeException(string message, Exception innerException) : base(message, innerException) {
		}


		protected RuntimeTypeException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}

}
