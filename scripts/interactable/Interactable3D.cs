using Godot;

public partial class Interactable3D : Node3D, IInteractable
{
    public override void _Ready()
    {
        base._Ready();
        // GetNode<Sprite2D>("Sprite2D").Material = new ShaderMaterial
        // {
        //     Shader = ResourceLoader.Load<Shader>("res://Shaders/Outline.gdshader")
        // };;
    }

    public virtual void Interact() {}

	public virtual void Interact(Entity entity) {}

	public void Highlight()
    {
    }

    public void Unhighlight()
    {
    }
}
