using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRecipePanel : MonoBehaviour
{
    public GameObject recipeBoxPrefab;
    public Transform contentPanel;
    public GameObject recipePanel;
    public Button closeButton;
    public List<Recipe> recipes;

    private void OnEnable()
    {
        GameEventsManager.Instance.craftingEvents.onOpenRecipeBook += OpenRecipePanel;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.craftingEvents.onOpenRecipeBook -= OpenRecipePanel;
    }

    private void Start()
    {
        SetRecieps();
        closeButton.onClick.AddListener(() => recipePanel.SetActive(false));
    }

    private void OpenRecipePanel()
    {
        recipePanel.SetActive(true);
    }

    private void SetRecieps()
    {
        foreach (Recipe recipe in recipes)
        {
            UIRecipeBox recipeBox = Instantiate(recipeBoxPrefab, contentPanel).GetComponent<UIRecipeBox>();
            recipeBox.SetRecipe(recipe);
        }
    }
}
