using Godot;
using System;

public partial class HealthComponent : Node
{
	[Export] private float MAX_HP = 100.0f;
	private float HP;
	
	public override void _Ready()
	{
		HP = MAX_HP;
	}

	public override void _Process(double delta)
	{
	}

	public void Damage( /** Attack attack*/ ) {
		// HP -= attack.GetDamage();
	}

}
