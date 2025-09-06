using UnityEngine;

public class CusceneHandler : MonoBehaviour
{
    public void EnableControl()
    {
        GameEventsManager.Instance.inputReader.EnableControl();
    }

    public void DisableControl()
    {
        GameEventsManager.Instance.inputReader.DisableControl();  
    }
}
