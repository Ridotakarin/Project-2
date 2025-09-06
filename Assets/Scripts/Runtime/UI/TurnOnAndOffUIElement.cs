using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnAndOffUIElement : MonoBehaviour
{
    CanvasGroup group;
    void Start()
    {
        group = GetComponent<CanvasGroup>();
        TurnOff();
    }

    public void TurnOn()
    {
        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
    }
    public void TurnOff()
    {
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }
}
