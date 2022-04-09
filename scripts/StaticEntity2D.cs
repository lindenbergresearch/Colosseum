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

using Godot;
using static Renoir.Logger;

namespace Renoir {

	public abstract class StaticEntity2D : StaticBody2D {
		/// <summary>
		///     Determines of the item is active
		/// </summary>
		public virtual bool Active { get; set; } = true;


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
		/// Called on node creation.
		/// </summary>
		protected virtual void Ready() { }

		/// <summary>
		/// Should be called upon node reset to initial state.
		/// </summary>
		public virtual void Reset() { }


		/*---------------------------------------------------------------------*/

		/// <summary>
		///     <para>Overridable function called by the engine (if defined) to draw the canvas item.</para>
		/// </summary>
		public sealed override void _Draw() { Draw(); }


		/// <summary>
		///     <para>Called when there is an input event. The input event propagates up through the node tree until a node consumes it.</para>
		///     <para>
		///         It is only called if input processing is enabled, which is done automatically if this method is overridden, and can be toggled with
		///         <see cref="M:Godot.Node.SetProcessInput(System.Boolean)" />.
		///     </para>
		///     <para>To consume the input event and stop it propagating further to other nodes, <see cref="M:Godot.SceneTree.SetInputAsHandled" /> can be called.</para>
		///     <para>
		///         For gameplay input, <see cref="M:Godot.Node._UnhandledInput(Godot.InputEvent)" /> and <see cref="M:Godot.Node._UnhandledKeyInput(Godot.InputEventKey)" /> are usually a better fit as they
		///         allow the GUI to intercept the events first.
		///     </para>
		/// </summary>
		public sealed override void _Input(InputEvent @event) { Input(@event); }


		/// <summary>
		///     <para>Called during the physics processing step of the main loop. Physics processing means that the frame rate is synced to the physics, i.e. the <c>delta</c> variable should be constant.</para>
		///     <para>
		///         It is only called if physics processing is enabled, which is done automatically if this method is overridden, and can be toggled with
		///         <see cref="M:Godot.Node.SetPhysicsProcess(System.Boolean)" />.
		///     </para>
		///     <para>Corresponds to the  notification in <see cref="M:Godot.Object._Notification(System.Int32)" />.</para>
		/// </summary>
		public sealed override void _PhysicsProcess(float delta) { PhysicsProcess(delta); }


		/// <summary>
		///     <para>Called during the processing step of the main loop. Processing happens at every frame and as fast as possible, so the <c>delta</c> time since the previous frame is not constant.</para>
		///     <para>It is only called if processing is enabled, which is done automatically if this method is overridden, and can be toggled with <see cref="M:Godot.Node.SetProcess(System.Boolean)" />.</para>
		///     <para>Corresponds to the  notification in <see cref="M:Godot.Object._Notification(System.Int32)" />.</para>
		/// </summary>
		public sealed override void _Process(float delta) { Process(delta); }


		/*---------------------------------------------------------------------*/

		public sealed override void _Ready() {
			this.Init();
			Ready();
			trace($"Created static entity: {this}");
		}
	}

}
