using Godot;

public partial class GameManager : Node
{
	public enum GAME_STATE {
		RUNNING,
		PAUSED,
		MENU
	}

	public override void _Ready()
	{
		Global.Instance.GameManager = this;
	}

	public void WriteToGUI(string t) {
		if (Global.Instance.SceneController.CurrentGUIScene.Name == "HUD") {
			Label label = Global.Instance.SceneController.CurrentGUIScene.GetNode<Label>("DebugLabel");
			label.Text = t;
		}
	}
}
