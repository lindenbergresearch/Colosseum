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

namespace Renoir;

/// <summary>
///     Helper class for key shortcuts
/// </summary>
public static class ActionKey {
	public static bool Up => Input.IsActionPressed("ui_up");
	public static bool Down => Input.IsActionPressed("ui_down");
	public static bool Left => Input.IsActionPressed("ui_left");
	public static bool Right => Input.IsActionPressed("ui_right");
	public static bool Run => Input.IsActionPressed("ui_accept");
	public static bool Jump => Input.IsActionJustPressed("ui_cancel");
	public static bool Select => Input.IsActionJustPressed("ui_select");
}
