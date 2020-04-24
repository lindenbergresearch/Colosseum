using Godot;
using static Renoir.Logger;

namespace Renoir {

	/// <summary>
	/// Coin collector interface
	/// </summary>
	public interface ICoinCollector {

		/// <summary>
		/// Called upon coin is touched.
		/// </summary>
		/// <param name="coin">Ref to the coin</param>
		bool onCoinCollect(Coin coin);
	}


	/// <summary>
	/// Standard collectable coins
	/// </summary>
	public class Coin : Area2D {

		[GNode("AnimatedSprite")]
		private Godot.AnimatedSprite _animatedSprite;

		[GNode("AudioStreamPlayer")]
		private AudioStreamPlayer _audioStreamPlayer;


		public bool Picked { get; set; }


		/// <summary>
		/// Init...
		/// </summary>
		public override void _Ready() {
			this.SetupNodeBindings();

			Picked = false;

			_animatedSprite.Play();

			Connect("body_entered", this, nameof(onBodyEnter));
		}


		/// <summary>
		/// Coin collected
		/// </summary>
		/// <param name="body"></param>
		public void onBodyEnter(Object body) {
			debug($"Body entered this coin: {body}");

			if (Picked || !(body is ICoinCollector)) {
				debug($"Body: {body} couldn't collect coins.");
				return;
			}

			debug($"Calling interface on: {body}");

			Picked = (body as ICoinCollector).onCoinCollect(this);

			if (!Picked) return;

			_audioStreamPlayer.Play();
			Picked = true;
			Visible = false;
			SetProcess(false);
			SetPhysicsProcess(false);
			SetProcessInput(false);
			Disconnect("body_entered", this, nameof(onBodyEnter));
		}
	}

}