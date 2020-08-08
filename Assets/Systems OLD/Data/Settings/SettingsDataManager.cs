using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

[System.Serializable]
public class SettingsDataManager
{
    QualitySetting quality;
    ZoomSetting zoom;
    bool music;
    bool sfx;

    public void Login(GetPlayerCombinedInfoResultPayload playerInfo)
    {

        string quality = PlayerPrefs.GetString("Quality");

        if (quality == "high")
        {
            QualitySettings.SetQualityLevel(2, true);
        }
        else if (quality == "low")
        {
            QualitySettings.SetQualityLevel(0, true);
        }
        else
        {
            QualitySettings.SetQualityLevel(1, true);
        }
    }
}

public enum ZoomSetting
{
    close = 0,
    medium = 1,
    far = 2
}


public enum QualitySetting
{
    high = 0,
    medium = 1,
    low = 2
}
