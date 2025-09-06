using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;
#if UNITY_EDITOR
using static UnityEditor.Progress;
#endif

[System.Serializable]
public class InventoryItem : IItemHolder
{
    [NonSerialized] private Item _item;
    [SerializeField] private string _id;
    [SerializeField] private string _itemName;
    [SerializeField] private int _quantity;
    [SerializeField] private int _maxStack;
    [SerializeField] private int _slotIndex;
    [SerializeField] private int _level;

    public Item Item
    { get { return _item; } }

    public string Id
    { get { return _id; } }

    public string ItemName
    { get { return _itemName; } }

    public int Quantity
    { get { return _quantity; } }

    public int MaxStack
    { get { return _maxStack; } }

    public int SlotIndex
    {  get { return _slotIndex; } }

    public int Level
    { get { return _level; } }
    public InventoryItem(string id, Item item, int index)
    {
        if (id == null)
        {
            this._id = Guid.NewGuid().ToString();
        }
        else
        {
            this._id = id;
        }
        this._item = item;
        this._itemName = item.itemName;
        this._quantity ++;
        this._maxStack = item.stackable ? 12 : 1;
        this._slotIndex = index;
    }

    public InventoryItem(string id, Item item, int index, int quantity, int level)
    {
        this._id = id;
        this._item = item;
        this._itemName = item.itemName;
        this._quantity = quantity;
        this._maxStack = item.stackable ? 12 : 1;
        this._slotIndex = index;
        this._level = level;
    }

    public void SetItem(Item item)
    {
        this._item = item;
    }

    public void IncreaseQuantity(int amount)
    {
        _quantity += amount;
    }

    public void DecreaseQuantity(int amount) 
    {
        _quantity -= amount;
        if (_quantity < 0) _quantity = 0;
    }

    public void SetQuantity(int amount)
    {
        _quantity = amount;
        if (_quantity < 0) _quantity = 0;
    }

    public void UpdateSlotIndex(int newIndex)
    {
        _slotIndex = newIndex;
    }

    public ItemWorld GetItemWorld()
    {
        ItemWorld itemWorld = new ItemWorld(_id, _item, _quantity, Vector3.zero, _level);
        return itemWorld;
    }
}
