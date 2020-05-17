using System;
using System.IO;
using Godot.Collections;
using Newtonsoft.Json.Linq;

namespace Renoir {


	public class DataClass {

		public class MetaData {
			public string fileName;
			public string json;

			public MetaData(string fileName, string json) {
				this.fileName = fileName;
				this.json = json;
			}
		}


		public static Dictionary<Type, MetaData> Meta = new Dictionary<Type, MetaData>();

	}


	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class DataClassAttribute : Attribute {

		private string jsonFile;
		private string json;

		public string Json {
			set {
				jsonFile = value;
				LoadJson();
			}

			get => jsonFile;
		}


		public string JsonText {
			get => json;
		}

		public bool Create { get; } = false;


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private void LoadJson() {
			if (jsonFile.Length < 1) return;

			try {
				Logger.trace($"Try to load json from file: {json}");
				json = File.ReadAllText(jsonFile);
			} catch (Exception e) {
				Logger.warn($"Unable to load json: '{json}' Exception is: {e.Message}");
			}
		}


		public DataClassAttribute() { }


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
			=> $"{GetType().FullName}: JsonFile={Json} CreateIfNotExists={Create}";


	}

}
