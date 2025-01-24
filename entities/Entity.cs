using Godot;

public partial class Entity : CharacterBody3D
{
	protected enum STATE {
		MOVE,
		CROUCH
	}

	[Export] protected HealthComponent _healthComponent;
	[Export] protected AnimationPlayer _animPlayer;
	[Export] protected AnimationTree _animTree;

    public override void _Ready()
    {
    }

    public override void _PhysicsProcess(double delta)
	{
	}
}
