using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Mode Settings", menuName = "Create New Game Mode Settings")]
public class GameModeSettings : ScriptableObject
{
    [SerializeField] private string m_GameModeName = "Game Mode";
    public string GameModeName => m_GameModeName;

    [SerializeField] private string m_GameModeDescription = "Description of this mode";
    public string GameModeDescription => m_GameModeDescription;

    [Tooltip("Prefab with the exact same name/path is not allowed to exist!")]
    [SerializeField] private GameObject m_PlayerManagerPrefab = null;
    public string PlayerManagerString => m_PlayerManagerPrefab.name;

    [Tooltip("Prefab with the exact same name/path is not allowed to exist!")]
    [SerializeField] private GameObject m_PlayerControllerPrefab = null;
    public string PlayerControllerString => m_PlayerControllerPrefab.name;

    [Tooltip("Prefab with the exact same name/path is nowt allowed to exist!")]
    [SerializeField] private GameObject m_PlayerSpectatorPrefab = null;
    public string PlayerSpectatorString => m_PlayerSpectatorPrefab.name;

    [SerializeField] private int m_MinimumPlayers = 2;
    public int MinimumPlayers => m_MinimumPlayers;

    [SerializeField] private int m_MaximumPlayers = 20;
    public int MaximumPlayers => m_MaximumPlayers;

    [SerializeField] private int m_DefaultSceneIndex = 1;
    public int DefaultSceneIndex => m_DefaultSceneIndex;

    [SerializeField] private PoolAbleObject m_MapsItem = null;
    public PoolAbleObject MapsItem => m_MapsItem;

    [SerializeField] private bool m_CloseRoomOnStart = true;
    public bool CloseRoomOnStart => m_CloseRoomOnStart;
}
