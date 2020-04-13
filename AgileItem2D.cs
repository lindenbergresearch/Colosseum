using Godot;
using System;
using System.Collections.Generic;
using Renoir;


/// <summary>
/// Basic consumable item
/// </summary>
public class AgileItem2D : KinematicBody2D, ICollider {
	private static readonly Vector2 GRAVITY = new Vector2(0, 900);
	private static readonly Vector2 INITIAL_IMPULSE = new Vector2(50, -250);

	private Motion2D Motion { get; set; } = (0, 0);
	private Motion2D Save { get; set; } = (0, 0);

	public bool Active { get; set; }

	[GNode("Label")] private Label _label;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="collision"></param>
	public void OnCollide(object sender, KinematicCollision2D collision) {
		Logger.debug($"coll: {sender.GetType().FullName}");
		if (sender is IConsumer consumer)
			consumer.OnConsume(this);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="delta"></param>
	public override void _PhysicsProcess(float delta) {
		if (!Active) return;
		if (!Visible) Visible = true;

		foreach (var collision2D in this.GetCollider()) {
			if (collision2D.Collider is IConsumer consumer) {
				consumer.OnConsume(this);
				continue;
			}

			if (collision2D.Top()) continue;

			Logger.debug($"position={collision2D.Position} motion={Save} velocity={collision2D.ColliderVelocity} collider={collision2D.Collider} vector={collision2D.Normal}");
			Motion = Save * -collision2D.Normal.Abs();
		}

		Save = Motion.Velocity;

		_label.Text = $"Motion={Motion}";

		Motion += delta * GRAVITY;
		Motion = MoveAndSlide(Motion, Motion2D.FLOOR_NORMAL);
	}


	/// <summary>
	/// 
	/// </summary>
	public void Activate(QuestionBox.ContentType type) {
		_label.Visible = Logger.Level == Logger.LogLevel.TRACE;

		Active = true;
		Motion += INITIAL_IMPULSE;
	}


	/// <summary>
	/// 
	/// </summary>
	public override void _Ready() {
		this.SetupNodeBindings();
		_label.Visible = false;
	}


}
