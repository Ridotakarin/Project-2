using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class DialogueTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Emote Animator")]
    [SerializeField] private Animator emoteAnimator;

    [Header("Ink Name")]
    [SerializeField] private string dialogueKnotName;

    public bool playerInRange;

    private void Awake() 
    {
        playerInRange = false;
        //visualCue.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = true;
            GameEventsManager.Instance.dialogueEvents.EnterDialogue(dialogueKnotName);
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collider) 
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = false;
        }
    }

    public void SetKnotName(string knotName)
    {
        dialogueKnotName = knotName;
    }    

    public void EnterDialogue()
    {
        GameEventsManager.Instance.dialogueEvents.EnterDialogue(dialogueKnotName);
    }    
}
