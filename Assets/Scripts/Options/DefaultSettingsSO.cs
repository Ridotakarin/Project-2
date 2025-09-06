using UnityEngine;

[CreateAssetMenu(fileName = "DefaultSettings", menuName = "Game/Default Settings")]
public class DefaultSettingsSO : ScriptableObject
{
    public SettingsData settingsData;

    public void ResetSettingData()
    {
        settingsData = new SettingsData();
    }
}
