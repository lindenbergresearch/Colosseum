#region

using System;
using System.Reflection;

#endregion

namespace Renoir {

	/// <summary>
	/// Property Extensions
	/// </summary>
	public static class PropertyExtensions {

		/// <summary>
		/// Search custom attributed property and process it.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="obj"></param>
		/// <exception cref="RuntimeTypeException"></exception>
		private static void FindAndRegister(Type type, object obj = null) {
			foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy))
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
								property.Default = sa.Default;
								property.ID = PropertyPool.CurrentId++;
								property.UpdateSubscriber();

								PropertyPool.Register(sa.Alias, property);
								propertyInfo.SetValue(obj, PropertyPool.Get(sa.Alias));
							} catch (Exception e) {
								throw new RuntimeTypeException($"Unable to create property '{propertyInfo.Name}' : {e.Message}");
							}
						}
		}


		/// <summary>
		/// Search for global properties marked via custom attribute and create + register them
		/// in the property pool.
		/// </summary>
		/// <param name="obj"></param>
		public static void SetupGlobalProperties(this object obj) {
			Logger.trace($"Setup global properties for type: {obj.GetType().Name}");
			FindAndRegister(obj.GetType(), obj);
		}


		/// <summary>
		/// Search for global properties marked via custom attribute and create + register them
		/// in the property pool.
		/// Used by static classes only.
		/// </summary>
		/// <param name="type"></param>
		public static void SetupGlobalProperties(Type type) {
			Logger.trace($"Setup global properties for static type: {type.Name}");
			FindAndRegister(type);
		}


		/// <summary>
		/// Init method for objects used in Ibnitializer
		/// </summary>
		/// <param name="obj"></param>
		public static void InitGlobalProperties(object obj) {
			Logger.trace($"Setup global properties for type: {obj.GetType().FullName}");
			FindAndRegister(obj.GetType(), obj);
		}
	}


	/// <summary>
	/// Custom attribute to specify a global property
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class RegisterAttribute : Attribute {


		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		public RegisterAttribute(string alias) {
			Alias = alias;
			Format = "";
		}


		/// <summary>
		/// Attribute constructor.
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="format"></param>
		public RegisterAttribute(string alias, string format = "") {
			Alias = alias;
			Format = format;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="default"></param>
		/// <param name="format"></param>
		public RegisterAttribute(string alias, object @default = null, string format = "") {
			Alias = alias;
			Format = format;
			Default = @default;
		}


		public string Alias { get; set; }
		public string Format { get; set; }
		public object Default { get; set; }

	}


	/// <summary>
	/// Raised if some runtime type conversion fails.
	/// </summary>
	public class RuntimeTypeException : Exception {

		public RuntimeTypeException(string message) : base(message) { }

	}

}
