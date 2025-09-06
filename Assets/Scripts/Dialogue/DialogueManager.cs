using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using Unity.VisualScripting;

public class DialogueManager : Singleton<DialogueManager>
{
    [Header("Ink Story")]
    [SerializeField] private TextAsset inkJson;

    private Story story;
    private int currentChoiceIndex = -1;

    private bool dialoguePlaying = false;
    private bool dialogueFinish = false;

    private InkExternalFunctions inkExternalFunctions;
    private InkDialogueVariables inkDialogueVariables;

    protected override void Awake()
    {
        base.Awake();

        story = new Story(inkJson.text);
        inkExternalFunctions = new InkExternalFunctions();
        inkDialogueVariables = new InkDialogueVariables(story);
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.dialogueEvents.onEnterDialogue += EnterDialogue;
        GameEventsManager.Instance.inputReader.uiActions.submitEvent += OnSubmit;
        GameEventsManager.Instance.dialogueEvents.onUpdateChoiceIndex += UpdateChoiceIndex;
        GameEventsManager.Instance.dialogueEvents.onUpdateInkDialogueVariable += UpdateInkDialogueVariable;
        GameEventsManager.Instance.questEvents.onQuestStateChange += QuestStateChange;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.dialogueEvents.onEnterDialogue -= EnterDialogue;
        GameEventsManager.Instance.inputReader.uiActions.submitEvent -= OnSubmit;
        GameEventsManager.Instance.dialogueEvents.onUpdateChoiceIndex -= UpdateChoiceIndex;
        GameEventsManager.Instance.dialogueEvents.onUpdateInkDialogueVariable -= UpdateInkDialogueVariable;
        GameEventsManager.Instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    private void Start()
    {
        dialoguePlaying = false;
    }

    private void QuestStateChange(Quest quest)
    {
        GameEventsManager.Instance.dialogueEvents.UpdateInkDialogueVariable(
            quest.info.id + "State",
            new StringValue(quest.state.ToString())
        );
    }

    private void UpdateInkDialogueVariable(string name, Ink.Runtime.Object value)
    {
        inkDialogueVariables.UpdateVariableState(name, value);
    }

    private void UpdateChoiceIndex(int choiceIndex)
    {
        this.currentChoiceIndex = choiceIndex;
    }

    private void SubmitPressed(InputEventContext inputEventContext)
    {
        if (!inputEventContext.Equals(InputEventContext.DIALOGUE))
        {
            return;
        }

        ContinueOrExitStory();
    }

    private void EnterDialogue(string knotName)
    { 
        if (dialoguePlaying)
        {
            return;
        }

        dialoguePlaying = true;

        // inform other parts of our system that we've started dialogue
        GameEventsManager.Instance.dialogueEvents.DialogueStarted();

        // freeze player movement
        GameEventsManager.Instance.playerEvents.DisablePlayerMovement();

        // input event context
        GameEventsManager.Instance.inputReader.SwitchActionMap(ActionMap.UI);

        // jump to the knot
        if (!knotName.Equals(""))
        {
            story.ChoosePathString(knotName);
        }
        else
        {
            Debug.LogWarning("Knot name was the empty string when entering dialogue.");
        }

        // start listening for variables
        inkDialogueVariables.SyncVariablesAndStartListening(story);
        inkExternalFunctions.Bind(story);

        // kick off the story
        ContinueOrExitStory();
    }

    private void ContinueOrExitStory()
    {
        // make a choice, if applicable
        if (story.currentChoices.Count > 0 && currentChoiceIndex != -1)
        {
            story.ChooseChoiceIndex(currentChoiceIndex);
            // reset choice index for next time
            currentChoiceIndex = -1;
        }

        if (story.canContinue)
        {
            string dialogueLine = story.Continue();

            // handle the case where there's an empty line of dialogue
            // by continuing until we get a line with content
            while (IsLineBlank(dialogueLine) && story.canContinue)
            {
                dialogueLine = story.Continue();
            }
            // handle the case where the last line of dialogue is blank
            // (empty choice, external function, etc...)
            if (IsLineBlank(dialogueLine) && !story.canContinue)
            {
                StartCoroutine(ExitDialogue());
            }
            else
            {
                GameEventsManager.Instance.dialogueEvents.DisplayDialogue(story, dialogueLine, story.currentChoices);
            }
        }
        else if (story.currentChoices.Count == 0)
        {
            currentChoiceIndex = -1;
            StartCoroutine(ExitDialogue());
        }
    }

    private IEnumerator ExitDialogue()
    {
        yield return new WaitForSeconds(0.2f);
        dialoguePlaying = false;

        // inform other parts of our system that we've finished dialogue
        GameEventsManager.Instance.dialogueEvents.DialogueFinished();

        // let player move again
        GameEventsManager.Instance.playerEvents.EnablePlayerMovement();

        // input event context
        GameEventsManager.Instance.inputReader.SwitchActionMap(ActionMap.Player);

        inkDialogueVariables.StopListening(story);

        inkExternalFunctions.Unbind(story);

        dialoguePlaying = false;

        // reset story state
        story.ResetState();

        dialogueFinish = true;
    }

    public void ExitDialogueAndLoadScene(Loader.Scene sceneToLoad)
    {
        StartCoroutine(ExitDialogue());
        if (dialogueFinish == true)
        {
            Loader.Load(sceneToLoad);
            dialogueFinish = false;
        }
    }

    private bool IsLineBlank(string dialogueLine)
    {
        return dialogueLine.Trim().Equals("") || dialogueLine.Trim().Equals("\n");
    }

    public void OnSubmitPressed(Component sender, object data)
    {
        SubmitPressed((InputEventContext)data);
    }

    private void OnSubmit()
    {
        if (!dialoguePlaying)
        {
            return;
        }
        SubmitPressed(InputEventContext.DIALOGUE);
    }
}
