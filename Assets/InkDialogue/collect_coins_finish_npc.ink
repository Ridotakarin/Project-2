=== collectCoinsFinish ===
{ CollectCoinsQuestState:
    - "FINISHED": -> finished
    - else: -> default
}

= finished
Oh! Alex, when did you come back? #speaker:Lyria #portrait:Lyria_Smile #layout:Left #audio:animal_crossing_mid

I just got back. #speaker:Alex #background: Say #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

Do you need something from me? #speaker:Lyria #portrait:Lyria_Smile #layout:Left #audio:animal_crossing_mid

-> END

= default
Hi Alex! Long time no see. Is there any problem? #speaker:Lyria #portrait:Lyria_Smile #layout:Left #audio:animal_crossing_mid 

* [Nothing.]
    -> END
* { CollectCoinsQuestState == "CAN_FINISH" } [Here is some wood you need.]
    ~ FinishQuest(CollectCoinsQuestId)

~ RemoveItem("Wood", 5)

Oh, thank you so much. Sigh, that Josh can't even help with a small thing. #speaker:Lyria #portrait:Lyria_Smile #layout:Left #audio:animal_crossing_mid

Ah, I have some gardening tools that my uncle asked me to send. #speaker:Lyria #portrait:Lyria_Smile #layout:Left #audio:animal_crossing_mid

~ AddItem("Strawberry Seed")
~ AddItem("Potato Seed")
~ AddItem("Carrot Seed")
~ AddItem("WaterCan")
~ AddItem("Hoe")
~ AddItem("Fishing Rod")

Seeds, hoe and watering can. Great, now I can go gardening.#speaker:Alex #background: Say #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
-> END