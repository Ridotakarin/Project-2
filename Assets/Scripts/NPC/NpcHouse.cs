using UnityEngine;

public class NpcHouse : MonoBehaviour
{
    [SerializeField] private GameObject npcPrefab;

    [SerializeField] private Transform npcSpawnPoint;

    [SerializeField] private Collider2D npcDestroyArea;

    private void OnEnable()
    {
        GameEventsManager.Instance.npcEvents.onSpawnNpc += SpawnNpc;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.npcEvents.onSpawnNpc -= SpawnNpc;
    }

    private void SpawnNpc()
    {
        if (npcPrefab == null || npcSpawnPoint == null)
        {
            return;
        }

        NpcController npc = Instantiate(npcPrefab, npcSpawnPoint.position, Quaternion.identity).GetComponent<NpcController>();
        if (npc == null)
        {
            Debug.LogError("Failed to instantiate NPC. Ensure the prefab has an AIController component.");
            return;
        }
        npc.SetTargetPosition(npcDestroyArea.transform);
    }
}
