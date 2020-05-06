#region

using Godot;
using Renoir;

#endregion


/// <summary>
/// Opening animation and sound
/// </summary>
public class Opening : Godot.Node2D {

	[GNode("AnimationPlayer")]
	private AnimationPlayer _animationPlayer;

	[GNode("AudioStreamPlayer")]
	private AudioStreamPlayer _audioStreamPlayer;


	/// <summary>
	/// Pre-delay of opening is setup in Godot Timer properties.
	/// </summary>
	public void _on_Timer_timeout() {
		_animationPlayer.CurrentAnimation = "Zoomer";
		_animationPlayer.Play();

		_audioStreamPlayer.Play();
	}


	/// <summary>
	/// Init
	/// </summary>
	public override void _Ready() {
		this.Init();
	}

}
