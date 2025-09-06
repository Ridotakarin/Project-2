using System;
using UnityEngine;

public class InventoryEvents
{
    public event Action<string> onItemAdded;
    public void AddItem(string itemName)
    {
        onItemAdded?.Invoke(itemName);
    }

    public event Action<InventoryItem, int> onAddItemToUI;
    public void AddItemToUI(InventoryItem invenItem, int index)
    {
        onAddItemToUI?.Invoke(invenItem, index);
    }

    public event Action<string, int> onRemoveItem;
    public void RemoveItem(string itemName, int amout)
    {
        onRemoveItem?.Invoke(itemName, amout);
    }

    public event Action<int> onItemRemoved;
    public void OnItemRemoved(int index)
    {
        onItemRemoved?.Invoke(index);
    }

    public event Action<int> upDateAmount;
    public void UpDateAmount(int slotIndex)
    {
        upDateAmount?.Invoke(slotIndex);
    }
}
