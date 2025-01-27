using Godot;

[GlobalClass]
public partial class Item : Resource
{
	[Export] public string Name;
	[Export] public PackedScene Model;
}
