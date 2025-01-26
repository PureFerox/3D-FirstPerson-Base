using Godot;
using System;

public partial class PlayerState : State
{
	public new Player Body;
	protected PlayerInputController _inputController;

	public override void Enter()
	{
		Body = base.Body as Player;
		_inputController = Body.InputController;
	}

	public override void _Process(double delta)
	{
	}
}
