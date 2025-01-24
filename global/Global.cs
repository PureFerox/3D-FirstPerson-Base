using Godot;

public partial class Global : Node
{
	public static Global Instance { get; private set; }
	public GameManager GameManager { get; set; }
	public SceneController SceneController { get; set; }

	public override void _Ready()
	{
		Instance = this;
	}

	public override void _Process(double delta)
	{
	}
}
