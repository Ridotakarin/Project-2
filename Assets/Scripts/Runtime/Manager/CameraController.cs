using Cinemachine;
using UnityEngine;

public class CameraController : PersistentSingleton<CameraController>
{
    private bool hasFollowTarget = false;
    public CinemachineVirtualCamera followCamera;

    private void OnEnable()
    {
        GameEventsManager.Instance.playerEvents.onPlayerSpawned += SetFollowTarget;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.playerEvents.onPlayerSpawned -= SetFollowTarget;
    }

    private void Start()
    {
        if (!hasFollowTarget)
        {
            PlayerController player = PlayerController.LocalInstance;
            if (player != null)
            {
                SetFollowTarget(player);
            }
            else
            {
                Debug.LogWarning("No PlayerController found in the scene. Camera will not follow any target.");
            }
        }
    }

    public void SetFollowTarget(PlayerController player = null)
    {
        if(player != null)
        {
            followCamera.Follow = player.transform;
            hasFollowTarget = true;
            return;
        }

        followCamera.Follow = null;
        hasFollowTarget = false;
    }    

    public void RefreshFollowCamera(CinemachineVirtualCamera newFollowCamera)
    {
        followCamera = newFollowCamera;
        PlayerController player = PlayerController.LocalInstance;
        SetFollowTarget(player);
    }
}