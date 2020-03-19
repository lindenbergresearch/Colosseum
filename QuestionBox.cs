using Godot;
using System;
using Colosseum;

/// <summary>
/// 
/// </summary>
public class QuestionBox : StaticBody2D, ICollidable {
	/// <summary>
	/// The questionbox's hidden content
	/// </summary>
	public enum ContentType {
		NOTHING,
		COIN,
		ONEUP,
		POWERUP
	}

	private Godot.AnimatedSprite anim;

	/// <summary>
	/// External property 
	/// </summary>
	/// <returns></returns>
	[Export] public ContentType content = ContentType.NOTHING;


	/// <summary>
	///  The questionbox's current state
	/// </summary>
	[Export] public bool active = true;


	private AudioStreamPlayer bumpSound;
	private AnimationPlayer bounce;


	/// <summary>
	/// 
	/// </summary>
	public void onCollide(KinematicCollision2D collision) {
		if (collision.Normal != Vector2.Down) return;

		
		if (!active) {
			bumpSound.Play();
			return;
		}
		
		active = false;

		bumpSound.Play();

		bounce.CurrentAnimation = "Bounce";
		bounce.Play();
		
		anim.Animation = "Deactive";
		anim.Play();
	}


	/// <summary>
	/// Init...
	/// </summary>
	public override void _Ready() {
		anim = GetNode<Godot.AnimatedSprite>("AnimatedSprite");

		if (active) anim.Animation = "Active";
		else anim.Animation = "Deactive";

		anim.Play();


		bumpSound = GetNode<AudioStreamPlayer>("BumpSound");
		bounce = GetNode<AnimationPlayer>("AnimationPlayer");
	}


	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public override string ToString() {
		return $"QuesionBox: {content}";
	}
}
