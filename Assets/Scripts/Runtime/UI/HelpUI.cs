using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpUI : MonoBehaviour
{
    [SerializeField] private GameObject helpPanel;
    [SerializeField] private List<Sprite> helpImages;
    [SerializeField] private Image helpImage;
    [SerializeField] private Button backButton;

    private void OnEnable()
    {
        GameEventsManager.Instance.uiEvents.onOpenHelpUI += OnHelpUIOpened;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.uiEvents.onOpenHelpUI -= OnHelpUIOpened;
    }

    private void Start()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(() => helpPanel.SetActive(false));
        }
        else
        {
            Debug.LogWarning("Back button is not assigned in HelpUI.");
        }

        helpPanel.SetActive(false); 
    }

    private void OnHelpUIOpened(int index)
    {
        if (index < 0 || index >= helpImages.Count)
        {
            Debug.LogError("Invalid help index: " + index);
            return;
        }

        helpImage.sprite = helpImages[index];
        helpPanel.gameObject.SetActive(true);
    }

    public void CloseHelpPanel()
    {
        GameEventsManager.Instance.inputReader.EnableControl();
        helpPanel.SetActive(false);
    }
}
