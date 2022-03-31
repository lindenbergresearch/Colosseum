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


/// <summary>
///     Interface to implement collision handling
/// </summary>
public interface ICollider {
	/// <summary>
	///     Triggered by the collider
	/// </summary>
	/// <param name="collision"></param>
	void OnCollide(object sender, KinematicCollision2D collision);
}
