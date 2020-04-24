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
							if (PropertyPool.Exists(sa.Alias)) {
								var p = PropertyPool.Get(sa.Alias);

								propertyInfo.SetValue(obj, p);
								if (p is BaseProperty bp) bp.UpdateSubscriber();
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

								property.Alias = sa.Alias;
								property.Format = sa.Format;
								property.ID = PropertyPool.CurrentId++;
								property.UpdateSubscriber();

								PropertyPool.Register(sa.Alias, property);
								propertyInfo.SetValue(obj, PropertyPool.Get(sa.Alias));
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
		/// <param name="alias"></param>
		/// <param name="format"></param>
		public RegisterAttribute(string alias, string format = "") {
			Alias = alias;
			Format = format;
		}


		public string Alias { get; set; }
		public string Format { get; set; }

	}


	/// <summary>
	///     Raised if some runtime type conversion fails.
	/// </summary>
	public class RuntimeTypeException : Exception {

		public RuntimeTypeException(string message) : base(message) { }

	}

}