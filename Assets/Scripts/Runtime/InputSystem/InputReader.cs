using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public enum ActionMap
{
    Player,
    UI
}

[CreateAssetMenu]
public class InputReader : ScriptableObject
{
    private Controls input;
    public PlayerActions playerActions;
    public UIActions uiActions;

    private void OnEnable()
    {
        playerActions = new PlayerActions();
        uiActions = new UIActions();
        if (input == null)
        {
            input = new Controls();
            input.Player.SetCallbacks(playerActions);
        }
        input.Player.Enable();
    }

    private void OnDisable()
    {
        if (input != null)
        {
            input.Player.Disable();
            input.UI.Disable();
        }
            
    }

    public void EnableControl()
    {
        if (input != null)
            input.Enable();
    }

    public void DisableControl()
    {
        if (input != null)
            input.Disable();
    }

    public void SwitchActionMap(ActionMap map)
    {
        if (input == null) return;
        input.Disable(); // Disable all action maps first
        input.Player.SetCallbacks(null);
        input.UI.SetCallbacks(null); // Clear callbacks for all action maps
        switch (map)
        {
            case ActionMap.Player:
                input.Player.SetCallbacks(playerActions);
                input.Player.Enable();
                break;

            case ActionMap.UI:
                input.UI.SetCallbacks(uiActions);
                input.UI.Enable(); 
                break;
        }
        Debug.Log("switched action map to: " + map.ToString());
    }

}
