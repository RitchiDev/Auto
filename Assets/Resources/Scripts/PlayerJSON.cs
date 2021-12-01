using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class JsonManager : MonoBehaviour
{
    public static JsonManager m_singleton;
    public string m_GamePath;
    public string PlayerJsonFileName = "ScoreList";

    private void Awake()
    {
        m_singleton = this;
        m_GamePath = Application.dataPath;
    }
}
public class PlayerJSON : MonoBehaviour
{
    public List<PlayerData> GetOldScores()
    {
        List<PlayerData> Data = new List<PlayerData>();
        if (File.Exists(JsonManager.m_singleton.m_GamePath + JsonManager.m_singleton.PlayerJsonFileName))
        {
            string[] lines = File.ReadAllLines(JsonManager.m_singleton.m_GamePath + JsonManager.m_singleton.PlayerJsonFileName);
            foreach (string item in lines)
            {
                Data.Add(JsonUtility.FromJson<PlayerData>(item));
            }   
        }
        return Data;
    }
    public void WriteNewScore(PlayerData Data)
    {
        string txt = JsonUtility.ToJson(Data);
        File.AppendAllText(JsonManager.m_singleton.m_GamePath + JsonManager.m_singleton.PlayerJsonFileName, txt);
    }
}
[Serializable]
public class PlayerData
{
    public PlayerData(string naam, int playerscore)
    {
        PlayerNaam = naam;
        Score = playerscore;
    }
    public string PlayerNaam;
    public int Score;
}
