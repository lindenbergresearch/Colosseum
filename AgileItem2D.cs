using Godot;
using System;
using System.Collections.Generic;
using Renoir;


/// <summary>
/// 
/// </summary>
public class AgileItem2D : KinematicBody2D, ICollidable {
	private static Vector2 GRAVITY = new Vector2(0, 900);
	private static Vector2 INITIAL_IMPULSE = new Vector2(50, -250);

	private Motion2D Motion { get; set; } = (0, 0);

	public bool Active { get; set; } = false;


	[GNode("Label")] private Label _label;


	


	/// <summary>
	/// 
	/// </summary>
	/// <param name="collision"></param>
	public void onCollide(KinematicCollision2D collision) {
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="delta"></param>
	public override void _PhysicsProcess(float delta) {
		if (!Active) return;


		if (!Visible) Visible = true;

		foreach (var collision2D in this.GetCollider()) {
			if (collision2D.Normal != Motion2D.FLOOR_NORMAL) {
				Logger.debug($"position={collision2D.Position} velocity={collision2D.ColliderVelocity} collider={collision2D.Collider} vector={collision2D.Normal}");
				Motion *= collision2D.Normal;
			}
		}

		_label.Text = $"Motion= {Motion}";

		Motion += delta * GRAVITY;
		Motion = MoveAndSlide(Motion, Motion2D.FLOOR_NORMAL);
	}


	/// <summary>
	/// 
	/// </summary>
	public void Activate() {
		_label.Visible = true;
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
