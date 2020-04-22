using System;
using Godot;
using Renoir;
using static Renoir.Logger;


/// <summary>
///     Main Player character
/// </summary>
public class Mario2D : Player2D, ICoinCollector, IConsumer, IPropertyChangeHandler {

	/// <summary>
	/// Power states
	/// </summary>
	public enum PowerStateEnum {
		SMALL,
		BIG,
		FIRE
	}


	// ----------------------------------------------------------------------------------------- \\

	#region Godot Node Bindings

	[GNode("AnimatedSprite")]
	private Godot.AnimatedSprite _animate;

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

	[Register("main.player.coins", "{0:D3}")]
	public static Property<int> CollectedCoins { get; set; }

	[Register("main.player.lives", "{0:D2}")]
	public static Property<int> LivesLeft { get; set; }

	[Register("main.player.score", "{0:D7}")]
	public static Property<int> TotalScore { get; set; }

	[Register("main.powerstate")]
	public static PowerStateEnum PowerState { get; set; } = PowerStateEnum.SMALL;

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


	private PlayerParameter player = new PlayerParameter();

	#endregion


	/// <summary>
	///     Handle coins
	/// </summary>
	/// <param name="coin"></param>
	/// <returns></returns>
	public bool onCoinCollect(Coin coin) {
		debug($"Collecting coin: {coin}");

		CollectedCoins += 1;
		TotalScore += 250;

		if (CollectedCoins.Value == 100) {
			SetLives();
			CollectedCoins.Value = 0;
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
	/// 
	/// </summary>
	/// <param name="item"></param>
	public void OnConsume(object item) {
		debug($"consuming {item}");
	}


	/// <summary>
	///     Check collisions and pass event to all collider
	/// </summary>
	protected override void UpdateCollisions(float delta) {
		foreach (var collision2D in this.GetCollider()) {
			/* match collider type **/
			switch (collision2D.Collider) {
				case TileMap _ when collision2D.Bottom():
					_bumpSound.Play();
					continue;
				case ICollider collider:
					trace(
						$"position={collision2D.Position} velocity={collision2D.ColliderVelocity} collider={collision2D.Collider} vector={collision2D.Normal} {collision2D.Normal.ToDirectionArrow()}");
					collider.OnCollide(this, collision2D);
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
		if (!Idle && !_animate.IsPlaying()) _animate.Play();
	}


	/// <summary>
	/// Update sound fx
	/// </summary>
	override protected void UpdateAudio(float delta) {
		if (Skidding && !_skiddingAudio.Playing && Motion.X > player.MaxWalkingSpeed * 0.8) _skiddingAudio.Play();
		if (AboutToJump) _jumpAudio.Play();
	}


	private void printDebug() {
		var vect = string.Format("V = {0,6:000.0}, {1,6:000.0}", Motion.X, Motion.Y);
		var pos = string.Format("P = {0,6:000.0}, {1,6:000.0}", GlobalPosition.x, GlobalPosition.y);
		_info.Text =
			$"Velocity: {vect}\nPosition: {pos}\nSL: {SkiddingLeft} " +
			$"SR: {SkiddingRight}\nGrounded: {Grounded}\nWalk: {Walking} " +
			$"Run: {Running}\nJump: {Jumping} Fall: {Falling}" +
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
		LivesLeft.Value = 3;
		CollectedCoins.Value = 0;
	}


	/// <summary>
	///     Init method
	/// </summary>
	override public void Ready() {
		debug("Initialize Player");

		ResetPlayer();

		player = BasicParameter.LoadFromJson<PlayerParameter>("PlayerParameter.json");

		camera.LimitLeft = 0;
		camera.LimitBottom = (int) Game.VIEWPORT_RESOLUTION.y;

		StartPosition = GlobalPosition;

		PropertyPool.AddSubscription("main.mouse.*", this);
	}


	override protected void Draw() {
	}


	override protected void Process(float delta) {
	}


	public void OnPropertyChange<T>(Property<T> sender, PropertyEventArgs<T> args) {
		debug($"D: {Node2D.MouseButton} from: {sender} args: {args}");

		if (Node2D.MouseButton.Value != null && Node2D.MouseButton.Value.Pressed) {
			camera.Offset = Node2D.MouseButton.Value.Position;
		}
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