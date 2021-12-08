using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Mode Settings", menuName = "Create New Game Mode Settings")]
public class GameModeSettings : ScriptableObject
{
    [SerializeField] private string m_GameModeName = "Game MOode";
    public string GameModeName => m_GameModeName;

    [SerializeField] private string m_GameModeDescription = "Description of this mode";
    public string GameModeDescription => m_GameModeDescription;

    [SerializeField] private string m_PhotonPrefabsFolder = "PhotonPrefabs";
    public string PhotonPrefabsFolder => m_PhotonPrefabsFolder;

    [SerializeField] private string m_PlayerManagerString = "PlayerManager";
    public string PlayerManagerString => m_PlayerManagerString;

    [SerializeField] private string m_PlayerControllerString = "PlayerController";
    public string PlayerControllerString => m_PlayerControllerString;

    [SerializeField] private string m_PlayerSpectatorString = "PlayerSpectator";
    public string PlayerSpectatorString => m_PlayerSpectatorString;

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
