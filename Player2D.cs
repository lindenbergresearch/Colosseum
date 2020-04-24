using Godot;
using static Godot.Input;

namespace Renoir {

	/// <summary>
	/// Abstract Player blueprint
	/// </summary>
	public abstract class Player2D : KinematicEntity2D {


		/// <summary>
		/// </summary>
		/// <param name="delta"></param>
		protected abstract void UpdateMotion(float delta);


		/// <summary>
		/// </summary>
		/// <param name="delta"></param>
		protected abstract void UpdateAnimation(float delta);


		/// <summary>
		/// </summary>
		/// <param name="delta"></param>
		protected abstract void UpdateAudio(float delta);


		/// <summary>
		/// </summary>
		/// <param name="delta"></param>
		protected abstract void UpdateCollisions(float delta);


		/// <summary>
		/// </summary>
		/// <param name="delta"></param>
		public override void _PhysicsProcess(float delta) {
			/* call standard update handler */
			UpdateMotion(delta);
			UpdateAnimation(delta);
			UpdateAudio(delta);
			UpdateCollisions(delta);

			PhysicsProcess(delta);
		}


		/// <summary>
		/// Map action keys
		/// </summary>
		protected static class ActionKey {
			public static bool Up => IsActionPressed("ui_up");
			public static bool Down => IsActionPressed("ui_down");
			public static bool Left => IsActionPressed("ui_left");
			public static bool Right => IsActionPressed("ui_right");
			public static bool Run => IsActionPressed("ui_accept");
			public static bool Jump => IsActionJustPressed("ui_cancel"); //IsActionPressed("ui_cancel");
			public static bool Select => IsActionJustPressed("ui_select");
		}
	}

}
