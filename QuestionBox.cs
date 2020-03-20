using Colosseum;
using Godot;

/// <summary>
/// </summary>
public class QuestionBox : StaticBody2D, ICollidable {
	/// <summary>
	///     The questionbox's hidden content
	/// </summary>
	public enum ContentType {
		NOTHING,
		COIN,
		ONEUP,
		POWERUP
	}


	/// <summary>
	///     The questionbox's current state
	/// </summary>
	[Export]
	public bool active = true;

	[GNode("AnimatedSprite")]
	private Godot.AnimatedSprite anim;
	[GNode("AnimationPlayer")]
	private AnimationPlayer bounce;
	[GNode("BumpSound")]
	private AudioStreamPlayer bumpSound;


	/// <summary>
	///     External property
	/// </summary>
	/// <returns></returns>
	[Export]
	public ContentType content = ContentType.NOTHING;


	/// <summary>
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
	///     Init...
	/// </summary>
	public override void _Ready() {
		this.SetupNodeBindings();

		if (active) anim.Animation = "Active";
		else anim.Animation = "Deactive";

		anim.Play();
	}


	/// <summary>
	/// </summary>
	/// <returns></returns>
	public override string ToString() {
		return $"QuesionBox: {content}";
	}
}