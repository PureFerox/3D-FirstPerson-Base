using Godot;
using System;

public partial class Boundry : Area3D
{
	[Export] Marker3D PSpawn;
	[Export] Marker3D BSpawn;
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void OnBodyEntered(Node3D body) {
		if (body is Player p) {
			p.GlobalPosition = PSpawn.GlobalPosition;
		} else if (body is RigidBody3D b) {
			b.GlobalPosition = BSpawn.GlobalPosition;
		}
	}
}
