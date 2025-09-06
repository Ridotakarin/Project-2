using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AreaEntranceType
{
    Mine,
    Town,
    HouseInDoor,
    HouseOutDoor,
    Other
}
public class AreaEntrance : MonoBehaviour
{
    [SerializeField] private string transitionName;
    [SerializeField] private AreaEntranceType entranceType;

    private void Start() 
    {
        CheckAndSpawnPlayer();
    }

    private void OnEnable()
    {
        MultiSceneManger.Instance.SubscribeToEntranceList(this);
    }

    private void OnDisable()
    {
        MultiSceneManger.Instance.UnsubscribeFromEntranceList(this);
    }

    public bool CheckAndSpawnPlayer()
    {
        if (transitionName == SceneManagement.SceneTransitionName)
        {
            if (entranceType == AreaEntranceType.Mine || entranceType == AreaEntranceType.HouseOutDoor)
            {
                PlayerController.LocalInstance.transform.position = this.transform.position + Vector3.down * 2;
            }
            else if (entranceType == AreaEntranceType.HouseInDoor)
            {
                PlayerController.LocalInstance.transform.position = this.transform.position + Vector3.up;
            }
            else
            {
                PlayerController.LocalInstance.transform.position = this.transform.position;
            }

            PlayerController.LocalInstance.CanMove = true;
            UI_Fade.Instance.FadeToClear();
            UI_Fade.Instance.gameObject.SetActive(false);

            return true;
        }

        return false;
    }
}
