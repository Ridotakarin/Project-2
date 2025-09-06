using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
#if UNITY_EDITOR
using static UnityEditor.Progress;
#endif

[System.Serializable]
public class Inventory
{
    [SerializeField] private int _maxSlotInventory;
    [SerializeField] private List<InventoryItem> _inventoryItemList;

    public int MaxSlotInventory
    {  get { return _maxSlotInventory; } }

    public List<InventoryItem> InventoryItemList
    {  get { return _inventoryItemList; } }

    public Inventory() 
    {
        this._maxSlotInventory = 44;
        this._inventoryItemList = new List<InventoryItem>(_maxSlotInventory);
    }

    public InventoryItem GetInventoryItemOfIndex(int index)
    {
        InventoryItem item = _inventoryItemList.Find(i => i.SlotIndex == index);
        return item;
    }

    public InventoryItem GetInventoryItemFromName(string name)
    {
        InventoryItem item = _inventoryItemList.Find(i => i.ItemName == name);
        if (item == null) return null;
        return item;
    }

    public bool AddItemToInventory(InventoryItem item, int slotIndex)
    {
        item.UpdateSlotIndex(slotIndex);
        _inventoryItemList.Add(item);
        return true;
    }

    //public bool AddItemToInventory(string id, Item item, int slotIndex, int amount)
    //{
    //    InventoryItem inventoryItem = new InventoryItem(id, item, slotIndex, amount,1);
    //    _inventoryItemList.Add(inventoryItem);
    //    return true;
    //}

    //public bool RemoveItemFromInventory(Item item, int amount = 1)
    //{
    //    InventoryItem existingItem = _inventoryItemList.Find(i => i.Item == item);
    //    if (existingItem != null)
    //    {
    //        existingItem.DecreaseQuantity(amount);
    //        if (existingItem.Quantity <= 0) _inventoryItemList.Remove(existingItem); 
    //        return true;
    //    }
    //    return false;
    //}

    public bool RemoveInventoryItem(InventoryItem inventoryItem)
    {
        InventoryItem existingItem = _inventoryItemList.Find(i => i.Id == inventoryItem.Id);
        if (existingItem != null)
        {
            _inventoryItemList.Remove(existingItem);
            return true;
        }
        return false;
    }

    public InventoryItem FindItemInInventory(int index)
    {
        InventoryItem existingItem = _inventoryItemList.Find(i => i.SlotIndex == index);
        if (existingItem != null) return existingItem;
        return null;
    }

    public int GetIndexOfInventory(InventoryItem inventoryItem)
    {
        return _inventoryItemList.IndexOf(inventoryItem);
    }
}
