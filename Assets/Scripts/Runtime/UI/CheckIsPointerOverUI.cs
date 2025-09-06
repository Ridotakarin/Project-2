using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class CheckIsPointerOverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private InventoryManagerSO _inventoryManagerSO;


    public void OnPointerEnter(PointerEventData eventData)
    {
        _inventoryManagerSO.IsPointerOverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _inventoryManagerSO.IsPointerOverUI = false;
    }
}
