using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AddItemTest : MonoBehaviour
{
    public string itemName;

    public void AddItem()
    {
        if (!string.IsNullOrEmpty(itemName))
        {
            GameEventsManager.Instance.inventoryEvents.AddItem(itemName);
            Debug.Log("Item added: " + itemName);
        }
        else
        {
            Debug.LogWarning("Item name is empty!");
        }
    }
}