using System.Linq;
using Godot;


/// <summary>
///     Simple RichText messagebox for showing debug messages on
///     game screen.
/// </summary>
public class DebugMessageBox : RichTextLabel {

	[GNode("../ColorRectDebug")]
	private ColorRect _colorRect;

	[GNode("../MessageSound")]
	private AudioStreamPlayer _messageSound;

	[GNode("../TextTimer")]
	private Timer _timer;

	private float current = -1;
	private int i;

	/// <summary>
	///     The redraw interval in ms
	/// </summary>
	[Export]
	public int RedrawInterval { get; set; } = 500;


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

			foreach (var message in subset) text += message + "\n";

			Text = text;
			_timer.Start();
		}
	}


	public override void _Process(float delta) {
		if (Logger.PrintDebug) UpdateMessageBox(delta);
	}

}