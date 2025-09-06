using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRecipeBox : MonoBehaviour
{
    public Image itemImg00;
    public Image itemImg01;
    public Image itemImg02;
    public Image itemImg10;
    public Image itemImg11;
    public Image itemImg12;
    public Image itemImg20;
    public Image itemImg21;
    public Image itemImg22;
    public Image itemOutputImg;

    public void SetRecipe(Recipe recipe)
    {
        if (recipe.item_00 != null) itemImg00.sprite = recipe.item_00.image;
        else itemImg00.gameObject.SetActive(false);
        if (recipe.item_01 != null) itemImg01.sprite = recipe.item_01.image;
        else itemImg01.gameObject.SetActive(false);
        if (recipe.item_02 != null) itemImg02.sprite = recipe.item_02.image;
        else itemImg02.gameObject.SetActive(false);
        if (recipe.item_10 != null) itemImg10.sprite = recipe.item_10.image;
        else itemImg10.gameObject.SetActive(false);
        if (recipe.item_11 != null) itemImg11.sprite = recipe.item_11.image;
        else itemImg11.gameObject.SetActive(false);
        if (recipe.item_12 != null) itemImg12.sprite = recipe.item_12.image;
        else itemImg12.gameObject.SetActive(false);
        if (recipe.item_20 != null) itemImg20.sprite = recipe.item_20.image;
        else itemImg20.gameObject.SetActive(false);
        if (recipe.item_21 != null) itemImg21.sprite = recipe.item_21.image;
        else itemImg21.gameObject.SetActive(false);
        if (recipe.item_22 != null) itemImg22.sprite = recipe.item_22.image;
        else itemImg22.gameObject.SetActive(false);
        if (recipe.itemOutput != null) itemOutputImg.sprite = recipe.itemOutput.image;
        else itemOutputImg.gameObject.SetActive(false);
    }
}
