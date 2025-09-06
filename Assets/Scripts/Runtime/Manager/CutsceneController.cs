using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Unity.Netcode;
using System.Collections;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    private bool hasPlayerAssigned = false;
    [SerializeField] private PlayableDirector cutsceneDirector;

    private void OnEnable()
    {
        GameEventsManager.Instance.playerEvents.onPlayerSpawned += AssignPlayer;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.playerEvents.onPlayerSpawned -= AssignPlayer;
    }

    private void Start()
    {
        if (!hasPlayerAssigned)
        {
            PlayerController player = PlayerController.LocalInstance;
            if (player != null)
            {
                AssignPlayer(player);
            }
            else
            {
                Debug.LogWarning("No PlayableDirector found in the scene. Cutscene cannot be played.");
            }
        }
    }

    private void AssignPlayer(PlayerController player)
    {
        if (!NetworkManager.Singleton.IsHost) return;
        
        if (GameFlowManager.Instance.Data.CompletedSecondCutscene) return;

        foreach (var playableAssetOutput in cutsceneDirector.playableAsset.outputs)
        {
            if (playableAssetOutput.streamName == "PlayerTrack")
            {
                cutsceneDirector.SetGenericBinding(playableAssetOutput.sourceObject, player.GetComponent<Animator>());
            }
        }
        if (SceneManager.GetActiveScene().name == Loader.Scene.CutScene.ToString()) player.gameObject.transform.position = new Vector3(4.371f, 1.154f, 0);
        else if (SceneManager.GetActiveScene().name == Loader.Scene.WorldScene.ToString()) player.gameObject.transform.position = new Vector3(-1.69902f, 37.40618f, 0);
        cutsceneDirector.Play();
        hasPlayerAssigned = true;
    }

    public void PlayCutscene()
    {
        if (cutsceneDirector.state == PlayState.Playing || cutsceneDirector.state == PlayState.Paused)
        {
            Debug.LogWarning("Cutscene is already playing or paused.");
            return;
        }

        if (!hasPlayerAssigned)
        {
            PlayerController player = PlayerController.LocalInstance;
            if (player != null)
            {
                AssignPlayer(player);
            }
            else
            {
                Debug.LogWarning("No PlayerController found. Cutscene cannot be played.");
                return;
            }
        }

        cutsceneDirector.Play();
    }
}
