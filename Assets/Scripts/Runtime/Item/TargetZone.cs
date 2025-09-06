using Unity.Netcode;
using UnityEngine;

public class TargetZone : NetworkBehaviour
{
    
    private ItemWorldControl _itemWorldControl;
    private void Awake()
    {
        _itemWorldControl = GetComponentInParent<ItemWorldControl>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _itemWorldControl.targetTransform == null && _itemWorldControl.CanPickup.Value)
        {
            var netObj = _itemWorldControl.GetComponent<NetworkObject>();
            if (netObj != null && netObj.IsSpawned)
            {
                SetTargetTransform(netObj, collision.GetComponent<NetworkObject>()); // set target transform to player transform
            }
            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.transform == _itemWorldControl.targetTransform)
        {
            var netObj = _itemWorldControl.GetComponent<NetworkObject>();
            if (netObj != null && netObj.IsSpawned)
            {
                SetTargetTransform(netObj); // remove target transform
            }
        }
    }
    public void SetTargetTransform(NetworkObject itemWorldControlNetObj, NetworkObject targetTransform = null)
    {
        if(targetTransform != null)
        RequestSetTargetTransformServerRpc(itemWorldControlNetObj, targetTransform);
        else
            RequestToRemoveTargetTransformServerRpc(itemWorldControlNetObj);
    }


    [ServerRpc(RequireOwnership = false)]
    private void RequestSetTargetTransformServerRpc(NetworkObjectReference itemWorldRef, NetworkObjectReference playerRef)
    {
        SetTargetTransformClientRpc(itemWorldRef, playerRef);
    }

    [ClientRpc]
    private void SetTargetTransformClientRpc(NetworkObjectReference itemWorldRef, NetworkObjectReference playerRef)
    {
        if(itemWorldRef.TryGet(out NetworkObject itemWorldObj) && playerRef.TryGet(out NetworkObject playerObj))
        {
            var itemWorldControl = itemWorldObj.GetComponent<ItemWorldControl>();
            var playerTransform = playerObj.GetComponent<Transform>();
            itemWorldControl.SetTargetTransform(playerTransform);
        }
    }

    

    [ServerRpc(RequireOwnership = false)]
    private void RequestToRemoveTargetTransformServerRpc(NetworkObjectReference itemWorldRef)
    {

        RemoveTargetTransformClientRpc(itemWorldRef);
    }

    [ClientRpc]
    private void RemoveTargetTransformClientRpc(NetworkObjectReference itemWorldRef)
    {
        if (itemWorldRef.TryGet(out NetworkObject itemWorldObj))
        {
            var itemWorldControl = itemWorldObj.GetComponent<ItemWorldControl>();
            itemWorldControl.targetTransform = null;
        }
    }
    
}
