using Godot;

public partial class Entity : CharacterBody3D
{
	[Export] protected HealthComponent _healthComponent;
	[Export] protected StateMachineComponent _stateMachineComponent;
	[Export] protected InventoryComponent _inventory;
	[Export] protected AnimationPlayer _animPlayer;
	[Export] protected AnimationTree _animTree;

    public override void _Ready()
    {
    }

    public override void _PhysicsProcess(double delta)
	{
	}
}
