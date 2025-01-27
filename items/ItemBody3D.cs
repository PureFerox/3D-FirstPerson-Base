using Godot;

public partial class ItemBody3D : RigidBody3D
{
	[Export] public Item itemData;

	public override void _Ready()
	{
		Node instance = itemData.Model.Instantiate();
		AddChild(instance);
	}

	public override void _PhysicsProcess(double delta)
	{
	}
}
