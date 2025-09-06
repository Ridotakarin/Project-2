using UnityEngine;
using System.Collections.Generic;

public interface IItemHolder
{
    string ItemName { get; }
    void SetItem(Item item);
}

public class ItemDatabase : PersistentSingleton<ItemDatabase>
{
    public List<Item> items;

    public Item GetItemByName(string itemName)
    {
        return items.Find(item => item.itemName == itemName);
    }

    public void SetItem<T>(List<T> list) where T : IItemHolder
    {
        foreach (T value in list)
        {
            Item item = GetItemByName(value.ItemName);
            value.SetItem(item);
        }
    }
}
