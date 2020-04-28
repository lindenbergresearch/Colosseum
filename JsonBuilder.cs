using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Renoir {


	/// <summary>
	/// JSON builder utility class for dumping object data
	/// </summary>
	public class JBuilder {

		/// <summary>
		/// Default bindings
		/// </summary>
		public static readonly BindingFlags DefaultBindings =
			BindingFlags.Public |
			BindingFlags.GetField |
			BindingFlags.GetProperty |
			BindingFlags.Static;


		/// <summary>
		/// Convert an object to JSON string.
		/// Uses JToken method.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string ObjectToJson(object obj)
			=> ((JObject) JToken.FromObject(obj)).ToString();


		/// <summary>
		/// Creates a new JBuilder instance
		/// </summary>
		/// <param name="binding"></param>
		/// <param name="root"></param>
		/// <returns></returns>
		public static JBuilder Create(BindingFlags binding, JObject root = null)
			=> new JBuilder(binding, root);


		/// <summary>
		/// Creates a new JBuilder instance with default bindings
		/// </summary>
		/// <returns></returns>
		public static JBuilder Create()
			=> new JBuilder(DefaultBindings, null);


		/// <summary>
		/// JSON root 
		/// </summary>
		private JObject root;

		/// <summary>
		/// Binding flags to specify which members should be selected
		/// </summary>
		public BindingFlags BindingFlags { get; }


		/// <summary>
		/// Property which returns the current data as JSON string
		/// </summary>
		public string Json { get => root.ToString(); }


		/// <summary>
		/// Create a new JBuilder instance
		/// </summary>
		/// <param name="binding">Flags</param>
		/// <param name="root">root object</param>
		public JBuilder(BindingFlags binding, JObject root) {
			if (root != null) this.root = root;
			else this.root = new JObject();

			BindingFlags = binding;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		public void Add(Type type) {
			var jobj = new JObject();
			var fields = Util.ListFields(type, BindingFlags);
			var properties = Util.ListProperties(type, BindingFlags);

			foreach (var (key, value) in fields.Concat(properties)) {
				jobj[key] = value.ToString();
			}

			root[type.Name] = jobj;
		}

	}

}
