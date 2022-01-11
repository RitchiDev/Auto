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
    public const string AutoDriftProperty = "AutoDrift";

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
        PlayerPrefs.GetInt(SettingsProperties.AutoDriftProperty); // 0 = false, 1 = true
    }

    public static void SaveSettings()
    {
        PlayerPrefs.Save();
    }
}