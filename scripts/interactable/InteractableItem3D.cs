using Godot;

public partial class InteractableItem3D : Interactable3D
{
	[Export] protected Item ItemData;

	public void Interact(InventoryComponent inventory)
    {
		inventory.AddItem(ItemData);
		QueueFree();
    }
}
