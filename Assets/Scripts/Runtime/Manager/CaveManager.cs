using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CaveManager : NetworkPersistentSingleton<CaveManager>
{
    public NetworkVariable<int> highestCaveLevel = new NetworkVariable<int>(-1);
    public NetworkVariable<int> highestCaveShape = new NetworkVariable<int>(-1);

    private int _currentLocalCaveLevel = 0;
    public int CurrentLocalCaveLevel 
    {
        get { return _currentLocalCaveLevel; }
        set
        {
            _currentLocalCaveLevel = value;
            _caveLevelText.text = _currentLocalCaveLevel.ToString();
            _caveLevelBox.SetActive(_currentLocalCaveLevel > 0); 
        }
    }
    [SerializeField] private TextMeshProUGUI _caveLevelText;
    [SerializeField] private GameObject _caveLevelBox;
    

    public void GetUIElement()
    {
        _caveLevelBox = GameObject.Find("CaveLevelBox");
        _caveLevelText = _caveLevelBox.GetComponentInChildren<TextMeshProUGUI>();
    }
    
    public void AdjustLocalCaveLevel(int amount)
    {
        CurrentLocalCaveLevel += amount;
    }

    [ServerRpc(RequireOwnership = false)]
    public void CheckAndUpdateHighestLevelServerRpc(int currentCaveLevel, int caveShape)
    {
        if(highestCaveLevel.Value == -1)
        {
            highestCaveLevel.Value = currentCaveLevel;
            highestCaveShape.Value = caveShape;
            return;
        }
            
        if(currentCaveLevel > highestCaveLevel.Value)
        {
            highestCaveLevel.Value = currentCaveLevel;
            highestCaveShape.Value = caveShape;
        }
    }

    
}
