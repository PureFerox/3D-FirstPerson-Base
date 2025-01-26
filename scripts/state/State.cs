using Godot;

public partial class State : Node
{
	public Entity Body { get; set; }
	public StateMachineComponent StateMachineComponent { get; set; }

	public virtual void Enter() {}
	public virtual void Update(double delta) {}
	public virtual void PhysicsUpdate(double delta) {}
	public virtual void Exit() {}
	public virtual void HandleInput(InputEvent @event) {}
}
