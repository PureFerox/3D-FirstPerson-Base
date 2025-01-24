using Godot;

public partial class Reticle : CenterContainer
{
	[Export] public float DOT_RADIUS = 4.0f;
	[Export] public Color DOT_COLOR;

    public override void _Ready()
    {
		QueueRedraw();
    }

    public override void _Draw()
    {
        base._Draw();
		    DrawCircle(new Vector2(0,0), DOT_RADIUS, DOT_COLOR);
    }
}
