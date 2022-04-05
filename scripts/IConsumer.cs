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

namespace Renoir {

	/// <summary>
	/// </summary>
	public interface IConsumer {
		/// <summary>
		/// </summary>
		/// <param name="item"></param>
		void OnConsume(object item);
	}

}
