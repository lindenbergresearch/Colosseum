#region header

// 
//    _____
//   (, /   )            ,
//     /__ /  _ __   ___   __
//  ) /   \__(/_/ (_(_)_(_/ (_  CORE LIBRARY
// (_/ ______________________________________/
// 
// 
// Renoir Core Library for the Godot Game-Engine.
// Copyright 2020-2022 by Lindenberg Research.
// 
// www.lindenberg-research.com
// www.godotengine.org
// 

#endregion

#region

using Godot;
using Renoir;
using static Renoir.Logger;
using static Renoir.Util;

#endregion


/// <summary>
///     Main Player character
/// </summary>
public class Mario2D : KinematicEntity2D, ICoinConsumer {
	/// <summary>
	/// 
	/// </summary>
	/// <param name="payload"></param>
	/// <param name="item"></param>
	/// <returns></returns>
	public bool DoCoinConsume(int payload, Object item) {
		debug($"consuming a coin with value: {payload}");

		CollectedCoins += payload;
		TotalScore += 250 * payload;

		if (CollectedCoins.Value >= 100) {
			SetLives();
			CollectedCoins.Value -= 100;
		}

		return true;
	}


	/// <summary>
	///     Set players lives
	/// </summary>
	/// <param name="delta">The lives to add (neg. values will shrink lives)</param>
	public void SetLives(int delta = 1) {
		if (delta > 0) _oneLiveUp.Play();

		LivesLeft += delta;
	}

	/// <summary>
	/// </summary>
	/// <param name="collision2D"></param>
	public override void HandleCollision(KinematicCollision2D collision2D) {
		switch (collision2D.Collider) {
			case TileMap when collision2D.Bottom():
				_bumpSound.Play();
				break;
			case ICollider collider:
				trace(DebugCollision(collision2D));
				collider.OnCollide(this, collision2D);
				break;
		}
	}


