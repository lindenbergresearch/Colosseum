using System;
using Colosseum;
using Godot;
using static Godot.Input;
using static DynamicStateCombiner;


/// <summary>
/// </summary>
public class Mario2D : KinematicBody2D, ICoinCollector {
	public static Motion2D motion = new Vector2(0, 0);

	private readonly Property<int>
		pCoins = PropertyPool.RegisterNewProperty("main.player.coins", 0, "$main.playerinfo");

	private readonly Property<int>
		pLives = PropertyPool.RegisterNewProperty("main.player.lives", 0, "$main.playerinfo");


	/** PROPERTIES *********************************************************************/
	private readonly Property<int>
		pScore = PropertyPool.RegisterNewProperty("main.player.score", 0, "$main.playerinfo");

	[GNode("AnimatedSprite")]
	private Godot.AnimatedSprite _animate;

	[GNode("BumpSound")]
	private AudioStreamPlayer _bumpSound;

	[GNode("Camera2D")]
	private Camera2D _camera;

	[GNode("InfoBox")]
	private RichTextLabel _info;

	[GNode("JumpSound")]
	private AudioStreamPlayer2D _jumpAudio;

	[GNode("OneLiveUp")]
	private AudioStreamPlayer _oneLiveUp;

	[GNode("SkiddingSound")]
	private AudioStreamPlayer2D _skiddingAudio;


	private float CameraTime { get; set; }

	/** PROPERTIES *********************************************************************/
	/** FLAGS **************************************************************************/
	private bool Grounded { get; set; }

	private DynamicStateCombiner Jumping { get; set; }
	private DynamicStateCombiner Falling { get; set; }
	private DynamicStateCombiner IsDead { get; set; }
	private bool Walking { get; set; }
	private bool Running { get; set; }
	private bool SkiddingLeft { get; set; }
	private bool SkiddingRight { get; set; }
	private bool Skidding { get; set; }

	private bool Debug { get; set; }

	private Vector2 StartPosition { get; set; }


	/// <summary>
	///     Handle coins
	/// </summary>
	/// <param name="coin"></param>
	/// <returns></returns>
	public bool onCoinCollect(Coin coin) {
		Logger.debug($"Collecting coin: {coin}");

		pCoins.Value += 1;

		if (pCoins.Value == 15) {
			SetLives();
			pCoins.Value = 0;
		}

		return true;
	}


	/// UPDATES ***********************************************************************
	/// <summary>
	/// </summary>
	private void updateKeys() {
		ActionKey.UP = IsActionPressed("ui_up");
		ActionKey.DOWN = IsActionPressed("ui_down");
		ActionKey.LEFT = IsActionPressed("ui_left");
		ActionKey.RIGHT = IsActionPressed("ui_right");

		ActionKey.RUN = IsActionPressed("ui_accept");
		ActionKey.JUMP = IsActionJustPressed("ui_cancel");

		ActionKey.SELECT = IsActionJustPressed("ui_select");
	}


	/// <summary>
	///     Set players lives
	/// </summary>
	/// <param name="delta">The lives to add (neg. values will shrink lives)</param>
	public void SetLives(int delta = 1) {
		if (delta > 0) _oneLiveUp.Play();

		pLives.Value += delta;
	}


	/// <summary>
	///     Check collisions and pass event to all coliders
	/// </summary>
	private void handleCollisions() {
		// exclude bottom collisions
		if (Grounded) return;

		for (var i = 0; i < GetSlideCount(); i++) {
			var coll = GetSlideCollision(i);

			var directon = "?";

			if (coll.Normal == Vector2.Down) directon = "TOP---^";

			if (coll.Normal == Vector2.Up) directon = "DOWN--v";

			if (coll.Normal == Vector2.Left) directon = "RIGHT -->";

			if (coll.Normal == Vector2.Right) directon = "<- LEFT";

			Logger.debug(
				$"Pos: {coll.Position} Vel: {coll.ColliderVelocity} Source: {coll.Collider} Normal: {coll.Normal} ({directon})");

			pScore.Value = pScore.Value + 123;

			if (coll.Collider is TileMap && coll.Normal == Vector2.Down) {
				_bumpSound.Play();
				continue;
			}


			if (coll.Collider is ICollidable collider) collider.onCollide(coll);
		}
	}


