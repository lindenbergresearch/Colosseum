using Godot;


/// <summary>
///     Interface to implement collision handling
/// </summary>
public interface ICollider {

	/// <summary>
	///     Triggered by the collider
	/// </summary>
	/// <param name="collision"></param>
	void OnCollide(object sender, KinematicCollision2D collision);

}
