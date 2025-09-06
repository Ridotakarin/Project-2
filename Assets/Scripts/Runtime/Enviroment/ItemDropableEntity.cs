using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(NetworkObject))]
public abstract class ItemDropableEntity : NetworkBehaviour
{
    protected Damageable damageable;
    [SerializeField] protected ItemDropableEntitySO entityInfo;
    [SerializeField] protected GameObject itemDropPrefab;

    protected virtual void Awake()
    {
        damageable = GetComponent<Damageable>();
    }

    public virtual void OnHit(Vector2 knockback) { }


    public void DropItem(bool makeLessDrop)
    {
        if(!IsServer) return;
        int numItem = 0;
        numItem = UtilsClass.PickOneByRatio(entityInfo.numOfItemCouldDrop, entityInfo.ratioForEachNum);

        Debug.Log("Drop item from entity: " + numItem);
        if (makeLessDrop) numItem /= 2;
        ItemWorld itemWorldDropInfo = new ItemWorld(System.Guid.NewGuid().ToString(), entityInfo.ItemToDrop, numItem, transform.position,1);
        ItemWorldManager.Instance.DropItemIntoWorld(itemWorldDropInfo, false, false);
       
    }


}
