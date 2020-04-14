using System.Linq;
using Godot;
using Renoir;


/// <summary>
/// Basic consumable item
/// </summary>
public class AgileItem2D : ConsumableItem2D {
	private static readonly Vector2 GRAVITY = new Vector2(0, 900);
	private static readonly Vector2 INITIAL_IMPULSE = new Vector2(50, -250);

	private Motion2D Motion { get; set; } = (0, 0);
	private Motion2D Save { get; set; } = (0, 0);

	[GNode("Label")] private Label _label;


	/// <summary>
	/// 
	/// </summary>
	/// <param name="delta"></param>
	public override void PhysicsProcess(float delta) {
		var collider = this.GetCollider();

		// if (!collider.Empty()) {
		// 	var collision2D = collider.First();
		// 	Motion = Save * -collision2D.Normal.Abs();
		// }


		foreach (var collision2D in collider) {
			if (!collision2D.Top())
				Motion = Save * -collision2D.Normal.Abs();
		}

		Save = Motion.Velocity;

		_label.Text = $"Motion={Motion}";

		Motion += delta * GRAVITY;
		Motion = MoveAndSlide(Motion, Motion2D.FLOOR_NORMAL);
	}


	/// <summary>
	/// Activate item
	/// </summary>
	public override void Activate() {
		base.Activate();

		_label.Visible = Logger.Level == Logger.LogLevel.TRACE;
		Motion += INITIAL_IMPULSE;

		Logger.debug($"Mo: {Motion}");
	}


	/// <summary>
	/// 
	/// </summary>
	public override void Ready() {
		_label.Visible = false;
	}


}
