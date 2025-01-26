using Godot;

public partial class CrouchMove : PlayerState
{
	[Export] private ShapeCast3D _shapeCast;

	public override void PhysicsUpdate(double delta) {
		_inputController.GetInput();

		Vector3 velocity = Body.Velocity;
		float FRIC = _inputController.FRIC_GROUND;
		float SPEED = _inputController.CrouchSpeed;

		velocity = Body.HandleMovement(velocity, SPEED, FRIC, delta);

		if (!_inputController.IsCrouching && !_shapeCast.IsColliding())
			StateMachineComponent.TransitionTo("Move");

		Body.AnimationValues["AnimStandingBlendVal"] = Mathf.Lerp(Body.AnimationValues["AnimStandingBlendVal"], 1, Body.AnimationValues["BlendSpeed"] * (float)delta);
		Body.Velocity = velocity;
		Body.MoveAndSlide();
	}
}
