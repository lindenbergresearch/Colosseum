using System;
using Godot;
using Newtonsoft.Json;
using Renoir;
using static DynamicStateCombiner;
using static Renoir.Logger;


/// <summary>
///     Main Player character
/// </summary>
public class Mario2D : Player2D, ICoinCollector {
	[GNode("AnimatedSprite")] private Godot.AnimatedSprite _animate;
	[GNode("BumpSound")] private AudioStreamPlayer _bumpSound;
	[GNode("Camera2D")] private Camera2D _camera;
	[GNode("InfoBox")] private RichTextLabel _info;
	[GNode("JumpSound")] private AudioStreamPlayer2D _jumpAudio;
	[GNode("OneLiveUp")] private AudioStreamPlayer _oneLiveUp;
	[GNode("SkiddingSound")] private AudioStreamPlayer2D _skiddingAudio;

	[Register("main.player.coins", "{0:D3}", "$main.playerinfo")]
	public Property<int> pCoins { get; set; }

	[Register("main.player.lives", "{0:D2}", "$main.playerinfo")]
	public Property<int> pLives { get; set; }

	[Register("main.player.score", "{0:D7}", "$main.playerinfo")]
	public Property<int> pScore { get; set; }


	private float CameraTime { get; set; }

	private bool Grounded => IsOnFloor();


	private bool Jumping => !Grounded && motion.MovingUp;
	private bool Falling => !Grounded && motion.MovingDown;
	private bool GameOver => pLives.Value == 0;
	private bool InMotion => motion.Abs.X > 0;

	private bool Walking { get; set; }
	private bool Running { get; set; }


	private bool SkiddingLeft => Grounded && motion.MovingRight && ActionKey.Left;
	private bool SkiddingRight => Grounded && motion.MovingLeft && ActionKey.Right;
	private bool Skidding => SkiddingLeft || SkiddingRight;


	private bool Debug { get; set; }


	private Vector2 StartPosition { get; set; }

	private PlayerParameter Parameter { get; set; }


	/// <summary>
	///     Handle coins
	/// </summary>
	/// <param name="coin"></param>
	/// <returns></returns>
	public bool onCoinCollect(Coin coin) {
		debug($"Collecting coin: {coin}");

		pCoins += 1;

		if (pCoins.Value == 15) {
			SetLives();
			pCoins.Value = 0;
		}

		return true;
	}


	/// <summary>
	///     Set players lives
	/// </summary>
	/// <param name="delta">The lives to add (neg. values will shrink lives)</param>
	public void SetLives(int delta = 1) {
		if (delta > 0) _oneLiveUp.Play();

		pLives += delta;
	}


	/// <summary>
	///     Check collisions and pass event to all coliders
	/// </summary>
	private void handleCollisions() {
		// exclude bottom collisions
		if (Grounded) return;

		for (var i = 0; i < GetSlideCount(); i++) {
			var coll = GetSlideCollision(i);

			var direction = "?";

			if (coll.Normal == Vector2.Down) direction = "TOP    ↑";

			if (coll.Normal == Vector2.Up) direction = "DOWN   ↓";

			if (coll.Normal == Vector2.Left) direction = "RIGHT ->";

			if (coll.Normal == Vector2.Right) direction = "<-  LEFT";

			debug($"Pos: {coll.Position} Vel: {coll.ColliderVelocity} Source: {coll.Collider} Normal: {coll.Normal} ({direction})");

			pScore += 123;

			switch (coll.Collider) {
				case TileMap _ when coll.Normal == Vector2.Down:
					_bumpSound.Play();
					continue;
				case ICollidable collider:
					collider.onCollide(coll);
					break;
			}
		}
	}


	/// <summary>
	/// </summary>
	private void updateStates() {
		if (ActionKey.Select) Debug = !Debug;


		Walking = Grounded && !Skidding && motion.Abs.X <= Parameter.MaxWalkingSpeed && motion.Abs.X > 0;
		Running = Grounded && !Skidding && motion.Abs.X > Parameter.MaxWalkingSpeed;
	}


	/// <summary>
	///     Apply gravity to the player
	/// </summary>
	/// <param name="delta"></param>
	private void applyGravity(float delta) {
		if (GlobalPosition.x <= 8 && motion.MovingLeft) {
			motion.reset();
			return;
		}

		motion += delta * Parameter.Gravity;
		motion = MoveAndSlide(motion, Motion2D.FLOOR_NORMAL);
	}


