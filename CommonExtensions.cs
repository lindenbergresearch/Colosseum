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
	/// String extensions
	/// </summary>
	public static class RString {

		/// <summary>
		/// Directly write a string to a file
		/// </summary>
		/// <param name="text">The string</param>
		/// <param name="filePath">The full path of the target filename</param>
		public static void ToTextFile(this string text, string filePath) {
			System.IO.File.WriteAllText(filePath, text);
		}


	}

}