using System;
using UnityEngine;

public class CraftingEvents
{
    public Action onOpenRecipeBook;
    public void OpenRecipeBook()
    {
        onOpenRecipeBook?.Invoke();
    }
}
