using System.Collections.Generic;
using UnityEngine;

public class QuestFlow : MonoBehaviour
{
    [SerializeField] private List<GameObject> fouthCutsceneQuests;

    private void Start()
    {
        foreach (GameObject quest in fouthCutsceneQuests)
        {
            quest.SetActive(false);
        }
    }

    private void Update()
    {
        if (GameFlowManager.Instance.Data.CompletedThirdCutscene) SetThirdCutsceneQuest();
    }

    private void SetThirdCutsceneQuest()
    {
        if (GameFlowManager.Instance.Data.CompletedFourthCutscene == false) return;
        foreach (GameObject quest in fouthCutsceneQuests)
        {
            quest.SetActive(true);
        }
    }
}
