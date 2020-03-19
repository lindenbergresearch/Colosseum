using System;
using Godot;

namespace Colosseum {

	/// <summary>
	/// 
	/// </summary>
	public class Motion2D {
		private Vector2 velocity;

		/// <summary>
		/// Represents the velocity of the 2D motion as Vector2
		/// </summary>
		public Vector2 Velocity {
			get => velocity;
			set => velocity = value;
		}

		/// <summary>
		/// Returns the X part of the velocity vector
		/// </summary>
		public float X {
			get => velocity.x;
			set => velocity.x = value;
		}

		/// <summary>
		/// Returns the Y part of the velocity vector
		/// </summary>
		public float Y {
			get => velocity.y;
			set => velocity.y = value;
		}

		/// <summary>
		/// Returns the absolute values of both vector components
		/// </summary>
		public Motion2D Abs {
			get => new Motion2D(new Vector2(Math.Abs(velocity.x), Math.Abs(velocity.y)));
		}


		/// <summary>
		/// Create a new Motion2D by some Vector2
		/// </summary>
		/// <param name="velocity"></param>
		public Motion2D(Vector2 velocity) {
			Velocity = velocity;
		}

		/// <summary>
		/// Reset velocity vector
		/// </summary>
		public void reset() {
			velocity = new Vector2(0, 0);
		}


		public bool upward() => Velocity.y < 0;
		public bool downward() => Velocity.y > 0;
		public bool left() => Velocity.x < 0;
		public bool right() => Velocity.x > 0;

		//public bool exceedsX(float value) => Velocity.x > value;

		public static implicit operator Motion2D(Vector2 vector2) => new Motion2D(vector2);
		public static implicit operator Vector2(Motion2D motion2D) => new Vector2(motion2D.Velocity);
	}

}
