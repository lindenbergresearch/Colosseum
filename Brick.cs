using Godot;


/// <summary>
/// Simple brick which could be damaged
/// </summary>
public class Brick : StaticBody2D, ICollider {

	[GNode("AudioStreamPlayer")]
	private AudioStreamPlayer _audioStreamPlayer;
	[GNode("CollisionShape2D")]
	private Godot.CollisionShape2D _collisionShape2D;
	[GNode("Particles2D")]
	private Particles2D _particles2D;
	[GNode("Sprite")]
	private Sprite _sprite;
	public bool Broken { get; set; } = false;


	/// <summary>
	/// Handle collision
	/// </summary>
	/// <param name="collision"></param>
	public void OnCollide(object sender, KinematicCollision2D collision) {
		if (collision.Normal != Vector2.Down) return;

		_particles2D.Emitting = true;
		_audioStreamPlayer.Play();
		_sprite.Visible = false;
		_collisionShape2D.Disabled = true;
	}


	/// <summary>
	/// Init
	/// </summary>
	public override void _Ready() {
		this.SetupNodeBindings();
	}

}
