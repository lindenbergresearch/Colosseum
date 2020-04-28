using System.Linq;
using Godot;
using static Renoir.Logger;


/// <summary>
/// Simple RichText messagebox for showing debug messages on
/// game screen.
/// </summary>
public class DebugMessageBox : RichTextLabel {

	[GNode("../../ColorRectDebug")]
	private ColorRect _colorRect;
	[GNode("../../MessageSound")]
	private AudioStreamPlayer _messageSound;
	[GNode("../../MessageSound2")]
	private AudioStreamPlayer _messageSound2;
	[GNode("../../TextTimer")]
	private Timer _timer;

	private float current = -1;
	private int i;
	private bool show;

	/// <summary>
	/// The redraw interval in ms
	/// </summary>
	[Export]
	public int RedrawInterval { get; set; } = 100;


	/*private void _on_Timer_timeout() {
		Visible = false;
		_colorRect.Visible = false;
	}*/


	public override void _Ready() {
		this.SetupNodeBindings();

		Visible = false;
		_colorRect.Visible = false;

		Text = "[READY]";
	}


	private void UpdateMessageBox(float delta) {
		current += delta * 1000.0f;

		if (current == -1 || current >= RedrawInterval && messages.Count != i) {
			if (!_messageSound.Playing) _messageSound.Play();

			//_timer.Stop();

			i = messages.Count;
			current = 0;

			//Visible = true;
			_colorRect.Visible = true;

			messages.Reverse();
			var subset = messages.ToList();
			messages.Reverse();

			var text = "";

			subset.Reverse();

			foreach (var message in subset) text += message + "\n";

			Text = text;

			//_timer.Start();
		}
	}


	public override void _Process(float delta) {
		if (Input.IsActionJustReleased("ui_tab")) {
			show = !show;

			if (show) _messageSound2.Play();

			Visible = show;
			_colorRect.Visible = show;
		}


		if (PrintDebug && show) UpdateMessageBox(delta);
	}
}
