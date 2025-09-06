using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class PlayCutSceneButton : MonoBehaviour
{
    public ECondition eCondition;
    private bool playerInrange = false;
    public PlayableDirector playableDirector;

    public void CheckToPlay(Component sender, object data)
    {
        PlayCutScene();
    }

    private bool HasCompleted(ECondition eCondition)
    {
        switch (eCondition)
        {
            case ECondition.CompletedFirstCutscene:
                return GameFlowManager.Instance.Data.CompletedFirstCutscene;
            case ECondition.CompletedSecondCutscene:
                return GameFlowManager.Instance.Data.CompletedSecondCutscene;
            case ECondition.CompletedThirdCutscene:
                return GameFlowManager.Instance.Data.CompletedThirdCutscene;
            case ECondition.CompletedAllCutscene:
                return GameFlowManager.Instance.Data.CompletedAllCutscene;
            default:
                return false;
        }
    }

    public void PlayCutScene()
    {
        if (HasCompleted(eCondition)) return;

        if (!HasCompleted(eCondition))
        {
            if (playerInrange)
            {
                AssignPlayer(PlayerController.LocalInstance);
                playableDirector.Play();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInrange = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInrange = true;
        }
    }

    private void AssignPlayer(PlayerController player)
    {
        foreach (var playableAssetOutput in playableDirector.playableAsset.outputs)
        {
            if (playableAssetOutput.streamName == "PlayerTrack")
            {
                playableDirector.SetGenericBinding(playableAssetOutput.sourceObject, player.GetComponent<Animator>());
            }
        }
        if (SceneManager.GetActiveScene().name == Loader.Scene.CutScene.ToString()) player.gameObject.transform.position = new Vector3(4.371f, 1.154f, 0);
        else if (SceneManager.GetActiveScene().name == Loader.Scene.WorldScene.ToString()) player.gameObject.transform.position = new Vector3(-1.69902f, 37.40618f, 0);
    }
}
