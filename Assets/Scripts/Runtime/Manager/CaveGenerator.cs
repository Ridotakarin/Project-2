using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using static UnityEditor.PlayerSettings;
#endif

public static class CaveShapes
{
    public static int[][,] caves = new int[][,]
    {
        new int[,] {
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,1,4,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,4,1,2,2,2,2,1,1,1,1,1,1,1,1,0,0},
            {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,1,1,1,1,1,0,0},
            {0,0,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,0,0,0,0,0,0,0,0,1,1,0,0},
            {0,0,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,0,0},
            {0,0,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,2,2,2,2,1,1,1,1,1,0,0,0,0,0,0,0,0,1,4,0,0},
            {0,0,4,1,0,0,0,0,0,0,0,0,4,1,1,1,1,1,1,2,2,2,2,2,1,1,1,1,0,0,0,0,0,0,0,0,1,1,0,0},
            {0,0,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,4,0,0,0,0,0,0,0,0,1,1,0,0},
            {0,0,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,2,2,1,1,1,1,0,0,0,0,0,0,0,0,1,1,0,0},
            {0,0,1,1,1,1,1,1,1,1,1,1,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
            {0,0,1,1,1,1,1,1,1,1,1,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
            {0,0,1,1,1,1,1,1,1,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,2,2,2,2,1,1,1,0,0},
            {0,0,1,1,1,1,1,1,1,2,2,2,2,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,2,2,2,2,2,1,1,1,0,0},
            {0,0,4,1,1,1,1,1,2,2,2,2,2,4,0,0,0,0,0,0,0,0,0,0,0,0,4,1,1,1,2,2,2,2,1,1,1,4,0,0},
            {0,0,1,2,2,1,1,1,2,2,2,2,2,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,2,2,1,1,1,1,0,0},
            {0,0,1,2,2,2,2,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,2,2,1,1,1,1,0,0},
            {0,0,1,2,2,2,2,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,2,2,1,1,1,1,0,0},
            {0,0,1,1,1,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
            {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
            {0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0},
            {0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0},
            {0,0,4,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,4,0,0},
            {0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0},
            {0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0},
            {0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0},
            {0,0,1,1,1,1,1,1,1,4,2,2,2,2,1,1,1,1,1,1,4,1,2,2,2,2,1,1,1,1,4,1,1,1,1,1,1,1,0,0},
            {0,0,1,1,1,1,1,1,1,1,1,2,2,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,1,1,1,1,1,1,1,1,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}
        },

        new int[,] {
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,0,0},
            {0,0,1,4,1,0,0,0,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,0,0,1,1,0,0},
            {0,0,1,1,1,0,0,0,1,1,1,1,2,2,2,2,1,1,1,1,1,1,1,1,1,1,2,2,2,1,1,1,1,1,0,0,1,1,0,0},
            {0,0,1,1,1,0,0,0,0,0,1,1,2,2,2,2,2,1,1,1,1,1,1,1,1,1,2,2,2,2,2,1,1,1,0,0,1,1,0,0},
            {0,0,1,2,2,1,0,0,0,0,1,1,1,2,2,2,2,1,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,0,0,1,4,0,0},
            {0,0,1,2,2,2,1,1,0,0,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,2,2,0,0,0,1,0,0,1,1,0,0},
            {0,0,1,1,2,2,2,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,0,0,0,1,0,0,1,1,0,0},
            {0,0,1,2,2,2,2,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,1,0,0,1,1,0,0},
            {0,0,1,2,2,2,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,1,1,0,0},
            {0,0,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,0,0},
            {0,0,4,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
            {0,0,1,1,1,0,0,0,1,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,4,0,0},
            {0,0,1,1,1,0,0,1,1,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,2,2,2,1,1,1,1,1,0,0},
            {0,0,1,1,1,0,0,1,2,2,2,2,0,0,0,0,1,1,1,1,4,1,1,1,1,0,0,0,1,1,2,2,2,2,2,1,1,1,0,0},
            {0,0,1,1,1,1,1,1,2,2,2,2,0,0,0,0,1,1,1,2,2,2,1,1,1,1,0,0,1,1,1,2,2,2,2,1,1,1,0,0},
            {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,1,1,2,2,0,0,1,1,1,1,1,2,2,1,1,1,0,0},
            {0,0,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,2,2,2,2,2,2,2,1,1,1,1,1,2,2,2,2,1,1,1,0,0},
            {0,0,1,1,1,0,0,1,1,0,0,0,1,1,1,1,1,1,1,1,2,2,2,2,2,2,1,1,1,1,1,2,2,2,0,0,1,1,0,0},
            {0,0,4,1,1,0,0,0,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,0,0},
            {0,0,1,1,1,0,0,0,0,1,1,1,1,0,0,0,0,1,1,1,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,1,4,0,0},
            {0,0,1,1,1,0,0,0,0,1,1,1,1,0,0,0,0,1,1,1,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,1,1,0,0},
            {0,0,1,1,1,1,1,0,0,0,0,1,1,1,4,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,4,1,1,1,1,1,0,0},
            {0,0,1,2,2,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,1,1,1,0,0},
            {0,0,1,2,2,2,2,1,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,1,1,1,0,0},
            {0,0,1,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,1,1,1,2,2,2,2,1,1,1,0,0},
            {0,0,1,1,1,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2,2,2,1,1,1,0,0},
            {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,1,1,1,1,1,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}
        },

        new int[,] {
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,1,1,1,4,1,1,1,1,1,1,1,2,2,2,4,1,1,1,1,1,1,1,1,4,1,0,0},
            {0,0,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,1,1,1,1,1,1,1,1,1,1,1,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,1,1,1,1,4,1,1,1,2,2,2,2,2,1,1,1,1,1,1,1,4,1,1,1,1,0,0},
            {0,0,1,1,1,1,1,1,1,1,1,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,0,0},
            {0,0,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,1,1,2,2,2,2,2,2,0,0},
            {0,0,1,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,0,0},
            {0,0,4,2,2,2,2,2,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,4,0,0},
            {0,0,1,2,2,2,2,2,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0},
            {0,0,1,1,2,2,1,1,1,1,1,1,0,0,0,0,1,1,1,4,1,1,1,1,1,1,1,1,0,0},
            {0,0,1,1,1,1,1,1,1,1,1,2,2,1,0,0,0,1,1,1,1,1,1,2,2,2,1,1,0,0},
            {0,0,1,1,1,1,0,0,1,2,2,2,2,1,1,0,0,1,1,1,1,1,1,2,2,2,1,1,0,0},
            {0,0,1,1,1,1,0,0,1,2,2,2,2,1,1,1,1,1,1,1,1,2,2,2,2,2,1,1,0,0},
            {0,0,1,1,1,4,0,0,0,2,2,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,1,4,0,0},
            {0,0,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
            {0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,0,0,1,1,4,1,0,0},
            {0,0,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,0,0},
            {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,2,2,1,1,1,0,0,0,1,1,1,1,1,0,0},
            {0,0,1,1,1,1,1,4,1,1,0,0,0,0,1,2,2,2,2,2,1,4,1,1,1,1,1,1,0,0},
            {0,0,1,1,1,2,2,1,1,1,1,1,0,0,1,2,2,2,2,2,1,1,1,1,1,1,1,1,0,0},
            {0,0,1,1,1,2,2,2,2,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,1,1,1,0,0},
            {0,0,4,1,2,2,2,2,2,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,1,1,1,0,0},
            {0,0,1,1,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}
        },
        new int[,] {
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,1,1,1,0,0,0,0,0,1,1,4,1,1,0,0,0,0,0,1,4,1,0,0,0,0,0,0},
            {0,0,0,4,1,1,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,1,1,1,0,0,0,0,0,0},
            {0,0,0,1,1,2,2,1,0,0,0,1,1,0,0,2,2,0,0,0,1,1,1,1,1,0,0,0,0,0},
            {0,0,0,1,1,2,2,2,0,0,0,1,1,0,0,2,2,0,0,0,1,1,1,1,1,0,0,0,0,0},
            {0,0,0,0,0,2,2,2,2,1,1,1,1,2,2,2,2,1,1,1,1,1,1,0,0,0,0,0,0,0},
            {0,0,0,0,0,1,2,2,2,1,1,1,1,2,2,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0},
            {0,0,0,0,0,1,1,0,0,0,1,1,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0},
            {0,0,0,0,0,1,1,0,0,0,1,1,0,0,0,0,0,0,0,1,2,2,1,0,0,0,0,0,0,0},
            {0,0,0,1,1,1,1,1,4,1,1,1,1,1,1,4,1,1,1,1,2,2,2,1,1,4,0,0,0,0},
            {0,0,0,1,1,1,1,1,2,2,1,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,0,0,0,0},
            {0,0,0,1,1,0,0,0,2,2,0,0,0,0,0,0,0,1,1,0,0,0,2,2,0,0,0,0,0,0},
            {0,0,0,1,1,0,0,0,2,2,0,0,0,0,0,0,0,1,1,0,0,0,2,2,0,0,0,0,0,0},
            {0,0,0,4,1,1,2,2,2,2,1,1,1,1,4,1,1,1,1,1,4,1,1,1,1,1,0,0,0,0},
            {0,0,0,1,1,1,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}
        },
    };
}


public class CaveGenerator : NetworkSingleton<CaveGenerator>
{
    [SerializeField]
    private int wallThickness;

    public List<Tilemap> tilemaps = new List<Tilemap>();
    public List<TileBase> tileBases = new List<TileBase>();
    public enum TilemapName
    {
        Ground,
        MineGround,
        Wall,
    }
    public enum TilebaseName
    {
        Ground,
        MineGround2,
        MineWall
    }
    [SerializeField]
    private GameObject _stairUpPrefab;
    [SerializeField]
    private GameObject _stairDownPrefab;

    // Objects
    [SerializeField]
    private GameObject _torchPrefab;
    List<Vector3Int> torchPositions = new List<Vector3Int>();

    [SerializeField]
    private List<GameObject> _enemys = new List<GameObject>();
    [SerializeField]
    List<Vector3Int> _groundTilePositions = new List<Vector3Int>();

    // Spawn Objects Parent
    public Transform MineralsParent;

    #region Ratios
    // Minerals
    [SerializeField]
    [Range(0, 100)]
    private float _stoneAndMineralFillPercent;

    [SerializeField]
    [Range(0, 100)]
    private float _mineralFillPercent;

    [SerializeField]
    [Range(0, 100)]
    private float _stairAppearRatio;
    #endregion

    #region Minerals infos
    [SerializeField]
    private GameObject[] _stonesAndMineralsPrefab;
    [SerializeField]
    private float[] _stonesAndMineralsPrefabChoosePercent;

    [SerializeField]
    private ItemDropableEntitySO[] _stonesInfo;


    [SerializeField]
    private ItemDropableEntitySO[] _smallMineralFromLevel1;

    [SerializeField]
    private ItemDropableEntitySO[] _bigMineralFromLevel1;

    [SerializeField]
    private ItemDropableEntitySO[] _smallMineralFromLevel10;

    [SerializeField]
    private ItemDropableEntitySO[] _bigMineralFromLevel10;

    [SerializeField]
    private ItemDropableEntitySO[] _smallMineralFromLevel20;

    [SerializeField]
    private ItemDropableEntitySO[] _bigMineralFromLevel20;

    [SerializeField]
    private ItemDropableEntitySO[] _smallMineralFromLevel30;

    [SerializeField]
    private ItemDropableEntitySO[] _bigMineralFromLevel30;

    private ItemDropableEntitySO[] _chosenSmallMineralInfoArray;
    private ItemDropableEntitySO[] _chosenBigMineralInfoArray;
    #endregion

    private bool _isHavingOtherPlayerInCave = false;


    protected override void Awake()
    {
        base.Awake();
        CaveManager.Instance.GetUIElement();
        CaveManager.Instance.AdjustLocalCaveLevel(1);
        AdjustMineralsRatio();
        ChooseMineralToSpawn();
    }

    public override void OnNetworkSpawn()
    {
        StartCoroutine(WaitForCaveSceneLoaded());
    }

    private IEnumerator WaitForCaveSceneLoaded()
    {
        yield return new WaitUntil(() => SceneManagement.GetCurrentSceneName() == Loader.Scene.MineScene.ToString());
        PlayerController.LocalInstance.GetComponent<PlayerRoomController>().UpdateRoom(new RoomId { Type = RoomType.Cave, Id = CaveManager.Instance.CurrentLocalCaveLevel });
        RoomManager.Instance.RefreshCheckOnAllVisibilityObjectsServerRpc();
        GetSpawnParentForObjects();
        DrawSelectedCave();
        AddInteriorForCaveServerRpc();

    }
    private void GetSpawnParentForObjects()
    {
        MineralsParent = GameObject.Find("MineralsParent").GetComponent<Transform>();
    }
    private Tilemap GetTilemapByName(string tilemapName)
    {
        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.name == tilemapName)
            {
                return tilemap;
            }
        }
        return null;
    }

