using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditorInternal.Profiling.Memory.Experimental;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using static UnityEditor.Progress;
#endif

public class UI_InventoryItem : MonoBehaviour, IPointerDownHandler
{
    [Header("UI")]
    public Image image;
    public Text countText;

    public Transform parentAfterDrag;
    private InventoryItem _inventoryItem;

    public InventoryItem InventoryItem => _inventoryItem;

    public static Action<int, int, Item> OnItemInCraftingSlotDrag;
    public static event Action<int, int, UI_InventoryItem> OnCraftingSlotAdded;
    public static event Action OnNewItemCreatedBeginDrag;
    [SerializeField]
    private bool itemJustGotCreated = false;



    private bool _isDragging = false;
    public bool IsDragging
    {
        get => _isDragging;
        set
        {
            _isDragging = value;
        }
    }


    //Game Event
    [SerializeField] private InventoryManagerSO _inventoryManagerSO;
    [SerializeField] private GameEvent onItemBeginDrag;
    [SerializeField] private GameEvent onCraftingWindowClose;

    public RectTransform rectTransform; // UI element to move
    public Canvas canvas;                // Your overlay canvas
    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();

        if (canvas == null)
        {
            Debug.LogError("No Canvas found in parent hierarchy. Dragging will not work.");
        }

        
    }
    private void Update()
    {
        if (!IsDragging || canvas == null) return;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );

        rectTransform.anchoredPosition = localPoint;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
    }
    public void InitialiseItem(InventoryItem newItem)
    {
        _inventoryItem = newItem;
        SetItemImage();
        RefreshCount();
    }
    public void SetItemImage()
    {
        if (_inventoryItem.Item.type == ItemType.Crop)
        {
            if (_inventoryItem.Item.cropLevelImage != null && _inventoryItem.Item.cropLevelImage.Length > _inventoryItem.Level)
            {
                Debug.Log("Setting crop level image for item: " + _inventoryItem.Item.name + " at level: " + _inventoryItem.Level);
                _inventoryItem.Item.image = _inventoryItem.Item.cropLevelImage[_inventoryItem.Level];
            }
        }
        
        image.sprite = _inventoryItem.Item.image;
    }
    public void RefreshCount() 
    {
        countText.text = _inventoryItem.Quantity.ToString();
        countText.gameObject.SetActive(_inventoryItem.Quantity > 1);
    }


    public void ChangeSlot(Transform slot, int index)
    {
        parentAfterDrag = slot;
        transform.SetParent(parentAfterDrag);
        _inventoryItem.UpdateSlotIndex(index);
    }

    


    private void CheckParentBeforeDrag()
    {
        if (parentAfterDrag.GetComponent<UI_CraftingSlot>() != null)
        {
            UI_CraftingSlot uI_CraftingSlot = parentAfterDrag.GetComponent<UI_CraftingSlot>();
            OnItemInCraftingSlotDrag?.Invoke(uI_CraftingSlot.i, uI_CraftingSlot.j, _inventoryItem.Item);
        }
    }

    public void StartToDrag()
    {
        IsDragging = true;
        image.raycastTarget = false;
        _inventoryManagerSO.currentDraggingItem = this;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!_inventoryManagerSO.isOpeningInventory &&
           !UIWorkbenchPanel.Instance.isOpen &&
           !UIShop.Instance.IsShopOpen) return; // prevent dragging when inventory is closed
        onItemBeginDrag.Raise(this,null); // for sound
        if (_inventoryManagerSO.currentDraggingItem == null) // start to drag
        {
            StartToDrag();
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                CheckParentBeforeDrag();

            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if(_inventoryItem.Quantity > 1)
                SplitItem();
                else CheckParentBeforeDrag();
            }

            if (itemJustGotCreated)
            {
                OnNewItemCreatedBeginDrag?.Invoke();
                _inventoryManagerSO.inventory.AddItemToInventory(InventoryItem, -1);
                itemJustGotCreated = false;
            }
        }
        else // put down
        {
            UI_InventoryItem draggedItem = _inventoryManagerSO.currentDraggingItem;
            
            if (itemJustGotCreated) // neu dang click vao 1 item moi dc tao ra
            {
                if (draggedItem.InventoryItem.Item == InventoryItem.Item &&
                    draggedItem.InventoryItem.Item.stackable &&
                    draggedItem.InventoryItem.Quantity < draggedItem.InventoryItem.MaxStack &&
                    draggedItem.InventoryItem.Level == InventoryItem.Level)
                {
                    OnNewItemCreatedBeginDrag?.Invoke();

                    draggedItem.InventoryItem.IncreaseQuantity(1);
                    draggedItem.RefreshCount();

                    Destroy(gameObject); // destroy the item that just got created

                }
            }
            else
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    if (_inventoryItem.Item != draggedItem._inventoryItem.Item || 
                        InventoryItem.Quantity == InventoryItem.MaxStack ||
                        _inventoryItem.Item == draggedItem._inventoryItem.Item && _inventoryItem.Level != draggedItem._inventoryItem.Level)
                    {
                        SwapThisItemWith(draggedItem);
                    }
                    else if(_inventoryItem.Item == draggedItem._inventoryItem.Item && 
                        InventoryItem.Quantity < InventoryItem.MaxStack &&
                        _inventoryItem.Level == draggedItem._inventoryItem.Level)
                    {
                        OnItemDropOnItem(draggedItem);
                    }
                }
                else if (eventData.button == PointerEventData.InputButton.Right)
                {
                    if (_inventoryItem.Item == draggedItem._inventoryItem.Item &&
                        _inventoryItem.Quantity < _inventoryItem.MaxStack &&
                        _inventoryItem.Level == draggedItem._inventoryItem.Level)
                    {
                        _inventoryItem.IncreaseQuantity(1);
                        RefreshCount();
                        draggedItem.InventoryItem.DecreaseQuantity(1);
                        draggedItem.RefreshCount();
                        if (draggedItem.InventoryItem.Quantity <= 0)
                        {
                            _inventoryManagerSO.RemoveInventoryItem(draggedItem.InventoryItem);
                            _inventoryManagerSO.currentDraggingItem = null;
                            Destroy(draggedItem.gameObject);
                        }
                    }
                }
            }


            

                
        }

    }

    private void SplitItem()
    {
        int ogQuantity = _inventoryItem.Quantity;
        int newItemQuantity = ogQuantity / 2;
        _inventoryItem.SetQuantity(ogQuantity- newItemQuantity);
        RefreshCount();
        InventoryItem newItem = new InventoryItem(System.Guid.NewGuid().ToString(), _inventoryItem.Item, _inventoryItem.SlotIndex, newItemQuantity, _inventoryItem.Level);
        _inventoryManagerSO.PutItemDownByRightClick(newItem,newItem.SlotIndex,parentAfterDrag.gameObject);
    }


    public void SwapThisItemWith(UI_InventoryItem otherItem)
    {
        Transform thisSlot = transform.parent;
        StartToDrag();
        otherItem.parentAfterDrag = thisSlot;
        otherItem.OnItemFinishDrag();
    }

    public void OnItemDropOnItem(UI_InventoryItem draggedItem)
    {
        if (InventoryItem.Quantity + draggedItem.InventoryItem.Quantity <= InventoryItem.MaxStack)
        {
            InventoryItem.IncreaseQuantity(draggedItem.InventoryItem.Quantity);
            RefreshCount();
            _inventoryManagerSO.RemoveInventoryItem(draggedItem.InventoryItem);
            _inventoryManagerSO.currentDraggingItem = null;
            Destroy(draggedItem.gameObject);
        }
        else
        {
            int quantityToAdd = InventoryItem.MaxStack - InventoryItem.Quantity;
            InventoryItem.IncreaseQuantity(quantityToAdd);
            RefreshCount();
            draggedItem.InventoryItem.DecreaseQuantity(quantityToAdd);
            draggedItem.RefreshCount();

        }
    }


    public void OnItemFinishDrag()
    {
        if(_inventoryManagerSO.currentDraggingItem == this) 
            _inventoryManagerSO.currentDraggingItem = null;
        IsDragging = false;
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
        CheckItemSlot();
    }

    public void CheckItemSlot()
    {
        if (GetComponentInParent<UI_InventorySlot>() != null)
        {
            _inventoryItem.UpdateSlotIndex(GetComponentInParent<UI_InventorySlot>().slotIndex);
        }
        else if (GetComponentInParent<UI_CraftingSlot>() != null)
        {
            UI_CraftingSlot uI_CraftingSlot = GetComponentInParent<UI_CraftingSlot>();
            if (uI_CraftingSlot != null)
            {
                OnCraftingSlotAdded?.Invoke(uI_CraftingSlot.i, uI_CraftingSlot.j, this);
            }
        }
    }

    public void IsItemCreated(bool value)
    {
        this.itemJustGotCreated = value;
    }

    public void OnCraftingWindowClosed()
    {
        onCraftingWindowClose.Raise(this, null);
    }
}