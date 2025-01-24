using System;
using System.ComponentModel;
using Godot;

public partial class Player : Entity
{
	#region Nodes
	[Export] private Node3D Head;
	[Export] private Camera3D Camera;
	[Export] private PlayerInputController _inputController;
	[Export] private ShapeCast3D _shapeCast;
	#endregion
	
	#region Animations
	private float _blendSpeed = 5;
	private float _animStandingBlendVal = 0;
	private float _bobFreq = 6f;
	private float _bobAmp = 0.0035f;
	private float _bob = 0.0f;
	#endregion

	private float _currentSpeed;
	private STATE state;
	
    public override void _Ready()
    {
    }

	private void UpdateCamera(double delta) {
		_inputController.CalculateRotations(delta);

		Transform3D headTransform = Head.Transform;
		headTransform.Basis = Basis.FromEuler(_inputController.CameraRotation);
		Head.Transform = headTransform;

		Vector3 headRotation = Head.Rotation;
		headRotation.Z = 0f;
		Head.Rotation = headRotation;

		Transform3D globalTransform = GlobalTransform;
		globalTransform.Basis = Basis.FromEuler(_inputController.PlayerRotation);
		GlobalTransform = globalTransform;

		_inputController.RotationInput = 0f;
		_inputController.TiltInput = 0f;
	}

    public override void _PhysicsProcess(double delta)
	{
		Global.Instance.GameManager.WriteToGUI("Colliding:" +_shapeCast.IsColliding());

		switch (state) {
			case STATE.MOVE:
				MoveState(delta);
				_animStandingBlendVal = Mathf.Lerp(_animStandingBlendVal, 0, _blendSpeed * (float)delta);
				break;
			case STATE.CROUCH:
				CrouchState(delta);
				_animStandingBlendVal = Mathf.Lerp(_animStandingBlendVal, 1, _blendSpeed * (float)delta);
				break;
		}
		_animStandingBlendVal = Mathf.Clamp(_animStandingBlendVal, 0, 1);
		_animTree.Set("parameters/IdleCrouch/blend_amount", _animStandingBlendVal);
	}

	private void MoveState(double delta) {
		UpdateCamera(delta);
		_inputController.GetInput();

		Vector3 velocity = Velocity;
		float FRIC = _inputController.FRIC_GROUND;
		float SPEED = _inputController.IsSprinting ? _inputController.SprintSpeed : _inputController.WalkSpeed;

		velocity = HandleMovement(velocity, SPEED, FRIC, delta);

		if (_inputController.IsCrouching > 0)
			state = STATE.CROUCH;

		Velocity = velocity;
		MoveAndSlide();
	}

	private void CrouchState(double delta) {
		UpdateCamera(delta);
		_inputController.GetInput();

		Vector3 velocity = Velocity;
		float FRIC = _inputController.FRIC_GROUND;
		float SPEED = _inputController.CrouchSpeed;

		velocity = HandleMovement(velocity, SPEED, FRIC, delta);

		if (_inputController.IsCrouching <= 0) {
			if (!_shapeCast.IsColliding()) {
				state = STATE.MOVE;
			}
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private Vector3 HandleMovement(Vector3 velocity, float SPEED, float FRIC, double delta) {
		float bobMult = 0f;
		if (!IsOnFloor()) {
			SPEED = _currentSpeed;
			FRIC = _inputController.FRIC_AIR;
			velocity.Y -= (velocity.Y > 0f ? _inputController.GravJump : _inputController.GravFall) * ((float)delta);
		} else {
			_currentSpeed = SPEED;
			velocity.Y = _inputController.IsJumping ? _inputController.JumpVelo : velocity.Y;
			bobMult = 1f;
		}

		Vector3 direction = (Transform.Basis * new Vector3(_inputController.InputDir.X, 0f, _inputController.InputDir.Y)).Normalized();

		if (direction != Vector3.Zero)
		{
			velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * SPEED, _inputController.ACCEL * ((float)delta));
			velocity.X = Mathf.Lerp(velocity.X, direction.X * SPEED, _inputController.ACCEL * ((float)delta));
		}
		else
		{
			velocity.Z = Mathf.Lerp(velocity.Z, 0f, FRIC * ((float)delta));
			velocity.X = Mathf.Lerp(velocity.X, 0f, FRIC * ((float)delta));
		}

		_bob += velocity.Length() * bobMult * ((float)delta);
		Transform3D camTrans = Camera.Transform;
		camTrans.Basis = Basis.FromEuler(HeadBob(_bob));
		Camera.Transform = camTrans;

		return velocity;
	}

	private Vector3 HeadBob(float bob) {
		Vector3 pos = Vector3.Zero;
		pos.Y = Mathf.Sin(bob * _bobFreq / 4) * _bobAmp;
		pos.X = Mathf.Cos(bob * _bobFreq / 2) * _bobAmp;
		return pos;
	}
}
