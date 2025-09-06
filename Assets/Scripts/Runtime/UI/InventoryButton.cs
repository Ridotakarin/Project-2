using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryButton : MonoBehaviour
{
    [SerializeField] private GameEvent onOpenInventory;
    public void OpenInventory()
    {
        onOpenInventory.Raise(this,ActionMap.UI);
    }

}