	/// <summary>
	///     Apply x motions to the player
	/// </summary>
	/// <param name="delta"></param>
	private void applyXMotion(float delta) {
		var speed = 0.0f;

		if (motion.X < Parameter.EpsilonVelocity && motion.X > -Parameter.EpsilonVelocity) motion.X = 0;

		if (!Skidding) {
			if (Walking) _animate.Animation = "Walk";
			else if (Running) _animate.Animation = "Run";

			if (ActionKey.Left && motion.X <= 0.0) {
				speed = -1;
				_animate.FlipH = true;
			}

			if (ActionKey.Right && motion.X >= 0.0) {
				speed = 1;
				_animate.FlipH = false;
			}

			speed *= ActionKey.Run ? Parameter.MaxRunningSpeed : Parameter.MaxWalkingSpeed;
			motion.X = Mathf.Lerp(motion.X, speed, Parameter.BodyWeightFactor);
		}

		if (Skidding) {
			_animate.Animation = "Skid";

			var v = motion.X;

			if (SkiddingLeft) {
				v -= Parameter.SkidDeceleration;
				if (v < 0) v = 0;
			}

			if (SkiddingRight) {
				v += Parameter.SkidDeceleration;
				if (v > 0) v = 0;
			}

			motion.X = v;

			if (!_skiddingAudio.Playing && Math.Abs(v) > Parameter.MaxWalkingSpeed)
				_skiddingAudio.Play();
		} else {
			if (_skiddingAudio.Playing)
				_skiddingAudio.Stop();
		}

		if (Grounded && ActionKey.Jump) {
			motion.Y = -(Parameter.JumpSpeed + Math.Abs(motion.X) * Parameter.JumpPushFactor);
			_jumpAudio.Play();
		}

		if (!Grounded && !Skidding) _animate.Animation = "Jump";


		if (!Skidding && !Walking && !Running && Grounded && !(ActionKey.Left || ActionKey.Right))
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
		} else {
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
	public override void PhysicsProcess(float delta) {
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
		debug("Reset player...");
		pLives.Value = 3;
		pCoins.Value = 0;
	}


	/// INIT *************************************************************************
	/// <summary>
	///     Init method
	/// </summary>
	public override void Ready() {
		this.SetupGlobalProperties();
		this.SetupNodeBindings();
		this.SetupNativeStates();

		debug("Setup player...");

		ResetPlayer();


		_info.Text = "!";

		_camera.LimitLeft = 0;
		_camera.LimitBottom = (int) Game.VIEWPORT_RESOLUTION.y;

		StartPosition = GlobalPosition;
	}


	public override void Draw() {
	}


	public override void Process(float delta) {
	}


	/// <summary>
	/// 
	/// </summary>
	public abstract class BasicParameter {
		/// <summary>
		/// Serialize parameters to JSON
		/// </summary>
		/// <param name="parameter">Parameter class instance</param>
		/// <returns></returns>
		public string Serialize()
			=> JsonConvert.SerializeObject(this);


		/// <summary>
		/// Deserialize parameters from JSON
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		public static BasicParameter DeSerialize(string json)
			=> JsonConvert.DeserializeObject<BasicParameter>(json);
	}


	/// <summary>
	/// Common player parameter for kinematic handling
	/// </summary>
	public class PlayerParameter : BasicParameter {
		//public   float MAX_JUMP_HEIGHT = 70.0f;
		public float JumpSpeed { get; set; } = 430f;
		public float JumpPushFactor { get; set; } = 0.20f;
		public float MaxWalkingSpeed { get; set; } = 110f;
		public float MaxWallPushSpeed { get; set; } = 50f;
		public float MaxRunningSpeed { get; set; } = 180f;
		public float XAccelerationFront { get; set; } = 400f;
		public float SlowingDeceleration { get; set; } = 392f;
		public float CrouchingDeceleration { get; set; } = 288f;
		public float SkidDeceleration { get; set; } = 8f;
		public float MoveOverSpeed { get; set; } = 48f;
		public float BodyWeightFactor { get; set; } = 0.1f;
		public float EpsilonVelocity { get; set; } = 5f;

		//TODO: should be moved to some subclass of level ...
		public Vector2 Gravity = new Vector2(0, 1200);
	}
}