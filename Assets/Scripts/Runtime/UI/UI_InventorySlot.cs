using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using static UnityEditor.Progress;
#endif

public class UI_InventorySlot : MonoBehaviour, IPointerDownHandler
{
    public Image image;
    public Sprite selectedColor, notSelectedColor;

    public int slotIndex;

    [SerializeField] private InventoryManagerSO _inventoryManagerSO;
    private void Awake()
    {
        Deselect();
    }

    // Select slot
    public void Select()
    {
        image.sprite = selectedColor;
    }

    public void Deselect()
    {
        image.sprite = notSelectedColor;
    }


    // Drop item into slot

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_inventoryManagerSO.currentDraggingItem == null) return;
        var DraggedItem = _inventoryManagerSO.currentDraggingItem;
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            DropItemOnInventorySlot(DraggedItem);
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
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
        if(draggedItem.InventoryItem.Quantity == 1)
        {
            DropItemOnInventorySlot(draggedItem);
            return;
        }
        int ogQuantity = draggedItem.InventoryItem.Quantity;
        draggedItem.InventoryItem.SetQuantity(ogQuantity - 1);
        draggedItem.RefreshCount();
        InventoryItem newItem = new InventoryItem(System.Guid.NewGuid().ToString(), draggedItem.InventoryItem.Item, slotIndex, 1, draggedItem.InventoryItem.Level);
        _inventoryManagerSO.PutItemDownByRightClick(newItem, newItem.SlotIndex, gameObject);
    }
}