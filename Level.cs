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

using Godot;

#endregion

namespace Renoir {

	/// <summary>
	///     Property class:
	///     Holds all properties for the current level
	/// </summary>
	public static class Level {




		/// <summary>
		///     The full name of the level
		/// </summary>
		[Register("main.level.name")]
		public static Property<string> Name { get; set; }

		/// <summary>
		///     The levels gravity vector
		/// </summary>
		[Register("main.level.gravity")]
		public static Property<Vector2> Gravity { get; set; }

		/// <summary>
		///     Current time
		/// </summary>
		[Register("main.level.time", "{0:D3}")]
		public static Property<int> Time { get; set; }
	}

}