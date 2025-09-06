using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : Controls.IPlayerActions
{
    public Action<Vector2> moveEvent;
    public Action attackEvent;
    public Action interactEvent;
    public Action secondInteractEvent;
    public Action<InputAction.CallbackContext> runEvent;
    public Action openInventoryEvent;
    public Action submitEvent;
    public Action onTestEvent;
    public Action onQuestEvent;

    public Action<int, bool> changeInventorySlotEvent;
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) attackEvent?.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) interactEvent?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveEvent?.Invoke(context.ReadValue<Vector2>());
        
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        runEvent?.Invoke(context);

    }

    public void OnSecondInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) secondInteractEvent?.Invoke();
    }


    public void OnChangeInventorySlotByButton(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && context.control.device is Keyboard)
        {
            if (int.TryParse(context.control.name, out int inputNumber))
                changeInventorySlotEvent?.Invoke(inputNumber - 1, true);
        }
        else if (context.phase == InputActionPhase.Started && context.control.device is Gamepad)
        {
            if (context.control.name == "Left Shoulder")
            {
                changeInventorySlotEvent?.Invoke(-1, false);
            }
            if (context.control.name == "Right Shoulder")
            {
                changeInventorySlotEvent?.Invoke(1, false);
            }
        }
    }

    public void OnChangeInventorySlotByMouseWheel(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            float scrollValue = context.ReadValue<float>();
            if (scrollValue > 0)
                changeInventorySlotEvent?.Invoke(-1, false);
            else if (scrollValue < 0)
                changeInventorySlotEvent?.Invoke(1, false);
        }

    }

    public void OnOpenInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) openInventoryEvent?.Invoke();
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) submitEvent?.Invoke();
    }

    public void OnTest(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            onTestEvent?.Invoke();
        }
    }

    public void OnQuest(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            onQuestEvent?.Invoke();
        }
    }
}
