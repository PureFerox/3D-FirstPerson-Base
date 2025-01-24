using Godot;
using System;

public partial class NPC : Entity, IInteractable
{
	public override void _Ready()
	{
		// GetNode<Sprite2D>("Sprite2D").Material = new ShaderMaterial
        // {
        //     Shader = ResourceLoader.Load<Shader>("res://Shaders/Outline.gdshader")
        // };;
	}

    public void Interact()
    {
        throw new NotImplementedException();
    }

    public void Interact(Entity entity)
    {
        throw new NotImplementedException();
    }

    public void Highlight()
    {
        throw new NotImplementedException();
    }

    public void Unhighlight()
    {
        throw new NotImplementedException();
    }

}
