using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI goldText;

    private void OnEnable() 
    {
        GameEventsManager.Instance.goldEvents.onGoldChange += GoldChange;
    }

    private void OnDisable() 
    {
        GameEventsManager.Instance.goldEvents.onGoldChange -= GoldChange;
    }

    private void GoldChange(int gold) 
    {
        goldText.text = gold.ToString();
    }
}
