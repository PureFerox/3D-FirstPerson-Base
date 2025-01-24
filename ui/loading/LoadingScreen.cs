using Godot;

public partial class LoadingScreen : TransitionalScene
{
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void OnShaderPrecompilationComplete() {
		Global.Instance.SceneController.Change3DScene(_defaultWorldScene, SceneController.SCENE_OPTION.HIDE);
		Global.Instance.SceneController.ChangeGUIScene(_defaultUIScene, SceneController.SCENE_OPTION.DELETE);
	}
}
