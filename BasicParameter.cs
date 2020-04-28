using System.IO;
using Newtonsoft.Json;

namespace Renoir {

	/// <summary>
	/// provides basic methods for handling persistent parameters
	/// </summary>
	public abstract class BasicParameter {
		/// <summary>
		/// Serialize to JSOn file
		/// </summary>
		/// <param name="fileName"></param>
		public void SaveJson(string fileName) {
			this.ToJson().ToTextFile(fileName);
		}


		/// <summary>
		/// Deserialize parameters from JSON file
		/// </summary>
		/// <param name="fileName">Path to JSON file</param>
		/// <returns></returns>
		public static T LoadFromJson<T>(string fileName) where T : BasicParameter {
			var json = File.ReadAllText(fileName);

			Logger.debug($"Try to load parameters from Json file: '{fileName}'");

			return JsonConvert.DeserializeObject<T>(json);
		}
	}

}
