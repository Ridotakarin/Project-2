using Unity.Netcode;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using static UnityEditor.Progress;
#endif
using Unity.VisualScripting;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine.SceneManagement;

public class ItemWorldManager : NetworkPersistentSingleton<ItemWorldManager>, IDataPersistence
{
    private ListItemWorld _listItemWorld;
    public NetworkList<ItemWorldNetworkData> networkItemWorldList = new NetworkList<ItemWorldNetworkData>(
                writePerm: NetworkVariableWritePermission.Server,
                readPerm: NetworkVariableReadPermission.Everyone);
    public GameObject itemDropPrefab;
    public List<ItemWorldControl> itemsOnMap;
    public bool IsReadyToInitialize = false;
    
    #region OnLoadStuff
    public void SpawnItemOnHostLoad()
    {
        if (!IsServer) return; // Only the server should spawn items
        foreach (var item in _listItemWorld.Items)
        {
            GameObject itemGO = Instantiate(itemDropPrefab, item.Position, Quaternion.identity);

            var itemNetworkObject = itemGO.GetComponent<NetworkObject>();
            itemNetworkObject.Spawn();
            itemNetworkObject.GetComponent<ItemWorldControl>().CanPickup.Value = true; // set to true when spawn item in world
            InitializeItemWorldOnHostLoadClientRpc(itemNetworkObject, NetworkVariableConverter.ItemWorldToNetwork(item));
        }
    }

    [ClientRpc]
    private void InitializeItemWorldOnHostLoadClientRpc(NetworkObjectReference itemWorldRef, ItemWorldNetworkData itemWorldData)
    {
        if(itemWorldRef.TryGet(out NetworkObject obj))
        {
            var itemWorld = NetworkVariableConverter.ItemWorldFromNetwork(itemWorldData);
            var itemWorldControl = obj.GetComponent<ItemWorldControl>();
            itemWorldControl.InitialItemWorld(itemWorld);
        } 
    }
    private void AddItemWorldIntoNetworkList()
    {
        if (networkItemWorldList == null) return;
        foreach (var item in _listItemWorld.Items)
        {
            Debug.Log($"Adding item to network list: {item.Id} - {item.ItemName}");
            networkItemWorldList.Add(NetworkVariableConverter.ItemWorldToNetwork(item));
        }
        _listItemWorld.Items.Clear();
    }
    public void SyncItemWorldOnLateJoin()
    {
        itemsOnMap.Clear();
        itemsOnMap = FindObjectsByType<ItemWorldControl>(FindObjectsSortMode.None).ToList();
        foreach (var item in itemsOnMap)
        {
            foreach (var netItem in networkItemWorldList)
            {
                if (netItem.Id == item.id.Value)
                {
                    var itemWorld = NetworkVariableConverter.ItemWorldFromNetwork(netItem);
                    item.InitialItemWorld(itemWorld);
                    break;
                }
            }
        }
        itemsOnMap.Clear();
    }
    #endregion

    #region add and remove item world
    public void RemoveItemWorld(ItemWorld item, ItemWorldControl itemWorldControl)
    {
        RequestToRemoveItemWorldServerRpc(NetworkVariableConverter.ItemWorldToNetwork(item), itemWorldControl.GetComponent<NetworkObject>()); // send to server to remove item world

    } 

