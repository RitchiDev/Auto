using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using Andrich.UtilityScripts;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance { get; private set; }

    [Header("Online")]
    [SerializeField] private TMP_InputField m_PlayerNameInputField;

    [Header("Host Room")]
    [SerializeField] private TMP_InputField m_RoomNameInputField;

    [Header("In Room")]
    [SerializeField] private TMP_Text m_RoomNameText;
    [SerializeField] private Transform m_PlayerListContent;
    [SerializeField] private GameObject m_PlayerListItemPrefab;
    [SerializeField] private GameObject m_StartGameButton;
    [SerializeField] private GameObject m_ChooseMapBlocker;
    private List<ReadyToggle> m_TogglesInRoom = new List<ReadyToggle>();
    private List<CarMaterialSelector> m_CarMaterialSelectorInRoom = new List<CarMaterialSelector>();

    [Header("Find Room")]
    [SerializeField] private Transform m_RoomListContent;
    [SerializeField] private GameObject m_RoomListItemPrefab;

    [Header("Debug")]
    [SerializeField] private TMP_Text m_ErrorText;
    [SerializeField] private List<RoomInfo> m_RoomsList = new List<RoomInfo>();

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("An instance of: " + this.ToString() + " already existed!");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        m_TogglesInRoom = new List<ReadyToggle>();
    }

    private void Start()
    {
        //PhotonNetwork.OfflineMode = true;
        m_PlayerNameInputField.onValueChanged.AddListener(text => EditingPlayerName(text));

        if(PlayerPrefs.HasKey(SettingsProperties.UsernameProperty))
        {
            string savedUsername = PlayerPrefs.GetString(SettingsProperties.UsernameProperty);
            savedUsername = savedUsername.Replace(" ", "_");
            savedUsername = savedUsername.ReplaceCurseWords();

            PhotonNetwork.NickName = savedUsername;
            m_PlayerNameInputField.text = savedUsername;
        }
    }

    private void EditingPlayerName(string value)
    {
        value = value.Replace(" ", "_");
        value = value.ReplaceCurseWords();

        PlayerPrefs.SetString(SettingsProperties.UsernameProperty, value);

        m_PlayerNameInputField.text = value;
        PhotonNetwork.NickName = value;
    }

    public void PlayOnline()
    {
        //PhotonNetwork.OfflineMode = false;
        MenuManager.Instance.OpenMenu(MenuName.loadingMenu);

        if(PhotonNetwork.IsConnected)
        {
            return;
        }

        Debug.Log("Connecting to master");

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = Application.version;
    }

    public void LeaveOnline()
    {
        Debug.Log("Disconnecting from master");

        //PhotonNetwork.OfflineMode = true;
        MenuManager.Instance.OpenMenu(MenuName.titleMenu);
        PhotonNetwork.Disconnect();
    }


    public void StartGame()
    {
        if(!Application.isEditor)
        {
            if(!AllPlayersReady() || !MinimumPlayersReached())
            {
                if(!MinimumPlayersReached())
                {
                    Debug.Log("Minimim Players hasn't been reached!");
                }
                return;
            }
        }

        if(GameModeManager.Instance.SelectedGameMode.CloseRoomOnStart)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }

        //Debug.Log("NOW LOADING: " + GameModeManager.Instance.CurrentSelectedSceneIndex);

        PhotonNetwork.LoadLevel(GameModeManager.Instance.CurrentSelectedSceneIndex);
    }

    private bool AllPlayersReady()
    {
        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetIfReady())
            {
                Debug.Log(players[i].NickName + " is Ready");

                continue;
            }

            Debug.Log("Some players aren't ready");

            return false;
        }

        return true;
    }

    private bool MinimumPlayersReached()
    {
        return PhotonNetwork.PlayerList.Length >= GameModeManager.Instance.SelectedGameMode.MinimumPlayers;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu(MenuName.onlineMenu);
        Debug.Log("Joined Lobby");

        if (string.IsNullOrEmpty(m_PlayerNameInputField.text) || string.IsNullOrEmpty(PhotonNetwork.NickName))
        {
            PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
        }
    }

    public void CreateRoom()
    {
        m_RoomNameInputField.text = m_RoomNameInputField.text.Replace(" ", "_");
        m_RoomNameInputField.text = m_RoomNameInputField.text.ReplaceCurseWords();

        if (string.IsNullOrEmpty(m_RoomNameInputField.text))
        {
            m_RoomNameInputField.text = PhotonNetwork.NickName + "'s Room";
        }


        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomPropertiesForLobby = new string[] { RoomProperties.GameModeNameProperty};
        roomOptions.CustomRoomProperties = new PhotonHashTable { { RoomProperties.GameModeNameProperty, GameModeManager.Instance.SelectedGameMode.GameModeName } };
        roomOptions.MaxPlayers = (byte)GameModeManager.Instance.MaxPlayers;

        PhotonNetwork.CreateRoom(m_RoomNameInputField.text, roomOptions);

        GameModeManager.Instance.ActivateMaps();

        MenuManager.Instance.OpenMenu(MenuName.loadingMenu);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");

        MenuManager.Instance.OpenMenu(MenuName.inRoomMenu);
        m_RoomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] playerList = PhotonNetwork.PlayerList;
        foreach (Transform transform in m_PlayerListContent)
        {
            Destroy(transform.gameObject);
        }

        foreach (Player player in playerList)
        {
            //Debug.Log("Created Item");

            GameObject item = Instantiate(m_PlayerListItemPrefab, Vector3.zero, Quaternion.identity);
            item.transform.SetParent(m_PlayerListContent, false);
            item.GetComponent<PlayerInRoomItem>().SetUp(player, player.NickName);

            ReadyToggle toggle = item.GetComponentInChildren<ReadyToggle>();
            if(toggle)
            {
                toggle.SetUp(player);
                m_TogglesInRoom.Add(toggle);
            }

            CarMaterialSelector selector = item.GetComponent<CarMaterialSelector>();
            if (selector)
            {
                selector.SetUp(player);
                m_CarMaterialSelectorInRoom.Add(selector);
            }
        }

        m_ChooseMapBlocker.SetActive(!PhotonNetwork.IsMasterClient);
        m_StartGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashTable changedProps)
    {
        for (int i = 0; i < m_TogglesInRoom.Count; i++)
        {
            ReadyToggle readyToggle = m_TogglesInRoom[i];

            if(readyToggle)
            {
                readyToggle.UpdateToggleState(targetPlayer);
            }
        }

        for (int i = 0; i < m_CarMaterialSelectorInRoom.Count; i++)
        {
            CarMaterialSelector selector = m_CarMaterialSelectorInRoom[i];

            if(selector)
            {
                selector.UpdateCarSprite(targetPlayer);
            }
        }

        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer == newMasterClient)
        {
            GameModeManager.Instance.ActivateMaps();

            m_ChooseMapBlocker.SetActive(false);
            m_StartGameButton.SetActive(true);
        }
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu(MenuName.loadingMenu);
        //Debug.Log(PhotonNetwork.NickName + " joined the room");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        m_ErrorText.text = "Room creation failed: " + message + " Try again!";
        MenuManager.Instance.OpenMenu(MenuName.errorMenu);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu(MenuName.loadingMenu);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");
        MenuManager.Instance.OpenMenu(MenuName.onlineMenu);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        m_RoomsList = roomList;

        RefreshRoomList();
    }

    public void RefreshRoomList()
    {
        foreach (Transform transform in m_RoomListContent)
        {
            Destroy(transform.gameObject);
        }

        for (int i = 0; i < m_RoomsList.Count; i++)
        {
            RoomInfo room = m_RoomsList[i];


            if (m_RoomListItemPrefab == null)
            {
                Debug.LogError("m_RoomListItemPrefab is null!");
                break;
            }

            if (!room.RemovedFromList)
            {
                string gameModeName = (string)room.CustomProperties[RoomProperties.GameModeNameProperty];
                Instantiate(m_RoomListItemPrefab, m_RoomListContent).GetComponent<RoomListItem>().SetUp(room, gameModeName);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player Entered Room");

        GameObject item = Instantiate(m_PlayerListItemPrefab, Vector3.zero, Quaternion.identity);
        item.transform.SetParent(m_PlayerListContent, false);
        item.GetComponent<PlayerInRoomItem>().SetUp(newPlayer, newPlayer.NickName);

        ReadyToggle toggle = item.GetComponentInChildren<ReadyToggle>();
        if (toggle)
        {
            toggle.SetUp(newPlayer);
            m_TogglesInRoom.Add(toggle);
        }

        CarMaterialSelector selector = item.GetComponent<CarMaterialSelector>();
        if(selector)
        {
            selector.SetUp(newPlayer);
            m_CarMaterialSelectorInRoom.Add(selector);
        }
    }
}