	/// <summary>
	/// </summary>
	private void updateStates() {
		if (ActionKey.SELECT) Debug = !Debug;

		Grounded = IsOnFloor();

		SkiddingLeft = Grounded && motion.right() && ActionKey.LEFT;
		SkiddingRight = Grounded && motion.left() && ActionKey.RIGHT;
		Skidding = SkiddingLeft || SkiddingRight;

		var absv = motion.Abs.X;

		Walking = Grounded && !Skidding && absv <= Parameter.MAX_WALKING_SPEED && absv > 0;
		Running = Grounded && !Skidding && motion.Abs.X > Parameter.MAX_WALKING_SPEED;
	}


	/// <summary>
	///     Apply gravity to the player
	/// </summary>
	/// <param name="delta"></param>
	private void applyGravity(float delta) {
		if (GlobalPosition.x <= 8 && motion.left()) {
			motion.reset();
			return;
		}

		motion += delta * Parameter.GRAVITY;
		motion = MoveAndSlide(motion, Parameter.FLOOR_NORMAL);
	}


	/// <summary>
	///     Apply x motions to the player
	/// </summary>
	/// <param name="delta"></param>
	private void applyXMotion(float delta) {
		var speed = 0.0f;

		if (motion.X < Parameter.EPSILON_VELOCITY && motion.X > -Parameter.EPSILON_VELOCITY) motion.X = 0;

		if (!Skidding) {
			if (Walking)
				_animate.Animation = "Walk";
			else if (Running) _animate.Animation = "Run";

			if (ActionKey.LEFT && motion.X <= 0.0) {
				speed = -1;
				_animate.FlipH = true;
			}

			if (ActionKey.RIGHT && motion.X >= 0.0) {
				speed = 1;
				_animate.FlipH = false;
			}

			speed *= ActionKey.RUN ? Parameter.MAX_RUNNING_SPEED : Parameter.MAX_WALKING_SPEED;
			motion.X = Mathf.Lerp(motion.X, speed, Parameter.BODY_WEIGHT_FACTOR);
		}

		if (Skidding) {
			_animate.Animation = "Skid";

			var v = motion.X;

			if (SkiddingLeft) {
				v -= Parameter.SKID_DECELERATION;
				if (v < 0) v = 0;
			}

			if (SkiddingRight) {
				v += Parameter.SKID_DECELERATION;
				if (v > 0) v = 0;
			}

			motion.X = v;

			if (!_skiddingAudio.Playing && Math.Abs(v) > Parameter.MAX_WALKING_SPEED)
				_skiddingAudio.Play();
		}
		else {
			if (_skiddingAudio.Playing)
				_skiddingAudio.Stop();
		}

		if (Grounded && ActionKey.JUMP) {
			motion.Y = -(Parameter.JUMP_SPEED + Math.Abs(motion.X) * Parameter.JUMP_PUSH_FACTOR);
			_jumpAudio.Play();
		}

		if (!Grounded && !Skidding) _animate.Animation = "Jump";


		if (!Skidding && !Walking && !Running && Grounded && !(ActionKey.LEFT || ActionKey.RIGHT))
			_animate.Animation = "Idle";

		_animate.Play();

		printDebug();
		_info.Visible = Debug;
	}


	private void printDebug() {
		var vect = string.Format("V = {0,6:000.0}, {1,6:000.0}", motion.X, motion.Y);
		var pos = string.Format("P = {0,6:000.0}, {1,6:000.0}", GlobalPosition.x, GlobalPosition.y);
		_info.Text =
			$"Velocity: {vect}\nPosition: {pos}\nSL: {SkiddingLeft} SR: {SkiddingRight}\nGrounded: {Grounded}\nWalk: {Walking} Run: {Running}\nJump: {Jumping} Fall: {Falling}";
	}