    [ServerRpc(RequireOwnership = false)]
    public void RequestToRemoveItemWorldServerRpc(ItemWorldNetworkData itemWorldNetworkData, NetworkObjectReference itemWorldRef)
    {
        if (!networkItemWorldList.Contains(itemWorldNetworkData))
        {
            Debug.LogWarning("[Server] Item not found in list");
            return;
        }


        if (itemWorldRef.TryGet(out NetworkObject obj))
        {

            networkItemWorldList.Remove(itemWorldNetworkData);
            obj.Despawn();
        }
        else
        {
            Debug.LogWarning("[Server] Could not resolve NetworkObjectReference");
        }

    }
    public void DropItemIntoWorld(ItemWorld itemWorldDropInfo, bool dropByStack, bool dropByPlayer = false)
    {
        var networkData = NetworkVariableConverter.ItemWorldToNetwork(itemWorldDropInfo); // put quantity and position to drop in here
        RequestToDropItemServerRpc(networkData,dropByStack, dropByPlayer);
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestToDropItemServerRpc(ItemWorldNetworkData itemWorldNetworkData , bool dropByStack, bool dropByPlayer, ServerRpcParams rpcParams = default)
    {
        if (dropByPlayer)
        {
            ulong senderClientId = rpcParams.Receive.SenderClientId;
            var player = NetworkManager.Singleton.ConnectedClients[senderClientId].PlayerObject;
            itemWorldNetworkData.Position = player.transform.position;
        }

        if (dropByStack || itemWorldNetworkData.Quantity == 1)
            SpawnStackOfItem(itemWorldNetworkData);
        else
            SpawnEachOfItem(itemWorldNetworkData);

    }

    private void SpawnStackOfItem(ItemWorldNetworkData itemWorldNetworkData)
    {
        Vector3 itemPos = itemWorldNetworkData.Position;
        Vector3 randomDir = UtilsClass.GetRandomDir();
        Vector3 dropPos = itemPos + randomDir * 0.2f;

        itemWorldNetworkData.Position = dropPos;

        SpawnItemByServer(itemWorldNetworkData, randomDir);
    }

    private void SpawnEachOfItem(ItemWorldNetworkData itemWorldNetworkData)
    {
        
        for (int i = 0; i < itemWorldNetworkData.Quantity; i++)
        {
            Vector3 itemPos = itemWorldNetworkData.Position;
            Vector3 randomDir = UtilsClass.GetRandomDir();
            Vector3 dropPos = itemPos + randomDir * 0.2f;

            var singleData = itemWorldNetworkData;
            singleData.Position = dropPos;
            singleData.Quantity = 1;
            singleData.Id = System.Guid.NewGuid().ToString();

            SpawnItemByServer(singleData, randomDir);
        }
    }

    private void SpawnItemByServer(ItemWorldNetworkData itemWorldNetworkData, Vector3 randomDir)
    {
        GameObject newItemObject = Instantiate(itemDropPrefab, itemWorldNetworkData.Position, Quaternion.identity);

        var newItemNetworkObject = newItemObject.GetComponent<NetworkObject>();
        newItemNetworkObject.Spawn();

        var itemWorldControl = newItemNetworkObject.GetComponent<ItemWorldControl>();
        itemWorldControl.StartWaitForPickup(1f);

        AddItemWorld(itemWorldNetworkData);
        SetItemDropStatusClientRpc(itemWorldNetworkData, newItemNetworkObject, randomDir);
    }
    public void AddItemWorld(ItemWorldNetworkData itemWorldNetworkData)
    {
        if (networkItemWorldList.Contains(itemWorldNetworkData)) return;
        networkItemWorldList.Add(itemWorldNetworkData);
    }
    [ClientRpc]
    private void SetItemDropStatusClientRpc(ItemWorldNetworkData itemWorldNetworkData, NetworkObjectReference itemNetworkObject, Vector3 randomDir)
    {
        if (itemNetworkObject.TryGet(out NetworkObject obj))
        {
            var itemWorld = NetworkVariableConverter.ItemWorldFromNetwork(itemWorldNetworkData);

            var itemWorldControl = obj.GetComponent<ItemWorldControl>();
            itemWorldControl.InitialItemWorld(itemWorld);
            itemWorldControl.GetComponent<SimpleBounceObject>().Bounce(randomDir, 0.5f); // bounce item when drop into world

        }
    }
    #endregion

    

    
    public void LoadData(GameData gameData)
    {
        if (!IsHost) return;
        //if (SceneManager.GetActiveScene().name == Loader.Scene.MainMenu.ToString() ||
        //    SceneManager.GetActiveScene().name == Loader.Scene.LoadingScene.ToString() ||
        //    SceneManager.GetActiveScene().name == Loader.Scene.LobbyScene.ToString()||
        //    SceneManager.GetActiveScene().name == Loader.Scene.CharacterSelectScene.ToString() ||
        //    SceneManager.GetActiveScene().name == Loader.Scene.UIScene.ToString()) 
        //    return;

        _listItemWorld = gameData.ListItemWold;
        itemsOnMap = FindObjectsByType<ItemWorldControl>(FindObjectsSortMode.None).ToList();

        if (_listItemWorld.Items == null || _listItemWorld.Items.Count == 0) // neu ko co item nao trong world dc luu trong file save truoc do
        {
            _listItemWorld = new ListItemWorld();
            foreach (var item in itemsOnMap) // khi add item vao scene truoc khi bat dau game thi se xai cai nay
            {
                ItemWorld itemWorld = item.GetItemWorld();
                _listItemWorld.AddItemWorld(itemWorld);
                Destroy(item.gameObject);
            }
        }
        else // neu co item trong file save thi se xai cai nay
        {
            ItemDatabase.Instance.SetItem(_listItemWorld.Items); // cap nhat lai item trong file save
            foreach (var item in itemsOnMap)
            {
                bool existItem = _listItemWorld.Items.Find(x => x.Id == item.id.Value) != null ? true : false;
                if (!existItem) // neu item truoc khi game bat dau nay ko ton tai trong file save thi add vao list
                {
                    ItemWorld itemWorld = item.GetItemWorld();
                    _listItemWorld.AddItemWorld(itemWorld);
                }
                Destroy(item.gameObject);
            }

        }
        if (_listItemWorld?.Items?.Count > 0)
        {
            SpawnItemOnHostLoad();
        }

        itemsOnMap.Clear();

        AddItemWorldIntoNetworkList();
    }
    

    private void MoveItemWorldFromNetworkToLocalList()
    {
        _listItemWorld.Items.Clear();
        foreach (var item in networkItemWorldList)
        {
            var itemWorld = NetworkVariableConverter.ItemWorldFromNetwork(item);
            _listItemWorld.AddItemWorld(itemWorld);
        }
    }
    public void SaveData(ref GameData gameData)
    {
        if (!IsHost) return;
        MoveItemWorldFromNetworkToLocalList();
        gameData.SetListItemWorld(_listItemWorld);
    }

}
