using Godot;
using System;


/// <summary>
/// Opening animation and sound
/// </summary>
public class Opening : Node2D {

	[GNode("AnimationPlayer")]
	private AnimationPlayer _animationPlayer;


	/// <summary>
	/// Init
	/// </summary>
	public override void _Ready() {
		this.SetupNodeBindings();
		_animationPlayer.Play();
	}

}
