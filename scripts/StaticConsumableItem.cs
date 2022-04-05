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

	/// <summary>
	/// Basic interface for item consumer. 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IItemConsumer {
	}

	/// <summary>
	/// Abstract static consumable item.
	/// An item which does not collide or move.
	/// </summary>
	public abstract class StaticConsumableItem<T> : Area2D where T : IItemConsumer {
		/// <summary>
		/// Indicates if the item has been consumed.
		/// </summary>
		public bool Consumed { get; set; }

		/// <summary>
		/// Catch signal and check if a valid consume tries to consume the item.
		/// </summary>
		/// <param name="body"></param>
		private void OnBodyEntered(Object body) {
			trace($"A body: {body} entered this static consumable item: {this}");

			if (!Consumed && body is T consumer) {
				OnConsumerRequest(consumer);
			}
		}

		/// <summary>
		/// Called on subclass if a valid consume request happened.
		/// </summary>
		/// <param name="consumer"></param>
		public abstract void OnConsumerRequest(T consumer);

		/// <summary>
		/// Called in subclass on ready.
		/// </summary>
		public abstract void Ready();


		/// <summary>
		/// Set item state to consumed and may deactivate in scene-tree.
		/// </summary>
		/// <param name="deactivate"></param>
		protected void SetConsumed(bool deactivate = true) {
			Consumed = true;

			if (deactivate) {
				Visible = false;
				SetProcess(false);
				SetPhysicsProcess(false);
				SetProcessInput(false);
				Disconnect("body_entered", this, nameof(OnBodyEntered));
			}
		}

		/// <summary>
		/// Init object and connect signal.
		/// </summary>
		public sealed override void _Ready() {
			this.Init();
			Ready();

			Connect("body_entered", this, nameof(OnBodyEntered));

			debug($"Created static consumable item: {this}");
		}
	}

}
