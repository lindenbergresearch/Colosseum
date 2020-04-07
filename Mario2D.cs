using System;
using Godot;
using static DynamicStateCombiner;
using static PropertyPool;
using static Renoir.RMath;

/// <summary>
///     Main Player character
/// </summary>
public class Mario2D : Player2D, ICoinCollector {
    /** PROPERTIES *********************************************************************/
    private readonly Property<int> pCoins = RegisterNewProperty("main.player.coins", 0, "$main.playerinfo");

    private readonly Property<int> pLives = RegisterNewProperty("main.player.lives", 0, "$main.playerinfo");
    private readonly Property<int> pScore = RegisterNewProperty("main.player.score", 0, "$main.playerinfo");

    /** PROPERTIES *********************************************************************/
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

    private bool Grounded => IsOnFloor();


    private DynamicStateCombiner Jumping { get; set; }
    private DynamicStateCombiner Falling { get; set; }
    private DynamicStateCombiner IsDead { get; set; }

    private bool Walking { get; set; }
    private bool Running { get; set; }


    private bool SkiddingLeft => Grounded && motion.MovingRight && ActionKey.Left;
    private bool SkiddingRight => Grounded && motion.MovingLeft && ActionKey.Right;
    private bool Skidding => SkiddingLeft || SkiddingRight;


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

            var direction = "?";

            if (coll.Normal == Vector2.Down) direction = "TOP    ↑";

            if (coll.Normal == Vector2.Up) direction = "DOWN   ↓";

            if (coll.Normal == Vector2.Left) direction = "RIGHT ->";

            if (coll.Normal == Vector2.Right) direction = "<-  LEFT";

            Logger.debug(
                $"Pos: {coll.Position} Vel: {coll.ColliderVelocity} Source: {coll.Collider} Normal: {coll.Normal} ({direction})");

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
        if (ActionKey.Select) Debug = !Debug;


        Walking = Grounded && !Skidding && motion.Abs.X <= Parameter.MAX_WALKING_SPEED && motion.Abs.X > 0;
        Running = Grounded && !Skidding && motion.Abs.X > Parameter.MAX_WALKING_SPEED;

        var c = 123.12f.exceeds(12);
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

            speed *= ActionKey.Run ? Parameter.MAX_RUNNING_SPEED : Parameter.MAX_WALKING_SPEED;
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

        if (Grounded && ActionKey.Jump) {
            motion.Y = -(Parameter.JUMP_SPEED + Math.Abs(motion.X) * Parameter.JUMP_PUSH_FACTOR);
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
        Logger.debug("Reset player...");
        pLives.Value = 3;
        pCoins.Value = 0;
    }


    /// INIT *************************************************************************
    /// <summary>
    ///     Init method
    /// </summary>
    public override void Ready() {
        this.SetupNodeBindings();
        this.SetupNativeStates();

        Logger.debug("Setup player...");

        ResetPlayer();

        pCoins.Format = "{0:D3}";
        pScore.Format = "{0:D7}";
        pLives.Format = "{0:D2}";

        Jumping = fun(() => !Grounded && motion.MovingUp);
        Falling = fun(() => !Grounded && motion.MovingDown);
        IsDead = fun(() => pLives.Value == 0);


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