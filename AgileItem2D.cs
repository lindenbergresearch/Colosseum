using Godot;
using Renoir;


/// <summary>
/// An abstract item which can be consumed by a consumer
/// </summary>
public class AgileItem2D : KinematicBody2D, ICollider {
	/// <summary>
	/// Determines of the item is active
	/// </summary>
	public bool Active { get; set; }


	/// <summary>
	/// Handle external collisions
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="collision"></param>
	public void OnCollide(object sender, KinematicCollision2D collision) {
		if (sender is IConsumer consumer) {
			consumer.OnConsume(this);
			Deactivate();
		}
	}


	/// <summary>
	/// Handle internal collisions
	/// </summary>
	/// <param name="delta"></param>
	public sealed override void _PhysicsProcess(float delta) {
		if (!Active) return;
		if (!Visible) Visible = true;

		foreach (var collision2D in this.GetCollider()) {
			if (collision2D.Collider is IConsumer consumer) {
				consumer.OnConsume(this);
				Deactivate();
				return;
			}
		}
	}


	/// <summary>
	/// Activate item
	/// </summary>
	public virtual void Activate() {
		Show();
		Active = true;
		AddChild(new VerticalMovingItem());
	}


	/// <summary>
	/// Deactivate item
	/// </summary>
	public virtual void Deactivate() {
		Active = false;
		Hide();
		QueueFree();
	}


	/// <summary>
	/// 
	/// </summary>
	public override void _Ready() {
		this.SetupNodeBindings();
		Hide();
	}

}


/// <summary>
/// 
/// </summary>
public class VerticalMovingItem : Node {
	private static readonly Vector2 INITIAL_IMPULSE = new Vector2(0, -25);

	private AgileItem2D AgileItem { get; set; }
	private Motion2D Motion { get; set; } = (0, 0);

	[GNode("../Label")] private Label _label;
	[GNode("../AudioStreamPlayer")] private AudioStreamPlayer _audioStreamPlayer;


	/// <summary>
	/// 
	/// </summary>
	public override void _Ready() {
		this.SetupNodeBindings();

		if (GetParent() is AgileItem2D agileItem2D)
			AgileItem = agileItem2D;

		Motion += INITIAL_IMPULSE;
		Logger.debug($"Ready.");

		_label.Visible = true;

		_audioStreamPlayer.Play();
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="delta"></param>
	public override void _PhysicsProcess(float delta) {
		_label.Text = ((Motion2D) AgileItem.Position).ToString();

		if (AgileItem.Position.y > -16)
			Motion = AgileItem.MoveAndSlide(Motion, Motion2D.FLOOR_NORMAL);
		else _audioStreamPlayer.Stop();
	}

}


/// <summary>
/// 
/// </summary>
public class HorizontalMovingItem : Node {
	private static readonly Vector2 GRAVITY = new Vector2(0, 900);
	private static readonly Vector2 INITIAL_IMPULSE = new Vector2(50, -250);

	private AgileItem2D AgileItem { get; set; }

	private Motion2D Motion { get; set; } = (0, 0);
	private Motion2D OldMotion2D { get; set; } = (0, 0);

	[GNode("../Label")] private Label _label;


	/// <summary>
	/// 
	/// </summary>
	public override void _Ready() {
		this.SetupNodeBindings();

		if (GetParent() is AgileItem2D agileItem2D)
			AgileItem = agileItem2D;


		_label.Visible = Logger.Level == Logger.LogLevel.TRACE;
		Motion += INITIAL_IMPULSE;


		Logger.debug($"Ready.");
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="delta"></param>
	public override void _PhysicsProcess(float delta) {
		foreach (var collision2D in AgileItem.GetCollider()) {
			if (!collision2D.Top()) Motion = OldMotion2D * -collision2D.Normal.Abs();
		}

		OldMotion2D = Motion.Velocity;

		_label.Text = $"Motion={Motion}";

		Motion += delta * GRAVITY;
		Motion = AgileItem.MoveAndSlide(Motion, Motion2D.FLOOR_NORMAL);
	}


}
