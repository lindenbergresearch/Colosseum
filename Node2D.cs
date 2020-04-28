using System;
using Godot;
using Renoir;
using static Renoir.Logger;
using Level = Renoir.Level;


/// <summary>
/// Basic configuration
/// </summary>
public static class Game {

	public const int TILE_SIZE = 8;
	public const int SCALE_FACTOR = 3;
	public static readonly Vector2 VIEWPORT_TILES = new Vector2(60, 40);
	public static readonly Vector2 VIEWPORT_RESOLUTION = VIEWPORT_TILES * TILE_SIZE;
	public static readonly Vector2 WINDOW_RESOLUTION = VIEWPORT_TILES * TILE_SIZE * SCALE_FACTOR;

	[Register("main.game.active")]
	public static Property<bool> Active { get; set; }
}


/// <summary>
/// ROOT node
/// </summary>
public class Node2D : Godot.Node2D {

	private float time, fps;

	[Register("debug.fps", "{0:F2} FPS")]
	public static Property<float> FPS { get; set; }
	

	[Export]
	public bool EnableDebug { get; set; }


	[Export]
	private readonly string levelName;


	[Register("main.mouse.button")]
	public static Property<InputEventMouseButton> MouseButton { get; set; }


	/// <summary>
	/// </summary>
	public void setupViewport() {
		debug($"Set viewport to resolution: {Game.VIEWPORT_RESOLUTION} Game has resolution: {Game.WINDOW_RESOLUTION}");

		GetViewport().SetSizeOverride(true, Game.VIEWPORT_RESOLUTION);
		GetViewport().SizeOverrideStretch = true;

		OS.WindowSize = Game.WINDOW_RESOLUTION;
	}


	/// <summary>
	/// </summary>
	public override void _Ready() {
		debug($"Loading: {GetType().FullName}");

		this.SetupGlobalProperties();

		PrintDebug = EnableDebug;
		setupViewport();
		
		Level.Name.Value = levelName;
		Level.Gravity.Value = new Vector2(0, 1200);
		Level.LevelTime.Value = 0;

		time = 300;

		Input.SetCustomMouseCursor(ResourceLoader.Load("mouse_pointer.png"));
	}


	public override void _Process(float delta) {
		if (Input.IsActionJustPressed("Reload")) {
			PropertyPool.Clear();
			GetTree().ReloadCurrentScene();
			return;
		}

		time -= delta;

		var rounded = (int) Math.Round(time);

		if (Level.LevelTime.Value != rounded) Level.LevelTime.Value = rounded;

		if (fps > 0) fps = Mathf.Lerp(fps, 1 / delta, 0.1f);
		else fps = 1 / delta;

		FPS.Value = fps;
	}


	/// <summary>
	/// TODO: Embed in common mouse handler or so... ;P 
	/// </summary>
	/// <param name="event"></param>
	/*public override void _Input(InputEvent @event) {
		if (!(@event is InputEventMouseButton eventMouseButton)) return;

		MouseButton.Value = eventMouseButton;

		// debug($"Mouse Click/Unclick at: {eventMouseButton.Position} {eventMouseButton.GlobalPosition} {eventMouseButton.Pressed}");
		// debug($"Viewport Resolution is: {GetViewportRect().Size}");
	}*/

}
