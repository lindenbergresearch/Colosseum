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
		/// Get all static classes which have been marked as DataClass
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
			Logger.debug("Initialize static data classes:");
			var c = 0;
			
			GetDataClassTypes().Each(pair => {
				DataClass.MetaData meta;
				var (type, dataClassAttribute) = pair;

				meta = new DataClass.MetaData(dataClassAttribute.Json, dataClassAttribute.JsonText);

				// register data class meta data
				DataClass.Meta.Add(type, meta);

				PropertyExtensions.SetupGlobalProperties(type);
				c++;
			});
			
			Logger.debug($"Processed: {c} dataclass(es).");
		}


		/// <summary>
		/// Instance initialisation
		/// </summary>
		/// <param name="obj"></param>
		public static void Init(this object obj) {
			Logger.debug($"Initialize object: {obj}");

			PropertyExtensions.InitGlobalProperties(obj);
			DynamicBindings.InitNodeBindings(obj);
		}


		/// <summary>
		/// 
		/// </summary>
		public static void Run() {
			Logger.debug("Init core engine...");
		}

	}


}
