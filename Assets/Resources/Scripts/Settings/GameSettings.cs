using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsProperties
{
    public const string UsernameProperty = "Username";
    public const string MasterVolumeProperty = "MasterVolume";
    public const string MusicVolumeProperty = "MusicVolume";
    public const string SFXVolumeProperty = "SFXVolume";
    public const string MouseSensitivityProperty = "MouseSensitivity";
    public const string SelectedPrimaryCarColorIndexProperty = "SelectedPrimaryCarColor";
    public const string SelectedSecondaryCarColorIndexProperty = "SelectedSecondaryCarColor";
    public const string AutoDriftProperty = "AutoDrift";

    public const float DefaultVolume = 1f;
    public const string DefaultUsername = "";
    public const byte DefaultCarColorIndex = 0;
}

public static class GameSettings
{
    public static void LoadSettings()
    {
        PlayerPrefs.GetString(SettingsProperties.UsernameProperty);
        PlayerPrefs.GetFloat(SettingsProperties.MasterVolumeProperty);
        PlayerPrefs.GetFloat(SettingsProperties.MusicVolumeProperty);
        PlayerPrefs.GetFloat(SettingsProperties.SFXVolumeProperty);
        PlayerPrefs.GetFloat(SettingsProperties.MouseSensitivityProperty);
        PlayerPrefs.GetInt(SettingsProperties.SelectedPrimaryCarColorIndexProperty);
        PlayerPrefs.GetInt(SettingsProperties.SelectedSecondaryCarColorIndexProperty);
        PlayerPrefs.GetInt(SettingsProperties.AutoDriftProperty); // 0 = false, 1 = true
        //PlayerPrefs.Save();
    }

    public static void SaveSettings()
    {
        PlayerPrefs.Save();
    }
}