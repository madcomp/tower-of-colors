using UnityEngine;
using System.Collections;

public static class SaveData
{
    public static void Clear()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
    
    public static int CurrentLevel
    {
        get {
            return PlayerPrefs.GetInt("CurrentLevel", 1);
        }
        set {
            PlayerPrefs.SetInt("CurrentLevel", value);
            PlayerPrefs.Save();
        }
    }

    public static float PreviousHighscore
    {
        get {
            return PlayerPrefs.GetFloat("PreviousHighscore", 0);
        }
        set {
            PlayerPrefs.SetFloat("PreviousHighscore", value);
            PlayerPrefs.Save();
        }
    }

    public static int CurrentColorList
    {
        get {
            return PlayerPrefs.GetInt("CurrentColorList", 0);
        }
        set {
            PlayerPrefs.SetInt("CurrentColorList", value);
            PlayerPrefs.Save();
        }
    }

    public static int VibrationEnabled
    {
        get {
            return PlayerPrefs.GetInt("VibrationEnabled", 1);
        }
        set {
            PlayerPrefs.SetInt("VibrationEnabled", value);
            PlayerPrefs.Save();
        }
    }
    
    public static string CurrentMissionsData
    {
        get {
            return PlayerPrefs.GetString("CurrentMissionsData", "");
        }
        set {
            PlayerPrefs.SetString("CurrentMissionsData", value);
            PlayerPrefs.Save();
        }
    }
    
    public static string WalletData
    {
        get {
            return PlayerPrefs.GetString("WalletData", "");
        }
        set {
            PlayerPrefs.SetString("WalletData", value);
            PlayerPrefs.Save();
        }
    }
}
