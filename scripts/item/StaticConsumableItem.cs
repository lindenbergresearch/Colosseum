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
using Renoir.item;
using static Renoir.Logger;

namespace Renoir {

	/// <summary>
	/// Abstract static consumable item.
	/// An item which does not collide or move.
	/// </summary>
	public abstract class StaticConsumableItem<T> : Area2D, IGameItem<T> where T : IItemConsumer {
		/// <inheritdoc />
		public ItemState State { get; set; }

		/// <inheritdoc />
		public abstract void OnConsumerRequest(T consumer);


		/// <inheritdoc />
		public void SetConsumed() {
			State = ItemState.Consumed;

			Visible = false;
			SetProcess(false);
			SetPhysicsProcess(false);
			SetProcessInput(false);
			Disconnect("body_entered", this, nameof(OnBodyEntered));
		}


		/// <summary>
		/// Catch signal and check if a valid consume tries to consume the item.
		/// </summary>
		/// <param name="body"></param>
		private void OnBodyEntered(Object body) {
			trace($"A body: {body} entered this static consumable item: {this}");

			if (State == ItemState.Consumable && body is T consumer) {
				debug($"The consumer: {body} requests consuming: {this}");
				OnConsumerRequest(consumer);
			} else
				debug($"The consumer: {body} cant consume: {this}");
		}

		/// <summary>
		/// Called after class init.
		/// </summary>
		public virtual void Ready() { }

		/// <inheritdoc />
		public sealed override void _Ready() {
			this.Init();
			Ready();

			Connect("body_entered", this, nameof(OnBodyEntered));

			debug($"Created static consumable item: {this}");
		}
	}

}
