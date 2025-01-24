using Godot;
using System.Collections.Generic;

public partial class InventoryComponent : Node
{
	[Export] private Item[] _items;
	public List<Item> Items;
	private int _slots = 10;

	public override void _Ready()
	{
		Items = new List<Item>(_slots);
		if (_items != null && _items.Length > 0)
			Items.AddRange(_items);	
	}
	
	public bool HasItem(Item item) {
		return Items.Contains(item);
	}

	public bool AddItem(Item item) {
		if (Items.Count < _slots) {
			Items.Add(item);
			GD.Print("Picked up " +item.GetName());
			return true;
		}
		GD.Print("Inventory is full");
		return false;
	}
}