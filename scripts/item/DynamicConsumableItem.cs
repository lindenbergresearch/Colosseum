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

namespace Renoir {

	/// <summary>
	/// A collide-able consumable item.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class DynamicConsumableItem<T> : KinematicEntity2D, IGameItem<T> where T : IItemConsumer {
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
		}

		/// <inheritdoc />
		public sealed override void HandleCollision(KinematicCollision2D collision2D) {
			if (State == ItemState.Consumable && collision2D is T consumer) {
				OnConsumerRequest(consumer);
			}
		}
	}

}
