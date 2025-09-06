=== start ===

Haizzz, another busy and tired day. #speaker:Alex #background: Think #portrait:Alex_Think #layout:Right #audio:animal_crossing_mid

I have been dealing with a lot of work every day since I have been living alone for over 2 years now. The overtime is draining me. Sometimes I wonder what is the purpose of my life? #speaker:Alex #background: Think #portrait:Alex_Think #layout:Right #audio:animal_crossing_mid

"~~~ Ping pong" #speaker:??? #background: Say #portrait:Alex_Surprise #layout:Left #audio:animal_crossing_mid

"Delivery here!!!" #speaker:??? #background: Say #portrait:Alex_Surprise #layout:Left #audio:animal_crossing_mid

Hmm? Strange, who sends letters these days? #speaker:Alex #background: Think #portrait:Alex_Surprise2 #layout:Right #audio:animal_crossing_mid

If it's from my parents, they must have contacted me by phone first. Let go check first. #speaker:Alex #background: Think #portrait:Alex_Surprise2 #layout:Right #audio:animal_crossing_mid

-> DONE

=== grandpa_letter ===

~ AddItem("Letter")

Oh! It's a letter from grandpa, let me read it. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

~ AddItem("Key")

There is also an old key inside. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

And also the gardening tools...? #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

-> read_letter

=== read_letter ===

+ [Read the letter carefully.]
   The letter seems important so I need to read it carefully. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
    -> DONE
    
=== prepare_departure ===

~ RemoveItem("Letter", 1)

~ CompletedFirstCutscene()

I miss him too. It seems like my chance to change myself has come. A life in the countryside seems to suit me better than the hustle and bustle of the city. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

The decision has been made, I need to prepare to leave for Eloria village. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

+ [Prepare your luggage carefully] 
   I need to clean up and organize the necessary items. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
    
    -> DONE

+ [Go now, there is no time to lose.]
    Without further ado, let's get started. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

    -> DONE

=== to_eloria ===
~ CompletedSecondCutscene()
What a long tiring trip, must quickly go home to rest.
#speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

-> DONE

=== read_instructions ===

According to the sign, the right side is Eloria village, so the left side must be grandpa's house.
#speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

-> DONE
    
=== eloria_village ===

Oh! How much the countryside has changed since before. I wonder how long it has been since my family left the countryside for the city? #speaker:Alex #background: Think #portrait:Alex_Surprise #layout:Right #audio:animal_crossing_mid

There are two keys, it seems this small key is used to open the house door.. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

-> DONE

=== House_1 ===

This house looks so old. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

I'm too tired to clean up, let's take a nap and then think about it.
#speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid
-> DONE

=== noises_1 ===

Hmm? What is that noise? #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

~ CompletedThirdCutscene()

I have to go check it. #speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

-> DONE

=== noise_2 ===

Haizzz~~~ Why does Lyria need so much wood. Really...... #speaker:??? #background: Say #portrait:Alex_Default #layout:Left #audio:animal_crossing_mid

The voice came from behind the house, I should go check it out.#speaker:Alex #background: Think #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

-> DONE

=== meet_bill ===

Oh! Alex, long time no see. What are you going to ask me to forge for you?  #speaker:Bill #background: Say #portrait:Bill_Smile #layout:Left #audio:animal_crossing_mid

Hi Uncle Bill, I have nothing to do now. #speaker:Alex #background: Say #portrait:Alex_Default #layout:Right #audio:animal_crossing_mid

So? Why don't you go find some ores for me to blacksmith? #speaker:Bill #background: Say #portrait:Bill_Default #layout:Left #audio:animal_crossing_mid

~ AddItem("Pickaxe")

This is a pickaxe to help you dig rocks. Go past the tree on the right and go northeast and you will see a cave. #speaker:Bill #background: Say #portrait:Bill_Default #layout:Left #audio:animal_crossing_mid

~ AddItem("Sword")

Also here's a sword in case you get attacked by animals. #speaker:Bill #background: Say #portrait:Bill_Default #layout:Left #audio:animal_crossing_mid
-> DONE