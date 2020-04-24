using Godot;

namespace Renoir {

	/// <summary>
	/// Abstract basic class for all kinematic objects.
	/// </summary>
	public abstract class KinematicEntity2D : KinematicBody2D {

		/// <summary>
		/// Motion vector
		/// </summary>
		public Motion2D Motion { get; set; }

		/// <summary>
		/// Determines of the item is active
		/// </summary>
		public bool Active { get; set; }


		/// <summary>
		/// </summary>
		protected virtual void Draw() { }


		/// <summary>
		/// </summary>
		/// <param name="event"></param>
		protected virtual void Input(InputEvent @event) { }


		/// <summary>
		/// </summary>
		/// <param name="delta"></param>
		protected virtual void PhysicsProcess(float delta) { }


		/// <summary>
		/// </summary>
		/// <param name="delta"></param>
		protected virtual void Process(float delta) { }


		/// <summary>
		/// </summary>
		protected virtual void Ready() { }


		/// <summary>
		///     <para>Overridable function called by the engine (if defined) to draw the canvas item.</para>
		/// </summary>
		public sealed override void _Draw() { Draw(); }


		/// <summary>
		///     <para>Called when there is an input event. The input event propagates up through the node tree until a node consumes it.</para>
		///     <para>It is only called if input processing is enabled, which is done automatically if this method is overridden, and can be toggled with <see cref="M:Godot.Node.SetProcessInput(System.Boolean)" />.</para>
		///     <para>To consume the input event and stop it propagating further to other nodes, <see cref="M:Godot.SceneTree.SetInputAsHandled" /> can be called.</para>
		///     <para>For gameplay input, <see cref="M:Godot.Node._UnhandledInput(Godot.InputEvent)" /> and <see cref="M:Godot.Node._UnhandledKeyInput(Godot.InputEventKey)" /> are usually a better fit as they allow the GUI to intercept the events first.</para>
		/// </summary>
		public sealed override void _Input(InputEvent @event) { Input(@event); }


		/// <summary>
		///     <para>Called during the physics processing step of the main loop. Physics processing means that the frame rate is synced to the physics, i.e. the <c>delta</c> variable should be constant.</para>
		///     <para>It is only called if physics processing is enabled, which is done automatically if this method is overridden, and can be toggled with <see cref="M:Godot.Node.SetPhysicsProcess(System.Boolean)" />.</para>
		///     <para>Corresponds to the  notification in <see cref="M:Godot.Object._Notification(System.Int32)" />.</para>
		/// </summary>
		public sealed override void _PhysicsProcess(float delta) { PhysicsProcess(delta); }


		/// <summary>
		///     <para>Called during the processing step of the main loop. Processing happens at every frame and as fast as possible, so the <c>delta</c> time since the previous frame is not constant.</para>
		///     <para>It is only called if processing is enabled, which is done automatically if this method is overridden, and can be toggled with <see cref="M:Godot.Node.SetProcess(System.Boolean)" />.</para>
		///     <para>Corresponds to the  notification in <see cref="M:Godot.Object._Notification(System.Int32)" />.</para>
		/// </summary>
		public sealed override void _Process(float delta) { Process(delta); }


		/// <summary>
		///     <para>Called when the node is "ready", i.e. when both the node and its children have entered the scene tree. If the node has children, their <see cref="M:Godot.Node._Ready" /> callbacks get triggered first, and the parent node will receive the ready notification afterwards.</para>
		///     <para>Corresponds to the  notification in <see cref="M:Godot.Object._Notification(System.Int32)" />. See also the <c>onready</c> keyword for variables.</para>
		///     <para>Usually used for initialization. For even earlier initialization,  may be used. See also <see cref="M:Godot.Node._EnterTree" />.</para>
		///     <para>Note: <see cref="M:Godot.Node._Ready" /> may be called only once for each node. After removing a node from the scene tree and adding again, <c>_ready</c> will not be called for the second time. This can be bypassed with requesting another call with <see cref="M:Godot.Node.RequestReady" />, which may be called anywhere before adding the node again.</para>
		/// </summary>
		public sealed override void _Ready() {
			this.SetupGlobalProperties();
			this.SetupNodeBindings();

			Active = true;
			Motion = (0, 0);

			Ready();
			Logger.trace($"Created entity: {this}");
		}


		/// <summary>
		/// Shorthand method to update motion based on current state
		/// </summary>
		public void UpdateMotion()
			=> Motion = MoveAndSlide(Motion, Motion2D.FLOOR_NORMAL);


	}

}
