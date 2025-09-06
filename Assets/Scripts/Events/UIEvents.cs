using System;
using UnityEngine;

public class UIEvents
{
    public event Action onOpenLetter;
    public void OpenLetter()
    {
        onOpenLetter?.Invoke();
    }

    public event Action<int> onOpenHelpUI;
    public void OpenHelpUI(int index)
    {
        onOpenHelpUI?.Invoke(index);
    }
}
