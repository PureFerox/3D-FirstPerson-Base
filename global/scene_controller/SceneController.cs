using Godot;

public partial class SceneController : Node
{
	public enum SCENE_OPTION {
		DELETE,
		KEEP_RUNNING,
		HIDE
	}

	[Export] public Node3D World3D;
	[Export] public Control GUI;

	public Node3D Current3DScene;
	public Control CurrentGUIScene;

	public override void _Ready()
	{
		Global.Instance.SceneController = this;
		Current3DScene = null;
		CurrentGUIScene = GUI.GetNode<Control>("SplashManager");
	}

	private void HandleSceneOptions(Node currentScene, SCENE_OPTION option) {
		if (currentScene != null) {
			switch (option) {
				case SCENE_OPTION.DELETE:
					currentScene.QueueFree();
					break;
				case SCENE_OPTION.KEEP_RUNNING:
					if (currentScene is Control c)
						c.Visible = false;
					else if (currentScene is Node3D n)
						n.Visible = false;
					break;
				case SCENE_OPTION.HIDE:
					currentScene.GetParent().RemoveChild(currentScene);
					break;
			}
		}
	}

	private void InstantiateScene(PackedScene newScene) {
		if (newScene != null) {
            Node scene = newScene.Instantiate();
			if (scene is Node3D n) {
                World3D.AddChild(n);
                Current3DScene = n;
            } else if (scene is Control c) {
                GUI.AddChild(c);
                CurrentGUIScene = c;
            }
        } else {
            GD.PrintErr($"Failed to load scene: {newScene}");
        }
	}

	public void Change3DScene(string newScenePath, SCENE_OPTION option) {
		HandleSceneOptions(Current3DScene, option);
		PackedScene newScene = ResourceLoader.Load<PackedScene>(newScenePath);
		InstantiateScene(newScene);
	}

	public void Change3DScene(PackedScene newScene, SCENE_OPTION option) {
		HandleSceneOptions(Current3DScene, option);
		InstantiateScene(newScene);
	}

	public void ChangeGUIScene(string newScenePath, SCENE_OPTION option) {
		HandleSceneOptions(CurrentGUIScene, option);
		PackedScene newScene = ResourceLoader.Load<PackedScene>(newScenePath);
		InstantiateScene(newScene);
	}

	public void ChangeGUIScene(PackedScene newScene, SCENE_OPTION option) {
		HandleSceneOptions(CurrentGUIScene, option);
		InstantiateScene(newScene);
	}
}