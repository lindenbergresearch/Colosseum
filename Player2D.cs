using System.Collections.Generic;
using Godot;
using static Godot.Input;

namespace Renoir {

	/// <summary>
	///     Abstract Player blueprint
	/// </summary>
	public abstract class Player2D : KinematicBody2D {

		/// <summary>
		///     Motion vector
		/// </summary>
		public static Motion2D Motion { get; set; } = (0, 0);


		/// <summary>
		///     Routing function for Draw event.
		/// </summary>
		protected abstract void Draw();


		/// <summary>
		///     Routing function for PhysicsProcess event.
		/// </summary>
		/// <param name="delta">Delta time in seconds</param>
		protected abstract void PhysicsProcess(float delta);


		/// <summary>
		///     Routing function for Process event.
		/// </summary>
		/// <param name="delta">Delta time in seconds</param>
		protected abstract void Process(float delta);


		/// <summary>
		/// 
		/// </summary>
		/// <param name="delta"></param>
		protected abstract void UpdateMotion(float delta);


		/// <summary>
		/// 
		/// </summary>
		/// <param name="delta"></param>
		protected abstract void UpdateAnimation(float delta);


		/// <summary>
		/// 
		/// </summary>
		/// <param name="delta"></param>
		protected abstract void UpdateAudio(float delta);


		/// <summary>
		/// 
		/// </summary>
		/// <param name="delta"></param>
		protected abstract void UpdateCollisions(float delta);


		/// <summary>
		///     Routing function Ready event.
		/// </summary>
		public abstract void Ready();


		/// <summary>
		/// </summary>
		public sealed override void _Draw() {
			Draw();
		}


		/// <summary>
		/// </summary>
		/// <param name="delta"></param>
		public sealed override void _PhysicsProcess(float delta) {
			/* call standard update handler */
			UpdateMotion(delta);
			UpdateAnimation(delta);
			UpdateAudio(delta);
			UpdateCollisions(delta);

			/* call custom handler */
			PhysicsProcess(delta);
		}


		/// <summary>
		/// </summary>
		/// <param name="delta"></param>
		public sealed override void _Process(float delta) {
			Process(delta);
		}


		/// <summary>
		/// </summary>
		public sealed override void _Ready() {
			this.SetupGlobalProperties();
			this.SetupNodeBindings();

			Ready();
		}


		/// <summary>
		///     Map action keys
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
