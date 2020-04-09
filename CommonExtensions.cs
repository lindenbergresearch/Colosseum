using System.IO;
using Newtonsoft.Json;

namespace Renoir {

	/// <summary>
	///     Extension methods for numeric stuff
	/// </summary>
	public static class RMath {
		/// <summary>
		///     Test for number exceeding a special value
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool exceeds(this float x, float y) {
			return x > y;
		}
	}


	/// <summary>
	///     String extensions
	/// </summary>
	public static class RString {

		/// <summary>
		///     Directly write a string to a file
		/// </summary>
		/// <param name="text">The string</param>
		/// <param name="filePath">The full path of the target filename</param>
		public static void ToTextFile(this string text, string filePath) {
			File.WriteAllText(filePath, text);
		}
	}


	/// <summary>
	///     Object extentions
	/// </summary>
	public static class RObject {

		/// <summary>
		///     Converts the current data of an object to a JSON string
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string ToJson(this object obj) {
			return JsonConvert.SerializeObject(obj);
		}


		/// <summary>
		/// Dump the object's content to string 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string Dump(this object obj)
			=> Util.Dump(obj);
	}


}
