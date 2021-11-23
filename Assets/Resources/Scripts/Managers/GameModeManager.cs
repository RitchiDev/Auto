using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance { get; private set; }
    [SerializeField] private List<GameModeSettings> m_GameModes;
    [SerializeField] private RoomManager m_RoomManager;
    private GameModeSettings m_SelectedGameMode;
    public GameModeSettings SelectedGameMode => m_SelectedGameMode;
    private int m_GameModeIndex;

    [SerializeField] private TMP_Text m_GameModeNameText;
    [SerializeField] private TMP_Text m_SelectedMapText;
    [SerializeField] private TMP_InputField m_MaxPlayersInputField;
    [SerializeField] private Transform m_MapsItemContent;
    private int m_MaxPlayers;
    public int MaxPlayers => m_MaxPlayers;
    private int m_SceneIndex;
    public int CurrentSelectedSceneIndex => m_SceneIndex;
    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("An instance of: " + ToString() + " already existed!");
            Destroy(this);
        }
        else
        {
            //DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }

    private void Start()
    {
        ChangeGameModeSettings(0);
        m_MaxPlayersInputField.onValueChanged.AddListener(number => SetMaxPlayers(int.Parse(number)));
    }

    public void ChangeGameModeSettings(int amount)
    {
        m_GameModeIndex = Mathf.Clamp(m_GameModeIndex + amount, 0, m_GameModes.Count - 1);
        m_SelectedGameMode = m_GameModes[m_GameModeIndex];
        m_GameModeNameText.text = m_SelectedGameMode.GameModeName;
        m_RoomManager.SetUp(SelectedGameMode);
        
        ChangeMaxPlayers(20);
        SelectMap(m_SelectedGameMode.DefaultSceneIndex, 1);
    }

    public void ChangeMaxPlayers(int amount)
    {
        m_MaxPlayers = Mathf.Clamp(m_MaxPlayers + amount, m_SelectedGameMode.MinimumPlayers, m_SelectedGameMode.MaximumPlayers);
        m_MaxPlayersInputField.text = m_MaxPlayers.ToString();
    }

    public void SetMaxPlayers(int value)
    {
        m_MaxPlayers = Mathf.Clamp(value, m_SelectedGameMode.MinimumPlayers, m_SelectedGameMode.MaximumPlayers);
        m_MaxPlayersInputField.text = m_MaxPlayers.ToString();
    }

    public void SelectMap(int scene, int map)
    {
        m_SceneIndex = scene;
        m_SelectedMapText.text = "Selected Map: " + map.ToString();
    }

    public void ActivateMaps()
    {
        PoolManager.Instance.GetObjectFromPool(m_SelectedGameMode.MapsItem, m_MapsItemContent);
        //maps.transform.parent = m_MapsItemContent.transform;
    }
}
