using Godot;
using Godot.Collections;
using System.Threading.Tasks;

public partial class SplashManager : TransitionalScene
{
	[Export] private float _inTime = 0.5f;
	[Export] private float _fadeInTime = 1.5f;
	[Export] private float _pauseTime = 1.5f;
	[Export] private float _fadeOutTime = 1.5f;
	[Export] private float _outTime = 0.5f;
	[Export] private CenterContainer SplashContainer;
	private Array<Node> splashes;
	
	public override async void _Ready()
	{
		HideSplashes();
		await Animate();
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        EndSplash();
    }

    private void HideSplashes() {
		splashes = SplashContainer.GetChildren();
		foreach (Node n in splashes) {
			if (n is TextureRect tR) {
				Color sm = tR.Modulate;
				sm.A = 0f;
				tR.Modulate = sm;
			}
		}
	}

	private async Task Animate() {
		foreach (Node n in splashes) {
			if (n is TextureRect tR) {
				Tween tween = GetTree().CreateTween();
				tween.TweenInterval(_inTime);
				tween.TweenProperty(tR, "modulate:a", 1.0f, _fadeInTime);
				tween.TweenInterval(_pauseTime);
				tween.TweenProperty(tR, "modulate:a", 0.0f, _fadeOutTime);
				tween.TweenInterval(_outTime);
				await ToSignal(tween, "finished");
			}
		}
		EndSplash();
	}

	private void EndSplash() {
		foreach (Tween t in GetTree().GetProcessedTweens())
			t.Kill();
			
		Global.Instance.SceneController.ChangeGUIScene(_defaultUIScene, SceneController.SCENE_OPTION.DELETE);
	}
}
