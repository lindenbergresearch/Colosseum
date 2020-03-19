using Godot;
using System;
using System.Diagnostics;
using Colosseum;

/// <summary>
/// Simple brick which could be damaged
/// </summary>
public class Brick : StaticBody2D, ICollidable {
	public bool Broken { get; set; } = false;


	private AudioStreamPlayer _audioStreamPlayer;
	private Godot.CollisionShape2D _collisionShape2D;
	private Particles2D _particles2D;
	private Sprite _sprite;


	/// <summary>
	/// Init
	/// </summary>
	public override void _Ready() {
		_audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		_collisionShape2D = GetNode<Godot.CollisionShape2D>("CollisionShape2D");
		_particles2D = GetNode<Particles2D>("Particles2D");
		_sprite = GetNode<Sprite>("Sprite");
	}


	/// <summary>
	/// Handle collision
	/// </summary>
	/// <param name="collision"></param>
	public void onCollide(KinematicCollision2D collision) {
		if (collision.Normal != Vector2.Down) return;

		_particles2D.Emitting = true;
		_audioStreamPlayer.Play();
		_sprite.Visible = false;
		_collisionShape2D.Disabled = true;
	}
}
