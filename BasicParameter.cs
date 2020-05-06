#region

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

#endregion

namespace Renoir {

	/// <summary>
	/// provides basic methods for handling persistent parameters
	/// </summary>
	public abstract class BasicParameter {
		/// <summary>
		/// Serialize to Json file
		/// </summary>
		/// <param name="fileName"></param>
		public void SaveJson(string fileName) {
			this.ToJson().ToTextFile(fileName);
		}


		/// <summary>
		/// Deserialize parameters from Json file
		/// </summary>
		/// <param name="fileName">Path to JSON file</param>
		/// <returns></returns>
		public static T LoadFromJson<T>(string fileName) where T : BasicParameter {
			var json = File.ReadAllText(fileName);

			Logger.debug($"Try to load parameters from Json file: '{fileName}'");

			return JsonConvert.DeserializeObject<T>(json);
		}
	}


	/// <summary>
	/// Custom attribute to specify a global property
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public abstract class ConstraintAttribute : Attribute {

		private enum Type {
			Range,
			Blacklist,
			Invalid,
			RegEx
		}


		public dynamic From { get; set; }
		public dynamic To { get; set; }
		public List<dynamic> Blacklist { get; set; }
		public string RegEx { get; set; }

		private Type type;


		/// <summary>
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		public ConstraintAttribute(dynamic from, dynamic to) {
			From = from;
			To = to;
			type = Type.Range;
		}


		/// <summary>
		/// </summary>
		/// <param name="blacklist"></param>
		public ConstraintAttribute(List<dynamic> blacklist) {
			Blacklist = blacklist;
			type = Type.Blacklist;
		}


		/// <summary>
		/// </summary>
		/// <param name="invalid"></param>
		public ConstraintAttribute(dynamic invalid) {
			Blacklist = new List<dynamic>(invalid);
			type = Type.Invalid;
		}


		/// <summary>
		/// </summary>
		/// <param name="regex"></param>
		public ConstraintAttribute(string regex) {
			RegEx = regex;
			type = Type.RegEx;
		}


	}

}