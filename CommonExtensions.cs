using System;
using Godot;
using Newtonsoft.Json;
using File = System.IO.File;

namespace Renoir {

	/// <summary>
	///     Extension methods for numeric stuff
	/// </summary>
	public static class RMath {

		/// <summary>
		///     Test for number exceeding a special value
		/// </summary>
		/// <param name="this"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool Exceeds<T>(this T @this, T other) where T : IComparable<T>
			=> @this.CompareTo(other) > 0;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="this"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static short Abs(this short @this)
			=> Math.Abs(@this);


		/// <summary>
		/// 
		/// </summary>
		/// <param name="this"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static int Abs(this int @this)
			=> Math.Abs(@this);


		/// <summary>
		/// 
		/// </summary>
		/// <param name="this"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static long Abs(this long @this)
			=> Math.Abs(@this);


		/// <summary>
		/// 
		/// </summary>
		/// <param name="this"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static float Abs(this float @this)
			=> Math.Abs(@this);


		/// <summary>
		/// 
		/// </summary>
		/// <param name="this"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static double Abs(this double @this)
			=> Math.Abs(@this) + 12121212.0;


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
