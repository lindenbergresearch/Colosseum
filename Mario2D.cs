using System;
using Godot;
using Renoir;
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
	private PlayerParameter player = new PlayerParameter();

	[Register("main.player.coins", "$main.playerinfo", "{0:D3}")]
	public Property<int> pCoins { get; set; }

	[Register("main.player.lives", "$main.playerinfo", "{0:D2}")]
	public Property<int> pLives { get; set; }

	[Register("main.player.score", "$main.playerinfo", "{0:D7}")]
	public Property<int> pScore { get; set; }


	private bool Grounded => IsOnFloor();
	private bool Jumping => !Grounded && motion.MovingUp;
	private bool Falling => !Grounded && motion.MovingDown;
	private bool Walking => Grounded && !Skidding && motion.MovingHorizontal && motion.Abs.X <= player.MaxWalkingSpeed;
	private bool Running => Grounded && !Skidding && motion.Abs.X > player.MaxWalkingSpeed;
	private bool SkiddingLeft => Grounded && motion.MovingRight && ActionKey.Left;
	private bool SkiddingRight => Grounded && motion.MovingLeft && ActionKey.Right;
	private bool Skidding => SkiddingLeft || SkiddingRight;


	private bool Debug { get; set; }
	private Vector2 StartPosition { get; set; }
	private float CameraTime { get; set; }


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
	///     Check collisions and pass event to all collider
	/// </summary>
	private void handleCollisions() {
		foreach (var coll in GetCollider()) {
			var direction = "?";
			if (coll.Normal == Vector2.Down) direction = "↑";
			if (coll.Normal == Vector2.Up) direction = "↓";
			if (coll.Normal == Vector2.Left) direction = "→";
			if (coll.Normal == Vector2.Right) direction = "←";

			/* match collider type **/
			switch (coll.Collider) {
				case TileMap _ when coll.Normal == Vector2.Down:
					_bumpSound.Play();
					continue;
				case ICollidable collider:
					debug($"position={coll.Position} velocity={coll.ColliderVelocity} collider={coll.Collider} vector={coll.Normal} {direction}");
					collider.onCollide(coll);
					break;
			}
		}
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

		motion += delta * player.Gravity;
		motion = MoveAndSlide(motion, Motion2D.FLOOR_NORMAL);
	}


	/// <summary>
	///     Apply x motions to the player
	/// </summary>
	/// <param name="delta"></param>
	private void applyXMotion(float delta) {
		var speed = 0.0f;

		if (motion.X < player.EpsilonVelocity && motion.X > -player.EpsilonVelocity) motion.X = 0;

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

			speed *= ActionKey.Run ? player.MaxRunningSpeed : player.MaxWalkingSpeed;
			motion.X = Mathf.Lerp(motion.X, speed, player.BodyWeightFactor);
		}

		if (Skidding) {
			_animate.Animation = "Skid";

			var v = motion.X;

			if (SkiddingLeft) {
				v -= player.SkidDeceleration;
				if (v < 0) v = 0;
			}

			if (SkiddingRight) {
				v += player.SkidDeceleration;
				if (v > 0) v = 0;
			}

			motion.X = v;

			if (!_skiddingAudio.Playing && Math.Abs(v) > player.MaxWalkingSpeed)
				_skiddingAudio.Play();
		} else {
			if (_skiddingAudio.Playing)
				_skiddingAudio.Stop();
		}

		if (Grounded && ActionKey.Jump) {
			motion.Y = -(player.JumpSpeed + Math.Abs(motion.X) * player.JumpPushFactor);
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


	/// <summary>
	///     Update Player
	/// </summary>
	/// <param name="delta"></param>
	public override void PhysicsProcess(float delta) {
		if (ActionKey.Select) Debug = !Debug;

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


	/// <summary>
	///     Init method
	/// </summary>
	public override void Ready() {
		this.SetupGlobalProperties();
		this.SetupNodeBindings();

		debug("Setup player...");

		ResetPlayer();


		//Parameter.SaveJson("PlayerParameter.json");
		player = BasicParameter.LoadFromJson<PlayerParameter>("PlayerParameter.json");


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
	/// Common player parameter for kinematic handling
	/// </summary>
	public class PlayerParameter : BasicParameter {

		//TODO: should be moved to some subclass of level ...
		public Vector2 Gravity = new Vector2(0, 1200);

		//public   float MAX_JUMP_HEIGHT = 70.0f;
		public float JumpSpeed { get; set; }
		public float JumpPushFactor { get; set; }
		public float MaxWalkingSpeed { get; set; }
		public float MaxWallPushSpeed { get; set; }
		public float MaxRunningSpeed { get; set; }
		public float XAccelerationFront { get; set; }
		public float SlowingDeceleration { get; set; }
		public float CrouchingDeceleration { get; set; }
		public float SkidDeceleration { get; set; }
		public float MoveOverSpeed { get; set; }
		public float BodyWeightFactor { get; set; }
		public float EpsilonVelocity { get; set; }
	}
}
