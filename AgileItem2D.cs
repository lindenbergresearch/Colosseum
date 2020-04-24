using Godot;
using Renoir;


/// <summary>
/// An abstract item which can be consumed by a consumer
/// </summary>
public class AgileItem2D : KinematicEntity2D, ICollider {

	/// <summary>
	/// </summary>
	public Node Item { get; set; }


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
	protected override void PhysicsProcess(float delta) {
		if (!Active) return;
		if (!Visible) Visible = true;

		foreach (var collision2D in this.GetCollider())
			if (collision2D.Collider is IConsumer consumer) {
				consumer.OnConsume(this);
				Deactivate();
				return;
			}
	}


	/// <summary>
	/// Activate item
	/// </summary>
	public void Activate() {
		Show();
		Active = true;
		if (Item != null) AddChild(Item);
	}


	/// <summary>
	/// Deactivate item
	/// </summary>
	public void Deactivate() {
		Active = false;
		Hide();
		QueueFree();
	}


	/// <summary>
	/// </summary>
	protected override void Ready() {
		Hide();
	}

}


/// <summary>
/// </summary>
public class VerticalMovingItem : Node {
	private static readonly Vector2 INITIAL_IMPULSE = new Vector2(0, -25);

	private AgileItem2D AgileItem { get; set; }


	[GNode("../Label")]
	private Label _label;

	[GNode("../AudioStreamPlayer")]
	private AudioStreamPlayer _audioStreamPlayer;


	/// <summary>
	/// </summary>
	public override void _Ready() {
		this.SetupNodeBindings();

		if (GetParent() is AgileItem2D agileItem2D)
			AgileItem = agileItem2D;

		AgileItem.Motion += INITIAL_IMPULSE;
		Logger.debug("Ready.");

		_label.Visible = true;
		_audioStreamPlayer.Play();
	}


	/// <summary>
	/// </summary>
	/// <param name="delta"></param>
	public override void _PhysicsProcess(float delta) {
		_label.Text = ((Motion2D) AgileItem.Position).ToString();

		if (AgileItem.Position.y > -16) AgileItem.UpdateMotion();
		else _audioStreamPlayer.Stop();
	}

}


/// <summary>
/// </summary>
public class HorizontalMovingItem : Node {
	private static readonly Vector2 GRAVITY = new Vector2(0, 900);
	private static readonly Vector2 INITIAL_IMPULSE = new Vector2(50, -250);

	private AgileItem2D AgileItem { get; set; }
	private Motion2D OldMotion2D { get; set; } = (0, 0);

	[GNode("../Label")]
	private Label _label;


	/// <summary>
	/// </summary>
	public override void _Ready() {
		this.SetupNodeBindings();

		if (GetParent() is AgileItem2D agileItem2D)
			AgileItem = agileItem2D;

		_label.Visible = Logger.Level == Logger.LogLevel.TRACE;
		AgileItem.Motion += INITIAL_IMPULSE;

		Logger.debug("Ready.");
	}


	/// <summary>
	/// </summary>
	/// <param name="delta"></param>
	public override void _PhysicsProcess(float delta) {
		foreach (var collision2D in AgileItem.GetCollider())
			if (!collision2D.Top())
				AgileItem.Motion = OldMotion2D * -collision2D.Normal.Abs();

		OldMotion2D = AgileItem.Motion.Velocity;

		_label.Text = $"Motion={AgileItem.Motion}";

		AgileItem.Motion += delta * GRAVITY;
		AgileItem.UpdateMotion();
	}


}
