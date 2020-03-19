using Godot;
using System;
using System.Linq;

/// <summary>
/// Simple RichText messagebox for showing debug messages on
/// game screen.
/// </summary>
public class DebugMessageBox : RichTextLabel {
	/// <summary>
	/// The redraw interval in ms
	/// </summary>
	[Export]
	public int RedrawInterval { get; set; } = 500;

	[BindTo("../TextTimer")]
	private Godot.Timer _timer;

	[BindTo("../ColorRectDebug")]
	private Godot.ColorRect _colorRect;

	[BindTo("../MessageSound")]
	private AudioStreamPlayer _messageSound;
	
	private float current = -1;
	private int i = 0;


	private void _on_Timer_timeout() {
		Visible = false;
		_colorRect.Visible = false;
	}


	public override void _Ready() {
		this.SetupNodeBindings();

		Text = "[READY]";
	}


	private void UpdateMessageBox(float delta) {
		current += delta * 1000.0f;

		if (current == -1 || current >= RedrawInterval && Logger.messages.Count != i) {
			if (!_messageSound.Playing) _messageSound.Play();
			
			_timer.Stop();

			i = Logger.messages.Count;
			current = 0;

			Visible = true;
			_colorRect.Visible = true;

			Logger.messages.Reverse();
			var subset = Logger.messages.Take(17).ToList();
			Logger.messages.Reverse();

			var text = "";

			subset.Reverse();

			foreach (var message in subset) {
				text += message + "\n";
			}

			Text = text;
			_timer.Start();
		}
	}


	public override void _Process(float delta) {
		if (Logger.PrintDebug) UpdateMessageBox(delta);
	}
}