    private TileBase GetTileBaseByName(string tilebaseName)
    {
        foreach (TileBase tileBase in tileBases)
        {
            if (tileBase.name == tilebaseName)
            {
                return tileBase;
            }
        }
        return null;

    }

    private void ChooseMineralToSpawn()
    {
        if (CaveManager.Instance.CurrentLocalCaveLevel < 10)
        {
            _chosenSmallMineralInfoArray = _smallMineralFromLevel1;
            _chosenBigMineralInfoArray = _bigMineralFromLevel1;
        }
        else if (CaveManager.Instance.CurrentLocalCaveLevel < 20)
        {
            _chosenSmallMineralInfoArray = _smallMineralFromLevel10;
            _chosenBigMineralInfoArray = _bigMineralFromLevel10;
        }
        else if (CaveManager.Instance.CurrentLocalCaveLevel < 30)
        {
            _chosenSmallMineralInfoArray = _smallMineralFromLevel20;
            _chosenBigMineralInfoArray = _bigMineralFromLevel20;
        }
        else if (CaveManager.Instance.CurrentLocalCaveLevel < 40)
        {
            _chosenSmallMineralInfoArray = _smallMineralFromLevel30;
            _chosenBigMineralInfoArray = _bigMineralFromLevel30;
        }
    }
    void DrawSelectedCave()
    {
        //int caveNumber = 0;
        //bool isInHighestCaveLevel = CaveManager.Instance.highestCaveLevel.Value == CaveManager.Instance.CurrentLocalCaveLevel;
        //isInHighestCaveLevel = false; // temp fix for single player
        //if (!isInHighestCaveLevel)
        //{
        //    _isHavingOtherPlayerInCave = false;
        //    caveNumber = Random.Range(0, CaveShapes.caves.Length);
        //}
        //else
        //    caveNumber = CaveManager.Instance.highestCaveLevel.Value;
        int caveNumber = Random.Range(0, CaveShapes.caves.Length);

        //CaveManager.Instance.CheckAndUpdateHighestLevelServerRpc(CaveManager.Instance.CurrentLocalCaveLevel, caveNumber);

        int[,] selected = CaveShapes.caves[caveNumber];
        int[,] padded = AddWallPadding(selected, wallThickness);

        int height = padded.GetLength(0); // rows = Y
        int width = padded.GetLength(1);  // columns = X

        /*
         * make if no one in this cave then create tile and object in it
         * if someone in this cave then just update tile for that client only
         * 
         */
        //if (!_isHavingOtherPlayerInCave)
            ModifyTileServerRpc(null, null, new NetworkVector3Int(Vector3Int.zero), true); // clear all tile first

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3Int pos = new Vector3Int(x, height - 1 - y, 0);

                int val = padded[y, x];

                CreateAllTileAndObject(val, padded, x, y, pos, _isHavingOtherPlayerInCave);
            }
        }
    }

    private void CreateAllTileAndObject(int val, int[,] padded,int x,int y,Vector3Int pos, bool isHavingPlayerInCave)
    {
        NetworkVector3Int networkPos = new NetworkVector3Int(pos);
        switch (val)
        {
            case 0: // wall
                ModifyTileServerRpc(TilemapName.Wall.ToString(), TilebaseName.MineWall.ToString(), networkPos, false, PlayerController.LocalInstance.OwnerClientId, true);
                if (HasAdjacentGround(padded, y, x)) // also update this
                    ModifyTileServerRpc(TilemapName.Ground.ToString(), TilebaseName.Ground.ToString(), networkPos, false, PlayerController.LocalInstance.OwnerClientId, true);
                break;
            case 1: // ground
                ModifyTileServerRpc(TilemapName.Ground.ToString(), TilebaseName.Ground.ToString(), networkPos, false, PlayerController.LocalInstance.OwnerClientId, true);
                if (!isHavingPlayerInCave)
                {
                    _groundTilePositions.Add(pos); // store for later enemy
                    CheckAndPlaceMineralsServerRpc(pos);
                }
                break;
            case 2: // mine ground
                ModifyTileServerRpc(TilemapName.Ground.ToString(), TilebaseName.Ground.ToString(), networkPos, false, PlayerController.LocalInstance.OwnerClientId, true);
                ModifyTileServerRpc(TilemapName.MineGround.ToString(), TilebaseName.MineGround2.ToString(), networkPos, false, PlayerController.LocalInstance.OwnerClientId, true);
                break;
            case 3: // stair up

                ModifyTileServerRpc(TilemapName.Wall.ToString(), TilebaseName.MineWall.ToString(), networkPos, false, PlayerController.LocalInstance.OwnerClientId, true);
                ModifyTileServerRpc(TilemapName.Ground.ToString(), TilebaseName.Ground.ToString(), networkPos, false, PlayerController.LocalInstance.OwnerClientId, true);
                if (!isHavingPlayerInCave)
                {
                    CreateStairServerRpc(pos);
                }
                break;
            case 4: // torch
                ModifyTileServerRpc(TilemapName.Ground.ToString(), TilebaseName.Ground.ToString(), networkPos, false, PlayerController.LocalInstance.OwnerClientId, true);
                if (!isHavingPlayerInCave)
                    torchPositions.Add(pos); // store for later torch placement
                break;
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void CreateStairServerRpc(Vector3Int pos)
    {
        GameObject stairObject = Instantiate(_stairUpPrefab, GetTilemapByName(TilemapName.Wall.ToString()).GetCellCenterWorld(pos), Quaternion.identity);
        stairObject.GetComponent<NetworkObject>().Spawn(true);
        stairObject.GetComponent<NetworkVisibilityRoom>().InitializeRoomInfo(CaveManager.Instance.CurrentLocalCaveLevel);

    }

    [ServerRpc(RequireOwnership = false)]
    private void ModifyTileServerRpc(string targetTilemapName, string tileName, NetworkVector3Int netPos, bool clearAllTile = false, ulong targetClientId = 0, bool isClientOnly = false)
    {
        if (isClientOnly)
        {
            var clientParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new List<ulong> { targetClientId }
                }
            };
            ApplyTileClientRpc(targetTilemapName, tileName, netPos, clearAllTile, clientParams);
        }
        else
        {
            ApplyTileClientRpc(targetTilemapName, tileName, netPos, clearAllTile);
        }
    }

    [ClientRpc]
    private void ApplyTileClientRpc(string targetTilemapName, string tileName, NetworkVector3Int netPos, bool clearAllTile, ClientRpcParams clientRpcParams = default)
    {
        if (clearAllTile)
        {
            foreach (Tilemap tilemap in tilemaps)
                tilemap.ClearAllTiles();
            return;
        }

        Vector3Int pos = netPos.ToVector3Int();
        Tilemap targetTilemap = GetTilemapByName(targetTilemapName);
        TileBase tile = GetTileBaseByName(tileName);

        targetTilemap.SetTile(pos, tile);

    }
    private void AdjustMineralsRatio()
    {
        int CurrentLevel = CaveManager.Instance.CurrentLocalCaveLevel;
        if (CurrentLevel % 5 == 0)
        {
            int amount = 2;
            _mineralFillPercent += amount;

            if (_mineralFillPercent > 100)
                _mineralFillPercent = 100;

            _stonesAndMineralsPrefabChoosePercent[0] -= amount / 2;
            _stonesAndMineralsPrefabChoosePercent[1] += amount / 2;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddInteriorForCaveServerRpc()
    {
        PlaceTorches();
        AddEnemy();
    }

    private void PlaceTorches()
    {
        foreach (var pos in torchPositions)
        {
            GameObject torch = Instantiate(_torchPrefab, GetTilemapByName(TilemapName.Ground.ToString()).GetCellCenterWorld(pos), Quaternion.identity);
            torch.GetComponent<NetworkObject>().Spawn(true);
            torch.GetComponent<NetworkVisibilityRoom>().InitializeRoomInfo(CaveManager.Instance.CurrentLocalCaveLevel);
            Vector3Int[] directionToWall = new Vector3Int[]
            {
                Vector3Int.left,
                Vector3Int.right,
                Vector3Int.up
            };

            foreach (var direction in directionToWall)
            {

                var wallPos = pos + direction;
                if (GetTilemapByName(TilemapName.Wall.ToString()).GetTile(wallPos) != null)
                {

                    SetTorchPositionClientRpc(new NetworkVector3Int(direction), torch.GetComponent<NetworkObject>());
                    
                    break;
                }


            }
        }
    }

    private void AddEnemy()
    {
        for(int i = 0; i < 4; i++)
        {
            Vector3Int groundPos = _groundTilePositions[Random.Range(0, _groundTilePositions.Count)];
            Vector3 groundWorldPos = GetTilemapByName(TilemapName.Ground.ToString()).CellToWorld(groundPos);
            Vector3 enemyPos = groundWorldPos + new Vector3(0.5f, 0.5f, 0); // Offset to center the enemy on the tile
            GameObject enemy = Instantiate(_enemys[Random.Range(0, _enemys.Count)], enemyPos, Quaternion.identity);
            enemy.GetComponent<NetworkObject>().Spawn(true);
            enemy.GetComponent<NetworkVisibilityRoom>().InitializeRoomInfo(CaveManager.Instance.CurrentLocalCaveLevel);
        }
    }

    [ClientRpc]
    private void SetTorchPositionClientRpc(NetworkVector3Int netDirection, NetworkObjectReference torchRef)
    {
        var direction = netDirection.ToVector3Int();
        torchRef.TryGet(out NetworkObject torch);
        if (direction == Vector3Int.left)
        {

            torch.transform.position += Vector3.left / 2;
            torch.GetComponent<Animator>().Play("Right");
        }
        else if (direction == Vector3Int.right)
        {
            torch.transform.position += Vector3.right / 2;
            torch.GetComponent<Animator>().Play("Left");
        }
        else // Up
        {
            torch.transform.position += Vector3.up * 1.1f;
            torch.GetComponent<Animator>().Play("Front");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckAndPlaceMineralsServerRpc(Vector3Int pos)
    {
        if (Random.Range(0, 100) > _stoneAndMineralFillPercent) return;
        GameObject stoneAndMineralPrefab = UtilsClass.PickOneByRatio(_stonesAndMineralsPrefab, _stonesAndMineralsPrefabChoosePercent);
        Vector3 worldPos = GetTilemapByName(TilemapName.Ground.ToString()).CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0f);
        GameObject mineralObject = Instantiate(stoneAndMineralPrefab, worldPos, Quaternion.identity, MineralsParent);
        mineralObject.GetComponent<NetworkObject>().Spawn();
        mineralObject.GetComponent<NetworkVisibilityRoom>().InitializeRoomInfo(CaveManager.Instance.CurrentLocalCaveLevel);

        StoneAndMineral mineral = mineralObject.GetComponent<StoneAndMineral>();
        if (mineral.stoneAndMineralType == StoneAndMineral.StoneAndMineralType.Small)
        {
            if (Random.Range(0, 100) < _mineralFillPercent) // true = mineral, false = stone
            {
                ItemDropableEntitySO smallMineralInfo = _chosenSmallMineralInfoArray[Random.Range(0, _chosenSmallMineralInfoArray.Length)];
                mineral.InitializeMineralClientRpc(smallMineralInfo.id);
            }
            else
            {

                ItemDropableEntitySO smallStoneInfo = _stonesInfo[Random.Range(0, _stonesInfo.Length)];
                mineral.InitializeMineralClientRpc(smallStoneInfo.id);
            }
        }
        else
        {
            if (Random.Range(0, 100) < _mineralFillPercent)
            {
                ItemDropableEntitySO bigMineralInfo = _chosenBigMineralInfoArray[Random.Range(0, _chosenBigMineralInfoArray.Length)];
                mineral.InitializeMineralClientRpc(bigMineralInfo.id);
            }
            else
            {
                mineralObject.GetComponent<NetworkObject>().Despawn(true);
            }
        }
    }
    bool HasAdjacentGround(int[,] map, int y, int x)
    {
        int height = map.GetLength(0); // rows
        int width = map.GetLength(1);  // columns

        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0) continue;
                int ny = y + dy;
                int nx = x + dx;
                if (ny >= 0 && ny < height && nx >= 0 && nx < width)
                {
                    if (map[ny, nx] == 1 || map[ny, nx] == 2)
                        return true;
                }
            }
        }

        return false;
    }
    int[,] AddWallPadding(int[,] originalMap, int padding)
    {
        int ow = originalMap.GetLength(0);
        int oh = originalMap.GetLength(1);
        int nw = ow + padding * 2;
        int nh = oh + padding * 2;

        int[,] padded = new int[nw, nh];

        for (int x = 0; x < nw; x++)
            for (int y = 0; y < nh; y++)
                padded[x, y] = 0; // default wall

        for (int x = 0; x < ow; x++)
            for (int y = 0; y < oh; y++)
                padded[x + padding, y + padding] = originalMap[x, y];

        return padded;
    }

    public void OnMineralDestroy(Component sender, object data)
    {
        if (!IsServer) return; // Ensure this only runs on the server
        if (Random.Range(0, 100) < _stairAppearRatio)
        {
            Vector2 mineralPosition = sender.transform.position;
            GameObject stair = Instantiate(_stairDownPrefab, mineralPosition, Quaternion.identity);
            var stairNetObj = stair.GetComponent<NetworkObject>();
            stairNetObj.Spawn();
        }
        else
        {
            _stairAppearRatio++;
        }

        sender.GetComponent<NetworkObject>().Despawn(true);
    }
}




