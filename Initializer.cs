#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Renoir {

	/// <summary>
	/// </summary>
	public static class Initializer {

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<KeyValuePair<Type, DataClassAttribute>> GetDataClassTypes() {
			foreach (var type in Util.GetAllTypes()) {
				foreach (var customAttribute in type.GetCustomAttributes(true)) {
					if (customAttribute is DataClassAttribute dataClassAttribute) {
						yield return new KeyValuePair<Type, DataClassAttribute>(type, dataClassAttribute);
						break;
					}
				}
			}
		}


		/// <summary>
		/// Static initialisation
		/// </summary>
		static Initializer() {
			//Logger.debug($"Size: {GetDataClassTypes().Count()}");
			GetDataClassTypes().Each(pair => {
				DataClass.MetaData meta;

				var (type, dataClassAttribute) = pair;
				meta.json = dataClassAttribute.JsonText;
				meta.fileName = dataClassAttribute.Json;

				// register data class meta data
				DataClass.Meta.Add(type, meta);
			});
		}


		/// <summary>
		/// Instance initialisation
		/// </summary>
		/// <param name="obj"></param>
		public static void Init(this object obj) {
			Logger.trace($"Init object: {obj}");

			ExecutorPool.Add("global property bindings", false, PropertyExtensions.InitGlobalProperties);
			ExecutorPool.Add("dynamic node bindings", false, DynamicBindings.InitNodeBindings);

			ExecutorPool.RunAll(obj);
		}


		/// <summary>
		/// 
		/// </summary>
		public static void Run() {
			Logger.debug("Init core engine...");
		}

	}


}
