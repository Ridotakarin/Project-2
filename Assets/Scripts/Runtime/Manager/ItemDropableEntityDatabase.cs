using System.Collections.Generic;
using UnityEngine;

public class ItemDropableEntityDatabase : PersistentSingleton<ItemDropableEntityDatabase>
{

    [SerializeField]
    private List<ItemDropableEntitySO> entityList = new List<ItemDropableEntitySO>();
   
    
    public ItemDropableEntitySO GetEntity(string entityId)
    {
        foreach(var entity in entityList)
        {
            if (entity.id == entityId)
            {
                Debug.Log("Found entity: " + entity.name);
                return entity;
            }
        }
        return null;
    }
}
