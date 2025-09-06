using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Recipe")]
public class Recipe : ScriptableObject
{
    public int Quantity = 1;

    public Item itemOutput;

    [Header("Item 0")]
    public Item item_00;
    [Header("Item 1")]
    public Item item_01;
    [Header("Item 2")]
    public Item item_02;
    [Header("Item 3")]
    public Item item_10;
    [Header("Item 4")]
    public Item item_11;
    [Header("Item 5")]
    public Item item_12;
    [Header("Item 6")]
    public Item item_20;
    [Header("Item 7")]
    public Item item_21;
    [Header("Item 8")]
    public Item item_22;

    public Item GetItem(int i, int j)
    {
        if (i == 0 && j == 0) return item_00;
        if (i == 1 && j == 0) return item_10;
        if (i == 2 && j == 0) return item_20;

        if (i == 0 && j == 1) return item_01;
        if (i == 1 && j == 1) return item_11;
        if (i == 2 && j == 1) return item_21;

        if (i == 0 && j == 2) return item_02;
        if (i == 1 && j == 2) return item_12;
        if (i == 2 && j == 2) return item_22;

        return null;
    }    
}