	/// <summary>
	///     Apply x motions to the player
	/// </summary>
	/// <param name="delta"></param>
	protected override void UpdateMotion(float delta) {
		var _gravity = Level.Gravity.Value * delta;

		Motion += _gravity;
		Motion = MoveAndSlide(Motion, Motion2D.FLOOR_NORMAL);

		/* if x velocity is small enough round it to zero to avoid small drifting */
		if (Motion.X.InsideRange(player.EpsilonVelocity, false)) Motion.X = 0;

		if (Idle || Walking || Running) {
			var v = 0.0f;

			if (TurnLeft) v = -1;
			if (TurnRight) v = 1;

			v *= ActionKey.Run
				? player.MaxRunningSpeed
				: player.MaxWalkingSpeed;

			Motion.X = Mathf.Lerp(Motion.X, v, player.BodyWeightFactor);
		}

		if (Jumping || Falling) {
			var v = 0.0f;

			if (TurnLeft) v = -1;
			if (TurnRight) v = 1;

			v *= player.MaxWalkingSpeed;
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

		if (AboutToJump) Motion.Y = -(player.JumpSpeed + Motion.X.Abs() * player.JumpPushFactor);
	}


	/// <summary>
	///     Update player animations
	/// </summary>
	protected override void UpdateAnimation(float delta) {
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
		if (!Idle && !_animate.IsPlaying()) _animate.Play();
	}


	/// <summary>
	///     Update sound fx
	/// </summary>
	protected override void UpdateAudio(float delta) {
		if (Skidding && !_skiddingAudio.Playing && Motion.X > player.MaxWalkingSpeed * 0.8) _skiddingAudio.Play();
		if (AboutToJump) _jumpAudio.Play();
	}


	private void printDebug() {
		var vect = $"V = {Motion.X,6:000.0}, {Motion.Y,6:000.0}";
		var pos = $"P = {GlobalPosition.x,6:000.0}, {GlobalPosition.y,6:000.0}";
		_info.Text =
			$"Velocity: {vect}\nPosition: {pos}\nSL: {SkiddingLeft} " +
			$"SR: {SkiddingRight}\nGrounded: {Grounded}\nWalk: {Walking} " +
			$"Run: {Running}\nJump: {Jumping} Fall: {Falling}\n" +
			$"Animation: {_animate.IsPlaying()}";
	}


	private void UpdateCamera(float delta) {
		CameraTime += delta;

		if (GlobalPosition.y >= (int) Game.VIEWPORT_RESOLUTION.y) {
			CameraTime = 0;
			camera.LimitBottom = 1000000;
		} else {
			if (CameraTime >= 2 && camera.LimitBottom != (int) Game.VIEWPORT_RESOLUTION.y) {
				if (camera.LimitBottom == 1000000) camera.LimitBottom = (int) (GlobalPosition.y * 2.0f);

				camera.LimitBottom = (int) Mathf.Lerp(camera.LimitBottom, Game.VIEWPORT_RESOLUTION.y, 0.01f);
			}
		}
	}


	/// <summary>
	///     Update Player
	/// </summary>
	/// <param name="delta"></param>
	protected override void PhysicsProcess(float delta) {
		if (ActionKey.Select) Debug = !Debug;

		//updateCamera(delta);

		printDebug();
		_info.Visible = Debug;

		if (GlobalPosition.y > 1000) GlobalPosition = StartPosition;
	}


	/// <summary>
	///     Resets all player properties to initial values
	/// </summary>
	public override void Reset() {
		debug("Reset player...");
		LivesLeft.Update(3);
		CollectedCoins.Update(0);
	}


	/// <summary>
	///     Init method
	/// </summary>
	protected override void Ready() {
		debug("Initialize Player");

		Reset();

		camera.LimitLeft = 0;
		camera.LimitBottom = (int) Game.VIEWPORT_RESOLUTION.y;
	}


	/// <summary>
	///     Common player parameter for kinematic handling
	/// </summary>
	public class PlayerParameter {
		public float JumpSpeed { get; set; } = 430.0f;
		public float JumpPushFactor { get; set; } = 0.2f;
		public float MaxWalkingSpeed { get; set; } = 110.0f;
		public float MaxWallPushSpeed { get; set; } = 150.0f;
		public float MaxRunningSpeed { get; set; } = 180.0f;
		public float XAccelerationFront { get; set; } = 400.0f;
		public float SlowingDeceleration { get; set; } = 392.0f;
		public float CrouchingDeceleration { get; set; } = 288.0f;
		public float SkidDeceleration { get; set; } = 8.0f;
		public float MoveOverSpeed { get; set; } = 48.0f;
		public float BodyWeightFactor { get; set; } = 0.1f;
		public float EpsilonVelocity { get; set; } = 5.0f;
	}


	// ----------------------------------------------------------------------------------------- \\

	#region Godot Node Bindings

	[GNode("AnimatedSprite")]
	private AnimatedSprite _animate;

	[GNode("BumpSound")]
	private AudioStreamPlayer _bumpSound;

	[GNode("Camera2D")]
	private Camera2D camera;

	[GNode("InfoBox")]
	private RichTextLabel _info;

	[GNode("JumpSound")]
	private AudioStreamPlayer2D _jumpAudio;

	[GNode("OneLiveUp")]
	private AudioStreamPlayer _oneLiveUp;

	[GNode("SkiddingSound")]
	private AudioStreamPlayer2D _skiddingAudio;

	#endregion

	// ----------------------------------------------------------------------------------------- \\

	#region Global Properties

	public static Parameter<int> CollectedCoins = new(0, "{0:D3}");
	public static Parameter<int> LivesLeft = new(0, "{0:D2}");
	public static Parameter<int> TotalScore = new(0, "{0:D7}");

	#endregion

	// ----------------------------------------------------------------------------------------- \\

	#region Functional State Properties

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
	private bool Transforming => _animate.Animation == "Transform" && _animate.Frame == _animate.Frames.GetFrameCount("Transform");

	#endregion

	// ----------------------------------------------------------------------------------------- \\

	#region Parameter and Variables

	private bool Debug { get; set; }
	private Vector2 StartPosition { get; set; }
	private float CameraTime { get; set; }


	private readonly PlayerParameter player = new();

	#endregion
}
