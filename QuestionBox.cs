using Godot;
using Renoir;


/// <summary>
/// </summary>
public class QuestionBox : StaticBody2D, ICollider {

	/// <summary>
	///     The question-box hidden content
	/// </summary>
	public enum ContentType {
		NOTHING,
		COIN,
		ONEUP,
		MUSHROOM,
		FLOWER,
		STAR
	}


	/// <summary>
	///     The questionbox's current state
	/// </summary>
	[Export]
	public bool Active { get; set; } = true;

	[GNode("AnimatedSprite")] private Godot.AnimatedSprite _animatedSprite;
	[GNode("AnimationPlayer")] private AnimationPlayer _animationPlayer;
	[GNode("BumpSound")] private AudioStreamPlayer _audioStreamPlayer;
	[GNode("AgileItem2D")] private AgileItem2D _agileItem2D;
	[GNode("AgileItem2D/AnimatedSprite")] private Godot.AnimatedSprite _animatedSpriteItem;


	/// <summary>
	///     External property
	/// </summary>
	/// <returns></returns>
	[Export]
	public ContentType Content { get; set; } = ContentType.NOTHING;


	/// <summary>
	/// 	Handle collisions
	/// </summary>
	public void OnCollide(object sender, KinematicCollision2D collision) {
		if (!collision.Bottom()) return;

		_audioStreamPlayer.Play();

		if (!Active) return;

		SetContent();

		Active = false;

		_animationPlayer.CurrentAnimation = "Bounce";
		_animationPlayer.Play();

		_animatedSprite.Animation = "Deactive";
		_animatedSprite.Play();

		if (Content != ContentType.NOTHING)
			_agileItem2D.Activate();
	}


	/// <summary>
	/// 
	/// </summary>
	private void SetContent() {
		switch (Content) {
			case ContentType.MUSHROOM:
				_agileItem2D.Item = new HorizontalMovingItem();
				_animatedSpriteItem.Play("Mushroom");
				break;
			case ContentType.FLOWER:
				_agileItem2D.Item = new VerticalMovingItem();
				_animatedSpriteItem.Play("Flower");
				break;
			default:
				_agileItem2D.Item = new HorizontalMovingItem();
				_animatedSpriteItem.Play("Mushroom");
				break;
		}
	}


	/// <summary>
	///     Init...
	/// </summary>
	public override void _Ready() {
		this.SetupNodeBindings();

		if (Active) _animatedSprite.Animation = "Active";
		else _animatedSprite.Animation = "Deactive";

		_animatedSprite.Play();
		_agileItem2D.Visible = false;
	}


	/// <summary>
	/// </summary>
	/// <returns></returns>
	public override string ToString()
		=> $"QuestionBox({Content})";


}
