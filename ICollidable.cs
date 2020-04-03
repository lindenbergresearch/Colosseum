using Godot;


/// <summary>
///     Interface to implement collision handling
/// </summary>
public interface ICollidable {

	/// <summary>
	///     Triggered by the collider
	/// </summary>
	/// <param name="collision"></param>
	void onCollide(KinematicCollision2D collision);

}