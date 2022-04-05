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
using static Renoir.Logger;

#endregion

namespace Renoir {

	/// <summary>
	/// Irem consumer for coins.
	/// </summary>
	public interface ICoinConsumer : IItemConsumer {
		/// <summary>
		/// Called on the consumer side.
		/// </summary>
		/// <param name="payload">The value of the coin.</param>
		/// <param name="item">Item ref.</param>
		bool DoCoinConsume(int payload, Object item);
	}


	/*---------------------------------------------------------------------*/

	/// <summary>
	///     Standard collectable coins
	/// </summary>
	public class Coin : StaticConsumableItem<ICoinConsumer> {
		/*---------------------------------------------------------------------*/

		[GNode("AnimatedSprite")]
		private AnimatedSprite _animatedSprite;

		[GNode("AudioStreamPlayer")]
		private AudioStreamPlayer _audioStreamPlayer;


		private CoinType coinType = CoinType.NORMAL;


		/*---------------------------------------------------------------------*/

		/// <summary>
		///     Init...
		/// </summary>
		public override void Ready() {
			_animatedSprite.Play();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="consumer"></param>
		public override void OnConsumerRequest(ICoinConsumer consumer) {
			var valid = consumer.DoCoinConsume((int) coinType, this);

			if (valid) {
				_audioStreamPlayer.Play();
				SetConsumed();
				debug($"Coin consumed by consumer: {consumer}");
			}
		}

		private enum CoinType {
			NORMAL = 1,
			SPECIAL = 5,
			SUPER = 10
		}
	}

}
