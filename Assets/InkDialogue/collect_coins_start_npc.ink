=== collectCoinsStart ===
{ CollectCoinsQuestState :
    - "REQUIREMENTS_NOT_MET": -> requirementsNotMet
    - "CAN_START": -> canStart
    - "IN_PROGRESS": -> inProgress
    - "CAN_FINISH": -> canFinish
    - "FINISHED": -> finished
    - else: -> END
}

= requirementsNotMet
Come back here when you're done it.  #speaker:Josh #portrait:Josh_Default #layout:Right #audio:animal_crossing_mid
-> END

= canStart
Hey mister. #speaker:Alex #background: Say #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
Oh, wait? It's Josh. #speaker:Alex #background: Say #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
Oh! Alex? #speaker:Josh #background: Say #portrait:Josh_Smile #layout:Left #audio:animal_crossing_mid
When did you back? Why don't you tell me soon?. #speaker:Josh #background: Say #portrait:Josh_Default #layout:Left #audio:animal_crossing_mid
I just arrived. How are you now? #speaker:Josh #background: Say #portrait:Alex_Default #layout:Left #audio:animal_crossing_mid
Now I'm fine, so you, take this axe and help me, quickly.#speaker:Josh #background: Say #portrait:Josh_Default #layout:Left #audio:animal_crossing_mid

~ AddItem("Axe")

C'mon Josh. I just arrived, give me a break please. #speaker:Josh #background: Say #portrait:Alex_Default #layout:Left #audio:animal_crossing_mid

 ~ StartQuest(CollectCoinsQuestId)
 
Stop whining. By the way, if you complete this, Lyria will have and reward for you.#speaker:Josh #background: Say #portrait:Josh_Default #layout:Left #audio:animal_crossing_mid
- -> END

= inProgress
Have you finished yet? #speaker:Josh #portrait:Josh_Default #layout:Left #audio:animal_crossing_mid
-> END

= canFinish
You've done it? Wow, that was faster than i though. #speaker:Josh #portrait:Josh_Smile #layout:Left #audio:animal_crossing_mid
Bring these to Lyria, she will have something to give you. #speaker:Josh #portrait:Josh_Smile #layout:Left #audio:animal_crossing_mid
Go right to the village and you will meet her. #speaker:Josh #portrait:Josh_Smile #layout:Left #audio:animal_crossing_mid
-> END

= finished
Have a hard working day right, Alex? #speaker:Josh #portrait:Josh_Default #layout:Left #audio:animal_crossing_mid

-> END