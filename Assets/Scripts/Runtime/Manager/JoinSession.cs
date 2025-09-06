using TMPro;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoinSession : MonoBehaviour
{
    public void GetISession(ISession session)
    {
        GameEventsManager.Instance.dataEvents.OnInitialized(session.Name + "Client");
        DataPersistenceManager.Instance.LoadGame();
    }
}
