using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_DropZone : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameEvent onCloseInventory;
    [SerializeField] private InventoryManagerSO _inventoryManagerSO;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(_inventoryManagerSO.isOpeningInventory == false) return;
        if (_inventoryManagerSO.currentDraggingItem == null) CloseInventory();
        else
        {
            var DraggedItem = _inventoryManagerSO.currentDraggingItem;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                DropAllOfDraggingItemIntoWorld(DraggedItem);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                DropOneOfDraggingItemIntoWorld(DraggedItem);
            }
        }
        
    }


    public void DropAllOfDraggingItemIntoWorld(UI_InventoryItem draggedItem)
    {
        ItemWorld itemWorldToDrop = draggedItem.InventoryItem.GetItemWorld();

        ItemWorldManager.Instance.DropItemIntoWorld(itemWorldToDrop, true, true);
        _inventoryManagerSO.RemoveInventoryItem(draggedItem.InventoryItem);
        Destroy(draggedItem.gameObject);
        _inventoryManagerSO.RefreshCurrentHoldingItem();
    }

    public void DropOneOfDraggingItemIntoWorld(UI_InventoryItem draggedItem)
    {
        if (draggedItem.InventoryItem.Quantity == 1)
        {
            DropAllOfDraggingItemIntoWorld(draggedItem);
            return;
        }
        int ogQuantity = draggedItem.InventoryItem.Quantity;
        draggedItem.InventoryItem.DecreaseQuantity(1);
        draggedItem.RefreshCount();
        ItemWorld itemWorldToDrop = draggedItem.InventoryItem.GetItemWorld();
        itemWorldToDrop.SetId(System.Guid.NewGuid().ToString());
        itemWorldToDrop.SetQuantity(1);
        ItemWorldManager.Instance.DropItemIntoWorld(itemWorldToDrop,true,true);
    }


    public void CloseInventory()
    {
        if(_inventoryManagerSO.isOpeningInventory)
        onCloseInventory.Raise(this, ActionMap.Player);
    }

}
