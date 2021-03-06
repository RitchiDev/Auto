using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Andrich.UtilityScripts;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class InGameUI : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [Header("Components")]
    [SerializeField] private PhotonView m_PhotonView;
    private EliminationPlayerManager m_PlayerManager;

    [Header("Dead Players UI")]
    [SerializeField] private Transform m_DeadPlayerListContainer;
    [SerializeField] private GameObject m_RespawnedPlayerItemPrefab;
    [SerializeField] private GameObject m_KOdPlayerItemPrefab;
    [SerializeField] private GameObject m_EliminatedPlayerItemPrefab;

    [Header("Tab Scoreboard UI")]
    [SerializeField] private GameObject m_TabScoreboard;

    [Header("On Screen Stats UI")]
    [SerializeField] private GameObject m_OnScreenStats;
    [SerializeField] private TMP_Text m_OnScreenScore;

    [Header("Pause UI")]
    [SerializeField] private GameObject m_PauseMenu;

    [Header("Win UI")]
    [SerializeField] private GameObject m_WinMenu;
    [SerializeField] private TMP_Text m_WinnerText;
    [SerializeField] private TMP_Text m_NumberOfVotedRematchText;

    [Header("Item UI")]
    [SerializeField] private Image m_ItemImage;
    [SerializeField] private Sprite m_EmptySprite;
    [SerializeField] private Sprite m_QuestionMarkSprite;

    [Header("Misc")]
    private bool m_GameHasBeenWon;
    private Player m_PlayerWhoWon;

    private void Start()
    {
        if(m_PhotonView.IsMine)
        {
            //m_GameHasBeenWon = false;
            m_PlayerManager = PhotonView.Find((int)m_PhotonView.InstantiationData[0]).GetComponent<EliminationPlayerManager>();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            
            //Destroy(m_TabScoreboard);
            //Destroy(m_OnScreenStats);
            //Destroy(m_PauseMenu);
            //Destroy(m_WinMenu);
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(!m_PhotonView.IsMine || m_GameHasBeenWon)
        {
            return;
        }
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OpenPauseMenu();
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            m_TabScoreboard.SetActive(true);
        }
        else
        {
            if (m_PauseMenu.activeInHierarchy)
            {
                return;
            }

            m_TabScoreboard.SetActive(false);
        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(m_GameHasBeenWon)
        {
            UpdateOnScreenPlayersWhoVotedRematch();
        }

        base.OnPlayerLeftRoom(otherPlayer);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(changedProps.ContainsKey(PlayerProperties.IsReadyProperty))
        {
            if(m_GameHasBeenWon)
            {
                UpdateOnScreenPlayersWhoVotedRematch();
            }
        }

        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (eventCode)
        {
            case PhotonEventCodes.CheckIfGameHasBeenWonEventCode:

                object[] winData = (object[])photonEvent.CustomData;

                m_GameHasBeenWon = (bool)winData[0];
                m_PlayerWhoWon = (Player)winData[1];

                UpdateOnScreenPlayersWhoVotedRematch();

                if (m_PlayerWhoWon != null)
                {
                    OpenWinMenu(m_PlayerWhoWon.NickName);
                }

                break;

            case PhotonEventCodes.AddPlayerRespawnedToUIEventCode:

                object[] respawnData = (object[])photonEvent.CustomData;

                AddRespawnToUI((string)respawnData[0], (string)respawnData[1], (bool)respawnData[2]);

                break;
            case PhotonEventCodes.AddPlayerGotEliminatedToUIEventCode:

                object[] eliminateData = (object[])photonEvent.CustomData;

                AddEliminateToUI((string)eliminateData[0]);

                break;

            default:
                break;
        }
    }

    public void AddEliminateToUI(string name)
    {
        Instantiate(m_EliminatedPlayerItemPrefab, m_DeadPlayerListContainer).GetComponent<PlayerDiedItem>().SetUp(name, "Eliminated");
    }

    public void AddRespawnToUI(string name, string deathCause, bool afterKO)
    {
        if (afterKO)
        {
            Instantiate(m_KOdPlayerItemPrefab, m_DeadPlayerListContainer).GetComponent<PlayerDiedItem>().SetUp(name, deathCause);
        }
        else
        {
            Instantiate(m_RespawnedPlayerItemPrefab, m_DeadPlayerListContainer).GetComponent<PlayerDiedItem>().SetUp(name, deathCause);
        }
    }

    public void VoteRematch()
    {
        if(m_PhotonView.IsMine)
        {
            Player player = PhotonNetwork.LocalPlayer;
            player.SetReadyState(!player.GetIfReady());
            //player.SetVotedRematchState(!player.GetIfVotedRematch());
            //Debug.Log("Voted: " + player.GetIfVotedRematch());
        }
    }

    private void UpdateOnScreenPlayersWhoVotedRematch()
    {
        m_NumberOfVotedRematchText.text = "Voted: " + RematchManager.Instance.NumberOfPlayersWhoVotedRematch + " / " + PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public void SetOnscreenScore(int score)
    {
        m_OnScreenScore.text = "Score: " + score.ToString();
    }

    public void SetItemImageSprite(Sprite sprite)
    {
        m_ItemImage.sprite = sprite;
    }

    public void EmptyItemImageSprite()
    {
        m_ItemImage.sprite = m_EmptySprite;
    }

    public void OpenPauseMenu()
    {
        if (m_PauseMenu.activeInHierarchy)
        {
            ClosePauseMenu();
        }
        else
        {
            m_PauseMenu.SetActive(true);
            m_TabScoreboard.SetActive(true);
            m_OnScreenStats.SetActive(false);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void ClosePauseMenu()
    {
        m_PauseMenu.SetActive(false);
        m_TabScoreboard.SetActive(false);
        m_OnScreenStats.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void OpenWinMenu(string playerName)
    {
        if(!m_PhotonView.IsMine)
        {
            return;
        }

        ClosePauseMenu();

        //m_GameHasBeenWon = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        m_WinMenu.SetActive(true);
        m_TabScoreboard.SetActive(true);
        m_OnScreenStats.SetActive(false);

        if(m_WinnerText)
        {
            m_WinnerText.text = playerName + " Won The Game!";
        }
    }

    public void ReturnToTitleScreen()
    {
        m_PlayerManager.ReturnToTitlescreen();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
