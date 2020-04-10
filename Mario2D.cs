using System;
using Godot;
using Renoir;
using static Renoir.Logger;


/// <summary>
///     Main Player character
/// </summary>
public class Mario2D : Player2D, ICoinCollector {

	/// <summary>
	/// 
	/// </summary>
	private enum PowerStateEnum {
		SMALL,
		BIG,
		FIRE
	}


	[GNode("AnimatedSprite")] private Godot.AnimatedSprite _animate;
	[GNode("BumpSound")] private AudioStreamPlayer _bumpSound;
	[GNode("Camera2D")] private Camera2D _camera;
	[GNode("InfoBox")] private RichTextLabel _info;
	[GNode("JumpSound")] private AudioStreamPlayer2D _jumpAudio;
	[GNode("OneLiveUp")] private AudioStreamPlayer _oneLiveUp;
	[GNode("SkiddingSound")] private AudioStreamPlayer2D _skiddingAudio;

	[Register("main.player.coins", "$main.playerinfo", "{0:D3}")]
	public Property<int> pCoins { get; set; }

	[Register("main.player.lives", "$main.playerinfo", "{0:D2}")]
	public Property<int> pLives { get; set; }

	[Register("main.player.score", "$main.playerinfo", "{0:D7}")]
	public Property<int> pScore { get; set; }


	private bool Grounded => IsOnFloor();
	private bool Jumping => !Grounded && Motion.MovingUp;
	private bool Falling => !Grounded && Motion.MovingDown;
	private bool Walking => Grounded && !Skidding && Motion.MovingHorizontal && Motion.X.Abs() <= player.MaxWalkingSpeed;
	private bool Running => Grounded && !Skidding && Motion.X.Abs() > player.MaxWalkingSpeed;
	private bool SkiddingLeft => Grounded && Motion.MovingRight && ActionKey.Left;
	private bool SkiddingRight => Grounded && Motion.MovingLeft && ActionKey.Right;
	private bool Skidding => SkiddingLeft || SkiddingRight;
	private bool Idle => Grounded && !Walking && !Running && !Jumping && !Falling && !Skidding;
	private bool TurnLeft => ActionKey.Left && Motion.X <= 0.0;
	private bool TurnRight => ActionKey.Right && Motion.X >= 0.0;
	private bool AboutToJump => Grounded && ActionKey.Jump;


	private bool Debug { get; set; }
	private Vector2 StartPosition { get; set; }
	private float CameraTime { get; set; }

	private PowerStateEnum PowerState { get; set; } = PowerStateEnum.SMALL;
	private PlayerParameter player = new PlayerParameter();


	/// <summary>
	///     Handle coins
	/// </summary>
	/// <param name="coin"></param>
	/// <returns></returns>
	public bool onCoinCollect(Coin coin) {
		debug($"Collecting coin: {coin}");

		pCoins += 1;
		pScore += 250;

		if (pCoins.Value == 100) {
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
	protected override void UpdateCollisions(float delta) {
		foreach (var coll in GetCollider()) {
			var direction = "?";
			if (coll.Normal == Vector2.Down) direction = "↑";
			if (coll.Normal == Vector2.Up) direction = "↓";
			if (coll.Normal == Vector2.Left) direction = "→";
			if (coll.Normal == Vector2.Right) direction = "←";

			/* match collider type **/
			switch (coll.Collider) {
				case TileMap _ when coll.Normal.y == 1:
					_bumpSound.Play();
					continue;
				case ICollidable collider:
					trace($"position={coll.Position} velocity={coll.ColliderVelocity} collider={coll.Collider} vector={coll.Normal} {direction}");
					collider.onCollide(coll);
					break;
			}
		}
	}


	/// <summary>
	///     Apply x motions to the player
	/// </summary>
	/// <param name="delta"></param>
	override protected void UpdateMotion(float delta) {
		Motion += delta * player.Gravity;
		Motion = MoveAndSlide(Motion, Motion2D.FLOOR_NORMAL);

		/* if x velocity is small enough round it to zero to avoid small drifting */
		if (Motion.X.InsideRange(player.EpsilonVelocity, false)) Motion.X = 0;

		if (Idle || Walking || Running) {
			var v = 0.0f;

			if (TurnLeft) v = -1;
			if (TurnRight) v = 1;

			v *= ActionKey.Run ? player.MaxRunningSpeed : player.MaxWalkingSpeed;
			Motion.X = Mathf.Lerp(Motion.X, v, player.BodyWeightFactor);
		}

		if (Skidding) {
			var v = Motion.X;

			if (SkiddingLeft) {
				v -= player.SkidDeceleration;
				if (v < 0) v = 0;
			}

			if (SkiddingRight) {
				v += player.SkidDeceleration;
				if (v > 0) v = 0;
			}

			Motion.X = v;
		}

		if (AboutToJump) {
			Motion.Y = -(player.JumpSpeed + Motion.X.Abs() * player.JumpPushFactor);
		}
	}


	/// <summary>
	/// Update player animations
	/// </summary>
	override protected void UpdateAnimation(float delta) {
		/* set animation depending on the current state */
		if (Idle) _animate.Animation = "Idle";
		if (Walking) _animate.Animation = "Walk";
		if (Running) _animate.Animation = "Run";
		if (Jumping || Falling) _animate.Animation = "Jump";
		if (Skidding) _animate.Animation = "Skid";

		/* set correct sprite direction */
		if (TurnLeft) _animate.FlipH = true;
		if (TurnRight) _animate.FlipH = false;

		/* start animation */
		_animate.Play();
	}


	/// <summary>
	/// Update sound fx
	/// </summary>
	override protected void UpdateAudio(float delta) {
		if (Skidding && !_skiddingAudio.Playing && Running) _skiddingAudio.Play();
		else if (_skiddingAudio.Playing) _skiddingAudio.Stop();

		if (AboutToJump) _jumpAudio.Play();
	}


	private void printDebug() {
		var vect = string.Format("V = {0,6:000.0}, {1,6:000.0}", Motion.X, Motion.Y);
		var pos = string.Format("P = {0,6:000.0}, {1,6:000.0}", GlobalPosition.x, GlobalPosition.y);
		_info.Text =
			$"Velocity: {vect}\nPosition: {pos}\nSL: {SkiddingLeft} SR: {SkiddingRight}\nGrounded: {Grounded}\nWalk: {Walking} Run: {Running}\nJump: {Jumping} Fall: {Falling}";
	}


	private void UpdateCamera(float delta) {
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
	override protected void PhysicsProcess(float delta) {
		if (ActionKey.Select) Debug = !Debug;

		//updateCamera(delta);

		printDebug();
		_info.Visible = Debug;

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
	override public void Ready() {
		debug("Initialize Player");

		ResetPlayer();

		//Parameter.SaveJson("PlayerParameter.json");
		player = BasicParameter.LoadFromJson<PlayerParameter>("PlayerParameter.json");


		_info.Text = "!";

		_camera.LimitLeft = 0;
		_camera.LimitBottom = (int) Game.VIEWPORT_RESOLUTION.y;

		StartPosition = GlobalPosition;
	}


	override protected void Draw() {
	}


	override protected void Process(float delta) {
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
