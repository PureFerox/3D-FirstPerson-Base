using Godot;

public partial class PlayerInputController : InputController
{
	#region Mouse
	[Export] public float MouseSensitivity;
	public float TILT_LOWER_LIMIT = Mathf.DegToRad(-90f);
	public float TILT_UPPER_LIMIT = Mathf.DegToRad(90f);
	public Vector3 MouseRotation;
	public float RotationInput;
	public float TiltInput;
	public Vector3 PlayerRotation;
	public Vector3 CameraRotation;
	#endregion
	
	#region Movement
	public float ACCEL = 10f;
	public float FRIC_GROUND = 10f;
	public float FRIC_AIR = 0f;
	public float WalkSpeed;
	public float SprintSpeed;
	public float CrouchSpeed;
	public const float SPRINT_MULTIPLIER = 1.75f;
	public const float CROUCH_MULTIPLIER = 0.4f;
	public Vector2 InputDir;
	public bool IsSprinting;
	public bool IsJumping;
	public bool IsCrouching;
	#endregion

	#region Jump
	public const float JUMP_PEAK_TIME = 0.25f;
	public const float JUMP_FALL_TIME = 0.25f;
	public const float JUMP_HEIGHT = 0.75f;
	public const float JUMP_DISTANCE = 2f;
	public float JumpVelo;
	#endregion
	
	#region Gravity
	public float GravJump;
	public float GravFall;
	#endregion

	public override void _Ready()
	{
		MouseSensitivity /= 100f;

		GravJump = ((2f * JUMP_HEIGHT) / Mathf.Pow(JUMP_PEAK_TIME, 2f));
		GravFall = ((2f * JUMP_HEIGHT) / Mathf.Pow(JUMP_FALL_TIME, 2f));
		JumpVelo = GravJump * JUMP_PEAK_TIME;
		WalkSpeed = JUMP_DISTANCE/(JUMP_PEAK_TIME + JUMP_FALL_TIME);
		SprintSpeed = WalkSpeed * SPRINT_MULTIPLIER;
		CrouchSpeed = WalkSpeed * CROUCH_MULTIPLIER;

		Input.MouseMode = Input.MouseModeEnum.Captured;

		InputDir = Vector2.Zero;
		IsSprinting = false;
		IsJumping = false;
	}

	public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

		if (Input.MouseMode == Input.MouseModeEnum.Captured && @event is InputEventMouseMotion mouseInput) {
			RotationInput = -mouseInput.Relative.X * MouseSensitivity;
			TiltInput = -mouseInput.Relative.Y * MouseSensitivity;
		}
    }

	public void CalculateRotations(double delta) {
		MouseRotation.X += (float)(TiltInput * delta);
		MouseRotation.X = Mathf.Clamp(MouseRotation.X, TILT_LOWER_LIMIT, TILT_UPPER_LIMIT);
		MouseRotation.Y += (float)(RotationInput * delta);

		PlayerRotation = new Vector3(0f, MouseRotation.Y, 0f);
		CameraRotation = new Vector3(MouseRotation.X, 0f, 0f);
	}

	public void GetInput() {
		IsSprinting = Input.IsActionPressed("game_sprint");
		IsJumping = Input.IsActionJustPressed("game_jump");
		IsCrouching = Input.IsActionPressed("game_crouch");

		InputDir = Vector2.Zero;
		InputDir.Y = Input.GetAxis("game_forward", "game_backward");
		InputDir.X = Input.GetAxis("game_strafe_left", "game_strafe_right");
		InputDir = InputDir.Normalized();
	}
}
