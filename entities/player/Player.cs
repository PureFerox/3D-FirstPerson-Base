using System.Collections.Generic;
using Godot;

public partial class Player : Entity
{
	#region Nodes
	[Export] public PlayerInputController InputController;
	[Export] private Node3D Head;
	[Export] private Camera3D Camera;
	#endregion
	
	public Dictionary<string, float> AnimationValues;

	private float _currentSpeed;
	
    public override void _Ready()
    {
		AnimationValues = new Dictionary<string, float>();
		AnimationValues["BlendSpeed"] = 5f;
		AnimationValues["AnimStandingBlendVal"] = 0f;
		AnimationValues["BobFreq"] = 6f;
		AnimationValues["BobAmp"] = 0.0035f;
		AnimationValues["Bob"] = 0f;
    }

	private void UpdateCamera(double delta) {
		InputController.CalculateRotations(delta);

		Transform3D headTransform = Head.Transform;
		headTransform.Basis = Basis.FromEuler(InputController.CameraRotation);
		Head.Transform = headTransform;

		Vector3 headRotation = Head.Rotation;
		headRotation.Z = 0f;
		Head.Rotation = headRotation;

		Transform3D globalTransform = GlobalTransform;
		globalTransform.Basis = Basis.FromEuler(InputController.PlayerRotation);
		GlobalTransform = globalTransform;

		InputController.RotationInput = 0f;
		InputController.TiltInput = 0f;
	}

    public override void _PhysicsProcess(double delta)
	{
		UpdateCamera(delta);

		AnimationValues["AnimStandingBlendVal"] = Mathf.Clamp(AnimationValues["AnimStandingBlendVal"], 0, 1);
		_animTree.Set("parameters/IdleCrouch/blend_amount", AnimationValues["AnimStandingBlendVal"]);
	}

	public Vector3 HandleMovement(Vector3 velocity, float SPEED, float FRIC, double delta) {
		float bobMult = 0f;
		if (!IsOnFloor()) {
			SPEED = _currentSpeed;
			FRIC = InputController.FRIC_AIR;
			velocity.Y -= (velocity.Y > 0f ? InputController.GravJump : InputController.GravFall) * ((float)delta);
		} else {
			_currentSpeed = SPEED;
			velocity.Y = InputController.IsJumping ? InputController.JumpVelo : velocity.Y;
			bobMult = 1f;
		}

		Vector3 direction = (Transform.Basis * new Vector3(InputController.InputDir.X, 0f, InputController.InputDir.Y)).Normalized();

		if (direction != Vector3.Zero)
		{
			velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * SPEED, InputController.ACCEL * ((float)delta));
			velocity.X = Mathf.Lerp(velocity.X, direction.X * SPEED, InputController.ACCEL * ((float)delta));
		}
		else
		{
			velocity.Z = Mathf.Lerp(velocity.Z, 0f, FRIC * ((float)delta));
			velocity.X = Mathf.Lerp(velocity.X, 0f, FRIC * ((float)delta));
		}

		AnimationValues["Bob"] += velocity.Length() * bobMult * ((float)delta);
		
		Transform3D camTrans = Camera.Transform;
		camTrans.Basis = Basis.FromEuler(HeadBob(AnimationValues["Bob"]));
		Camera.Transform = camTrans;

		return velocity;
	}

	private Vector3 HeadBob(float bob) {
		Vector3 pos = Vector3.Zero;
		pos.Y = Mathf.Sin(bob * AnimationValues["BobFreq"] / 4) * AnimationValues["BobAmp"];
		pos.X = Mathf.Cos(bob * AnimationValues["BobFreq"] / 2) * AnimationValues["BobAmp"];
		return pos;
	}
}
