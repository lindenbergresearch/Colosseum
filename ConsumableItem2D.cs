using Godot;
using Renoir;


/// <summary>
/// An abstract item which can be consumed by a consumer
/// </summary>
public abstract class ConsumableItem2D : KinematicBody2D, ICollider {
	/// <summary>
	/// 
	/// </summary>
	public bool Active { get; set; }


	/// <summary>
	/// 
	/// </summary>
	/// <param name="delta"></param>
	public abstract void PhysicsProcess(float delta);


	/// <summary>
	/// 
	/// </summary>
	public abstract void Ready();


	/// <summary>
	/// 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="collision"></param>
	public void OnCollide(object sender, KinematicCollision2D collision) {
		if (sender is IConsumer consumer) {
			consumer.OnConsume(this);
			Hide();
			QueueFree();
		}
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="delta"></param>
	public sealed override void _PhysicsProcess(float delta) {
		if (!Active) return;
		if (!Visible) Visible = true;

		foreach (var collision2D in this.GetCollider()) {
			if (collision2D.Collider is IConsumer consumer) {
				consumer.OnConsume(this);
				Hide();
				QueueFree();
				return;
			}
		}
		
		PhysicsProcess(delta);
	}


	/// <summary>
	/// Activate item
	/// </summary>
	public virtual void Activate() {
		Show();
		Active = true;
	}


	/// <summary>
	/// Deactivate item
	/// </summary>
	public virtual void Deactivate() {
		Active = false;
		Hide();
		QueueFree();
	}


	/// <summary>
	/// 
	/// </summary>
	public override void _Ready() {
		this.SetupNodeBindings();
		Hide();
		Ready();
	}

}
