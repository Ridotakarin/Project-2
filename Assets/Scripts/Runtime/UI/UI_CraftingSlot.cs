using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftingSlot : MonoBehaviour,  IPointerDownHandler
{
    public int i, j;

    [SerializeField] private InventoryManagerSO _inventoryManagerSO;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_inventoryManagerSO.currentDraggingItem == null) return;
        var DraggedItem = _inventoryManagerSO.currentDraggingItem;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            DropItemOnInventorySlot(DraggedItem);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            DropOneOfDraggingItem(DraggedItem);
        }
    }


    public void DropItemOnInventorySlot(UI_InventoryItem draggedItem)
    {
        draggedItem.parentAfterDrag = transform;
        draggedItem.OnItemFinishDrag();
    }

    public void DropOneOfDraggingItem(UI_InventoryItem draggedItem)
    {
        if (draggedItem.InventoryItem.Quantity == 1)
        {
            DropItemOnInventorySlot(draggedItem);
            return;
        }
        int ogQuantity = draggedItem.InventoryItem.Quantity;
        draggedItem.InventoryItem.SetQuantity(ogQuantity - 1);
        draggedItem.RefreshCount();
        InventoryItem newItem = new InventoryItem(System.Guid.NewGuid().ToString(), draggedItem.InventoryItem.Item, -1, 1,draggedItem.InventoryItem.Level);
        _inventoryManagerSO.PutItemDownByRightClick(newItem, newItem.SlotIndex, gameObject);
    }
    public Item GetItem()
    {
        UI_InventoryItem existingItem = transform.GetComponentInChildren<UI_InventoryItem>();
        return existingItem != null ? existingItem.InventoryItem.Item : null;
    }


}
