using System;

namespace Renoir {

	/// <summary>
	/// 
	/// </summary>
	public static class Initializer {

		/// <summary>
		/// List of static initializers
		/// </summary>
		public static Type[] staticInitList = {
			typeof(Logger),
			typeof(MainApp)
		};


		/// <summary>
		/// Static initialisation
		/// </summary>
		static Initializer() {
			staticInitList.Each(type => RunInit(type));
		}


		/// <summary>
		/// Instance initialisation
		/// </summary>
		/// <param name="obj"></param>
		public static void Init(this object obj) {
			Logger.trace($"Init object: {obj}");
			
			ExecutorPool.Add("global property bindings", true, PropertyExtensions.InitGlobalProperties);
			ExecutorPool.Add("dynamic node bindings", true, DynamicBindings.InitNodeBindings);
			
			ExecutorPool.RunAll(obj);
		}


		/// <summary>
		/// Examine type and run all init methods
		/// </summary>
		/// <param name="type"></param>
		/// <param name="methodPrefix"></param>
		private static void RunInit(Type type, string methodPrefix = "Init") {
			foreach (var methodInfo in type.GetMethods()) {
				if (!methodInfo.Name.StartsWith(methodPrefix) || methodInfo.GetParameters().Length != 0) continue;
				Logger.trace($"Invoking init method: {type.FullName} => {methodInfo.Name}");
				methodInfo.Invoke(null, new object[] { });
			}
		}

	}


}
