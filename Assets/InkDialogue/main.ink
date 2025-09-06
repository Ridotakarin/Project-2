// external functions
EXTERNAL StartQuest(questId)
EXTERNAL AdvanceQuest(questId)
EXTERNAL FinishQuest(questId)
EXTERNAL PlayEmote(emoteName)
EXTERNAL AddItem(itemName)
EXTERNAL LoadScene(sceneName)
EXTERNAL ShowHelpUI(indexHelp)
EXTERNAL CompletedFirstCutscene()
EXTERNAL CompletedSecondCutscene()
EXTERNAL CompletedThirdCutscene()
EXTERNAL CompletedFourthCutscene()
EXTERNAL CompletedAllCutscene()
EXTERNAL RemoveItem(itemName, amount)


// quest ids (questId + "Id" for variable name)
VAR CollectCoinsQuestId = "CollectCoinsQuest"

// quest states (questId + "State" for variable name)
VAR CollectCoinsQuestState = "REQUIREMENTS_NOT_MET"

// character states
VAR ItemName = "DEFAULT"

VAR SceneName = "WorldScene"

// ink files
INCLUDE collect_coins_start_npc.ink
INCLUDE collect_coins_finish_npc.ink
INCLUDE Part1_Start.ink
INCLUDE Part1_eloria.ink