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
		public static Parameter<string> Name { get; set; }
			= new("");

		public static Parameter<Vector2> Gravity { get; set; }
			= new(new Vector2(0, 1600));


		public static Parameter<int> Time { get; set; }
			= new(0, "{0:D3}");
	}

}