	private void updateCamera(float delta) {
		CameraTime += delta;

		if (GlobalPosition.y >= (int) Game.VIEWPORT_RESOLUTION.y) {
			CameraTime = 0;
			_camera.LimitBottom = 1000000;
		}
		else {
			if (CameraTime >= 2 && _camera.LimitBottom != (int) Game.VIEWPORT_RESOLUTION.y) {
				if (_camera.LimitBottom == 1000000) _camera.LimitBottom = (int) (GlobalPosition.y * 2.0f);

				_camera.LimitBottom = (int) Mathf.Lerp(_camera.LimitBottom, Game.VIEWPORT_RESOLUTION.y, 0.01f);
			}
		}
	}


	/// MAIN *************************************************************************
	/// <summary>
	///     Update Player
	/// </summary>
	/// <param name="delta"></param>
	public override void _PhysicsProcess(float delta) {
		updateKeys();
		updateStates();

		applyGravity(delta);
		applyXMotion(delta);

		handleCollisions();

		//updateCamera(delta);

		if (GlobalPosition.y > 1000) GlobalPosition = StartPosition;
	}


	/// <summary>
	///     Resets all player properties to initial values
	/// </summary>
	public void ResetPlayer() {
		Logger.debug("Reset player...");
		pLives.Value = 3;
		pCoins.Value = 0;
	}


	/// INIT *************************************************************************
	/// <summary>
	///     Init method
	/// </summary>
	public override void _Ready() {
		this.SetupNodeBindings();

		Logger.debug("Setup player...");

		ResetPlayer();

		pCoins.Format = "{0:D3}";
		pScore.Format = "{0:D7}";
		pLives.Format = "{0:D2}";

		Jumping = fun(() => !Grounded && motion.upward());
		Falling = fun(() => !Grounded && motion.downward());
		IsDead = fun(() => pLives.Value == 0);


		_info.Text = "!";

		_camera.LimitLeft = 0;
		_camera.LimitBottom = (int) Game.VIEWPORT_RESOLUTION.y;

		StartPosition = GlobalPosition;
	}


	/// <summary>
	///     Type which holds pressed keys
	/// </summary>
	private static class ActionKey {
		public static bool UP { get; set; }
		public static bool DOWN { get; set; }
		public static bool LEFT { get; set; }
		public static bool RIGHT { get; set; }
		public static bool RUN { get; set; }
		public static bool JUMP { get; set; }
		public static bool SELECT { get; set; }
	}

	/// <summary>
	///     Common player parameter for kinematic handling
	/// </summary>
	private static class Parameter {
		/*** CONSTANTS *************************************************************/
		public static readonly float MAX_JUMP_HEIGHT = 70.0f;
		public static readonly float JUMP_SPEED = 430.0f;
		public static readonly float JUMP_PUSH_FACTOR = 0.20f;
		public static readonly float MAX_WALKING_SPEED = 110.0f;
		public static readonly float MAX_WALL_PUSH_SPEED = MAX_WALKING_SPEED / 2.0f;
		public static readonly float MAX_RUNNING_SPEED = 180.0f;
		public static readonly float X_ACCELERATION_FRONT = 400.0f;
		public static readonly float SLOWING_DECELERATION = 392.0f;
		public static readonly float CROUCHING_DECELERATION = 288.0f;
		public static readonly float SKID_DECELERATION = 8.0f;
		public static readonly float MOVE_OVER_SPEED = 48.0f;
		public static readonly float BODY_WEIGHT_FACTOR = 0.1f;
		public static readonly float EPSILON_VELOCITY = 5.0f;

		/*** CONSTANTS *************************************************************/
		public static readonly Vector2 GRAVITY = new Vector2(0, 1200);

		public static readonly Vector2 FLOOR_NORMAL = new Vector2(0, -1);
		/*** CURRENTS **************************************************************/
	}
}
