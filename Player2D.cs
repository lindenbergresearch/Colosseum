using Godot;
using static Godot.Input;


/// <summary>
/// 
/// </summary>
public abstract class Player2D : KinematicBody2D {

	/// <summary>
	/// Motion vector
	/// </summary>
	public static Motion2D motion = new Vector2(0, 0);


	/// <summary>
	/// Map action keys
	/// </summary>
	protected static class ActionKey {

		public static bool Up {
			get => IsActionPressed("ui_up");
		}

		public static bool Down {
			get => IsActionPressed("ui_down");
		}

		public static bool Left {
			get => IsActionPressed("ui_left");
		}

		public static bool Right {
			get => IsActionPressed("ui_right");
		}

		public static bool Run {
			get => IsActionPressed("ui_accept");
		}

		public static bool Jump {
			get => IsActionPressed("ui_cancel");
		}

		public static bool Select {
			get => IsActionPressed("ui_select");
		}

	}


	/// <summary>
	/// Routing function for Draw event.
	/// </summary>
	public abstract void Draw();


	/// <summary>
	/// Routing function for PhysicsProcess event.
	/// </summary>
	/// <param name="delta">Delta time in seconds</param>
	public abstract void PhysicsProcess(float delta);


	/// <summary>
	/// Routing function for Process event.
	/// </summary>
	/// <param name="delta">Delta time in seconds</param>
	public abstract void Process(float delta);


	/// <summary>
	/// Routing function Ready event.
	/// </summary>
	public abstract void Ready();


	/// <summary>
	/// 
	/// </summary>
	public override void _Draw() {
		Draw();
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="delta"></param>
	public override void _PhysicsProcess(float delta) {
		PhysicsProcess(delta);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="delta"></param>
	public override void _Process(float delta) {
		Process(delta);
	}


	/// <summary>
	/// 
	/// </summary>
	public override void _Ready() {
		Ready();
	}

}