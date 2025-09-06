using System.Collections.Generic;
using UnityEngine;

public class NPCCutsceneController : MonoBehaviour
{
    public void EnterDialogue(string knotName)
    {
        GameEventsManager.Instance.dialogueEvents.EnterDialogue(knotName);
    }    

    
}
