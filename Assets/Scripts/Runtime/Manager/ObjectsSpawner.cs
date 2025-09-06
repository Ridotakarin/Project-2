using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ECondition
{
    CompletedFirstCutscene,
    CompletedSecondCutscene,
    CompletedThirdCutscene,
    CompletedFourthCutscene,
    CompletedAllCutscene,
}

public class ObjectsSpawner : MonoBehaviour
{
    public enum EObjDialogue
    {
        Default,
        Quest,
        Npc
    }   
    
    public enum ENpcAction
    {
        None,
        Chopping
    }   
    
    public EObjDialogue eObjDialogue;
    public ENpcAction action;
    public List<ECondition> conditionsToSpawn;
    public ECondition conditionForNotSpawning;
    [SerializeField] private string knotName;
    [SerializeField] private GameObject objectPrefab;

    private void OnEnable()
    {
        GameEventsManager.Instance.objectEvents.onSpawnObject += SpawnObject;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.objectEvents.onSpawnObject -= SpawnObject;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SpawnObject();
    }

    private void Start()
    {
        SpawnObject();
    }

    private bool HasCompleted(ECondition eCondition)
    {
        if (eCondition == ECondition.CompletedFirstCutscene) return GameFlowManager.Instance.Data.CompletedFirstCutscene;
        if (eCondition == ECondition.CompletedSecondCutscene) return GameFlowManager.Instance.Data.CompletedSecondCutscene;
        if (eCondition == ECondition.CompletedThirdCutscene) return GameFlowManager.Instance.Data.CompletedThirdCutscene;
        if (eCondition == ECondition.CompletedAllCutscene) return GameFlowManager.Instance.Data.CompletedAllCutscene;
        return false;
    }

    private bool CheckConditionsToSpawn()
    {
        foreach (var eCondition in conditionsToSpawn)
        {
            if (!HasCompleted(eCondition)) return false;
        }

        return true;
    }

    private bool CheckConditionForNotSpawn()
    {
        if (HasCompleted(conditionForNotSpawning)) return true;
        return false;
    }    

    private void SpawnObject()
    {
        if (!CheckConditionsToSpawn()) return;

        if (CheckConditionForNotSpawn()) return;

        if (eObjDialogue == EObjDialogue.Default)
        {
            DialogueTrigger dlTrigger = Instantiate(objectPrefab, this.gameObject.transform.position, Quaternion.identity).GetComponent<DialogueTrigger>();
            dlTrigger.enabled = true;
            dlTrigger.SetKnotName(knotName);
        }
        else if (eObjDialogue == EObjDialogue.Npc)
        {
            NpcController npcGO = Instantiate(objectPrefab, this.gameObject.transform.position, Quaternion.identity).GetComponent<NpcController>();
            DialogueTrigger dialogueTrigger = npcGO.GetComponentInChildren<DialogueTrigger>();
            if (dialogueTrigger != null)
            {
                dialogueTrigger.enabled = true;
                dialogueTrigger.SetKnotName(knotName);
            }

            if (action == ENpcAction.Chopping)
            {
                npcGO.StartChoppingTrees();
            }
        }
        else if (eObjDialogue == EObjDialogue.Quest)
        {
            Instantiate(objectPrefab, this.gameObject.transform.position, Quaternion.identity);
        }
    }    
}
