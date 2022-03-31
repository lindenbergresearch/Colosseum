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

using System;
using Godot;
using Renoir;
using static Renoir.Logger;

#endregion


/// <summary>
///     Basic configuration
/// </summary>
public static class Game {
	public const int TILE_SIZE = 8;
	public const int SCALE_FACTOR = 3;
	public static readonly Vector2 VIEWPORT_TILES = new(60, 40);
	public static readonly Vector2 VIEWPORT_RESOLUTION = VIEWPORT_TILES * TILE_SIZE;
	public static readonly Vector2 WINDOW_RESOLUTION = VIEWPORT_TILES * TILE_SIZE * SCALE_FACTOR;


	public static Parameter<bool> Active { get; set; }
}


/// <summary>
///     ROOT node
/// </summary>
public class Node2D : Godot.Node2D {
	[Export]
	private readonly string levelName;

	private float time, fps;

	public static Parameter<float> FPS { get; set; }
		= new(0, "{0:F2}");


	[Export]
	public bool EnableDebug { get; set; }


	/// <summary>
	/// </summary>
	public void setupViewport() {
		debug($"Set viewport to resolution: {Game.VIEWPORT_RESOLUTION} Game has resolution: {Game.WINDOW_RESOLUTION}");

		GetViewport().SetSizeOverride(true, Game.VIEWPORT_RESOLUTION);
		GetViewport().SizeOverrideStretch = true;

		OS.WindowSize = Game.WINDOW_RESOLUTION;
		OS.CenterWindow();
	}


	/// <summary>
	/// </summary>
	public override void _Ready() {
		debug($"Loading: {GetType().FullName}");

		this.Init();

		PrintDebug = EnableDebug;
		setupViewport();

		Level.Name.Value = levelName;
		Level.Gravity.Value = new Vector2(0, 1200);
		Level.Time.Value = 0;

		time = 300;

		Input.SetCustomMouseCursor(ResourceLoader.Load("mouse_pointer.png"));
	}


	public override void _Process(float delta) {
		if (Input.IsActionJustPressed("Reload")) {
			GetTree().ReloadCurrentScene();
			return;
		}

		time -= delta;

		var rounded = (int) Math.Round(time);

		if (Level.Time.Value != rounded) Level.Time.Value = rounded;

		if (fps > 0) fps = Mathf.Lerp(fps, 1 / delta, 0.1f);
		else fps = 1 / delta;

		FPS.Value = fps;
	}


	// <summary>
	// TODO: Embed in common mouse handler or so... ;P 
	// </summary>
	// <param name="event"></param>
	/*public override void _Input(InputEvent @event) {
		if (!(@event is InputEventMouseButton eventMouseButton)) return;

		MouseButton.Value = eventMouseButton;

		// debug($"Mouse Click/Unclick at: {eventMouseButton.Position} {eventMouseButton.GlobalPosition} {eventMouseButton.Pressed}");
		// debug($"Viewport Resolution is: {GetViewportRect().Size}");
	}*/
}
