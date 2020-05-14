#region

using System;
using Godot;
using static Renoir.PropertyExtensions;

#endregion

namespace Renoir {

	/// <summary>
	/// Property class:
	/// Holds all properties for the current level
	/// </summary>
	[DataClass(Json = "sdsdsdsdsd")]
	public class Level {


		/// <summary>
		/// The full name of the level
		/// </summary>
		[Register("main.level.name")]
		public static Property<string> Name { get; set; }

		/// <summary>
		/// The levels gravity vector
		/// </summary>
		[Register("main.level.gravity")]
		public static Property<Vector2> Gravity { get; set; }

		/// <summary>
		/// Current time
		/// </summary>
		[Register("main.level.time", "{0:D3}")]
		public static Property<int> Time { get; set; }


		


	}

}
