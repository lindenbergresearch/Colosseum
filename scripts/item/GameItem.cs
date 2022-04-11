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

namespace Renoir.item {

	/// <summary>
	/// Current Item state.
	/// </summary>
	public enum ItemState {
		Inactive,
		Consumable,
		Consumed
	}

	/*---------------------------------------------------------------------*/

	/// <summary>
	/// Standard properties and functions.
	/// </summary>
	public interface IGameItem<T> where T : IItemConsumer {
		/// <summary>
		/// The current state of the item.
		/// </summary>
		ItemState State { get; set; }


		/// <summary>
		/// Sets an item to consumed and disappear.
		/// </summary>
		public void SetConsumed();


		/// <summary>
		/// Called on subclass if a valid consume request happened.
		/// </summary>
		/// <param name="consumer"></param>
		public void OnConsumerRequest(T consumer);
	}


	/*---------------------------------------------------------------------*/

	/// <summary>
	/// Basic interface for item consumer. 
	/// </summary>
	public interface IItemConsumer {
	}

}
