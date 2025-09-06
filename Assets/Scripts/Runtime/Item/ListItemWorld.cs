using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class ListItemWorld
{
    [SerializeField] List<ItemWorld> _items;

    public List<ItemWorld> Items
    { 
        get { return _items; }
        private set { _items = value; }
    }

    public ListItemWorld() 
    {
        _items = new List<ItemWorld>();
    }

    public void AddItemWorld(ItemWorld item)
    {
        var itemToAdd = _items.Find(i => i.Id == item.Id);
        if(itemToAdd == null)
        _items.Add(item);
    }

    public void RemoveItemWorld(ItemWorld item)
    {
        var itemToRemove = _items.Find(i => i.Id == item.Id);
        _items.Remove(itemToRemove);
    }

    public void SetListItemWorld(List<ItemWorld> items)
    {
        Items = items;
    }
}
