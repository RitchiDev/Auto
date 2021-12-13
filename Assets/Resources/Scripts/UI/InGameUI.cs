using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Andrich.UtilityScripts;

public class InGameUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private PhotonView m_PhotonView;

    [SerializeField] private GameObject m_Scoreboard;
    [SerializeField] private GameObject m_OnScreenStats;
    [SerializeField] private GameObject m_PauseMenu;
    [SerializeField] private GameObject m_WinMenu;
    [SerializeField] private TMP_Text m_WinnerText;
    [SerializeField] private TMP_Text m_OnScreenScore;

    private GameMode.Elimination.EliminationPlayerManager m_PlayerManager;
    private bool m_GameIsWon;

    private void Start()
    {
        if(m_PhotonView.IsMine)
        {
            m_GameIsWon = false;
            m_PlayerManager = PhotonView.Find((int)m_PhotonView.InstantiationData[0]).GetComponent<GameMode.Elimination.EliminationPlayerManager>();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Destroy(m_Scoreboard);
            Destroy(m_OnScreenStats);
            Destroy(m_PauseMenu);
            Destroy(m_WinMenu);
            Destroy(gameObject);
            //gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(!m_PhotonView.IsMine || m_GameIsWon)
        {
            return;
        }
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OpenPauseMenu();
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            m_Scoreboard.SetActive(true);
        }
        else
        {
            if (m_PauseMenu.activeInHierarchy)
            {
                return;
            }

            m_Scoreboard.SetActive(false);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if(PhotonNetwork.CurrentRoom.GetIfGameHasBeenWon())
        {
            if(PhotonNetwork.CurrentRoom.GetPlayerWhoWon() != null)
            {
                OpenWinMenu(PhotonNetwork.CurrentRoom.GetPlayerWhoWon().NickName);
            }
        }

        base.OnRoomPropertiesUpdate(propertiesThatChanged);
    }

    public void SetOnscreenScore(int score)
    {
        m_OnScreenScore.text = "Score: " + score.ToString();
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
            m_Scoreboard.SetActive(true);
            m_OnScreenStats.SetActive(false);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void ClosePauseMenu()
    {
        m_PauseMenu.SetActive(false);
        m_Scoreboard.SetActive(false);
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

        m_GameIsWon = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        m_WinMenu.SetActive(true);
        m_Scoreboard.SetActive(true);
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
