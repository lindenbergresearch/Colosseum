#region header

// 
//    _____
//   (, /   )            ,
//     /__ /  _ __   ___   __
//  ) /   \__(/_/ (_(_)_(_/ (_  CORE LIBRARY
// (_/ ______________________________________/
// 
// 
// Renoir Core Library for the Godot Game-Engine.
// Copyright 2020-2022 by Lindenberg Research.
// 
// www.lindenberg-research.com
// www.godotengine.org
// 

#endregion

#region

using System;
using System.Reflection;
using Newtonsoft.Json.Linq;

#endregion

namespace Renoir {


	/// <summary>
	///     JSON builder utility class for dumping object data
	/// </summary>
	public class JBuilder {

		/// <summary>
		///     Default bindings
		/// </summary>
		public const BindingFlags DefaultBindings = BindingFlags.Public
		                                            | BindingFlags.GetField
		                                            | BindingFlags.GetProperty
		                                            | BindingFlags.Static
		                                            | BindingFlags.Instance;


		/// <summary>
		///     JSON root
		/// </summary>
		private readonly JObject root;


		/// <summary>
		///     Create a new JBuilder instance
		/// </summary>
		/// <param name="binding">Flags</param>
		/// <param name="root">root object</param>
		public JBuilder(BindingFlags binding, JObject root) {
			if (root != null) this.root = root;
			else this.root = new JObject();

			BindingFlags = binding;
		}

		/// <summary>
		///     Binding flags to specify which members should be selected
		/// </summary>
		public BindingFlags BindingFlags { get; }


		/// <summary>
		///     Property which returns the current data as JSON string
		/// </summary>
		public string Json => root.ToString();


		/// <summary>
		///     Convert an object to JSON string.
		///     Uses JToken method.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string ObjectToJson(object obj) {
			return ((JObject) JToken.FromObject(obj)).ToString();
		}


		/// <summary>
		///     Creates a new JBuilder instance
		/// </summary>
		/// <param name="binding"></param>
		/// <param name="root"></param>
		/// <returns></returns>
		public static JBuilder Create(BindingFlags binding, JObject root = null) {
			return new JBuilder(binding, root);
		}


		/// <summary>
		///     Creates a new JBuilder instance with default bindings
		/// </summary>
		/// <returns></returns>
		public static JBuilder Create() {
			return new JBuilder(DefaultBindings, null);
		}


		/// <summary>
		///     Examines the given type and optionally an object instance.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="obj"></param>
		private void ExamineType(Type type, object obj = null) {
			var jObject = new JObject();
			var jFields = new JObject();
			var jProperties = new JObject();
			var jMethods = new JArray();

			var fields = Util.ListFields(type, obj, BindingFlags);
			var properties = Util.ListProperties(type, obj, BindingFlags);

			properties.Each(p => jFields[p.Item1] = p.Item2);
			fields.Each(p => jFields[p.Item1] = p.Item2);
			type.GetMethods().Each(m => jMethods.Add((JToken) m.ToString()));

			jObject["fields"] = jFields;
			jObject["properties"] = jProperties;
			jObject["methods"] = jMethods;

			root[type.FullName ?? "<null>"] = jObject;
		}


		public static JObject FromProperties(Type type, object obj = null) {
			var jRoot = new JObject();

			type.GetProperties()
				.Each(p => jRoot[p.Name] = (JToken) p.GetValue(obj));

			return jRoot;
		}


		/// <summary>
		///     Add via type
		/// </summary>
		/// <param name="type"></param>
		public void Add(Type type) {
			ExamineType(type);
		}


		/// <summary>
		///     Add via object
		/// </summary>
		/// <param name="obj"></param>
		public void Add(object obj) {
			ExamineType(obj.GetType(), obj);
		}


		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString() {
			return Json;
		}
	}

}