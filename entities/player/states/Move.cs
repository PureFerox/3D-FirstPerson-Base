using Godot;

public partial class Move : PlayerState
{
	public override void PhysicsUpdate(double delta) {
		_inputController.GetInput();

		Vector3 velocity = Body.Velocity;
		float FRIC = _inputController.FRIC_GROUND;
		float SPEED = _inputController.IsSprinting ? _inputController.SprintSpeed : _inputController.WalkSpeed;

		velocity = Body.HandleMovement(velocity, SPEED, FRIC, delta);

		if (_inputController.IsCrouching)
			StateMachineComponent.TransitionTo("CrouchMove");

		Body.AnimationValues["AnimStandingBlendVal"] = Mathf.Lerp(Body.AnimationValues["AnimStandingBlendVal"], 0, Body.AnimationValues["BlendSpeed"] * (float)delta);
		Body.Velocity = velocity;
		Body.MoveAndSlide();
	}
}
