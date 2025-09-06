using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : PersistentSingleton<CursorManager>
{
    [SerializeField] private Texture2D[] cursorTextures;

    private Vector2 cursorHotspot;

    private void ChangeCursor()
    {
        
    }
}