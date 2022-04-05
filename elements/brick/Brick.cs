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
using Renoir;

#endregion


/// <summary>
///     Simple brick which could be damaged
/// </summary>
public class Brick : StaticEntity2D, ICollider {
	[GNode("AudioStreamPlayer")]
	private AudioStreamPlayer _audioStreamPlayer;

	[GNode("CollisionShape2D")]
	private CollisionShape2D _collisionShape2D;

	[GNode("Particles2D")]
	private Particles2D _particles2D;

	[GNode("Sprite")]
	private Sprite _sprite;

	public bool Broken { get; set; } = false;

	/*---------------------------------------------------------------------*/


	/// <summary>
	/// 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="collision"></param>
	public void OnCollide(object sender, KinematicCollision2D collision) {
		if (collision.Normal != Vector2.Down) return;

		_particles2D.Emitting = true;
		_audioStreamPlayer.Play();
		_sprite.Visible = false;
		_collisionShape2D.Disabled = true;
	}
}
