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
using Renoir.item;
using static Renoir.Logger;

#endregion

namespace Renoir {

	/// <inheritdoc />
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


		private CoinType Type { get; set; } = CoinType.Single;

		/*---------------------------------------------------------------------*/

		/// <inheritdoc />
		public override void Ready() {
			_animatedSprite.Play();
		}

		/// <inheritdoc />
		public override void OnConsumerRequest(ICoinConsumer consumer) {
			var valid = consumer.DoCoinConsume((int) Type, this);

			if (valid) {
				_audioStreamPlayer.Play();
				SetConsumed();
				debug($"Coin consumed by consumer: {consumer}");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private enum CoinType {
			Single = 1,
			Special = 5,
			Super = 10
		}
	}

}
