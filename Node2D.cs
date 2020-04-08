using System;
using Godot;
using Renoir;
using static Renoir.Logger;


/// <summary>
/// Basic configuration
/// </summary>
public static class Game {
	public static readonly int TILE_SIZE = 8;
	public static readonly int SCALE_FACTOR = 3;
	public static readonly Vector2 VIEWPORT_TILES = new Vector2(60, 40);
	public static readonly Vector2 VIEWPORT_RESOLUTION = VIEWPORT_TILES * TILE_SIZE;
	public static readonly Vector2 WINDOW_RESOLUTION = VIEWPORT_TILES * TILE_SIZE * SCALE_FACTOR;
}


/// <summary>
/// ROOT node 
/// </summary>
public class Node2D : Godot.Node2D {


	private float time;

	[Register("main.level.name", "", "$main.playerinfo")]
	public Property<string> pLevelName { get; set; }

	[Register("main.level.time", "{0:D3}", "$main.playerinfo")]
	public Property<int> pTime { get; set; }

	[Export] public bool EnableDebug { get; set; }
	[Export] public string LevelName { get; set; }


	/// <summary>
	/// 
	/// </summary>
	public void setupViewport() {
		debug($"Set viewport to resolution: {Game.VIEWPORT_RESOLUTION} Game has resolution: {Game.WINDOW_RESOLUTION}");

		GetViewport().SetSizeOverride(true, Game.VIEWPORT_RESOLUTION);
		GetViewport().SizeOverrideStretch = true;

		OS.WindowSize = Game.WINDOW_RESOLUTION;
	}


	/**
	 * 	 Called when the node is ready.
	 *	 Setup man game stuff.
	 *
	 */
	public override void _Ready() {
		this.SetupGlobalProperties();

		PrintDebug = EnableDebug;
		setupViewport();

		time = 300;
		pTime.Value = 0;

		pLevelName.Value = LevelName;

		Input.SetCustomMouseCursor(ResourceLoader.Load("mouse_pointer.png"));
	}


	public override void _Process(float delta) {
		if (Input.IsActionJustPressed("Reload")) GetTree().ReloadCurrentScene();

		time -= delta;

		var rounded = (int) Math.Round(time);

		if (pTime.Value != rounded)
			pTime.Value = rounded;
	}
}
