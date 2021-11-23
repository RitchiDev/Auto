using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance { get; private set; }
    [SerializeField] private PhotonView m_PhotonView;

    [SerializeField] private GameObject m_Scoreboard;
    [SerializeField] private GameObject m_OnScreenStats;
    [SerializeField] private GameObject m_PauseMenu;

    [SerializeField] private TMP_Text m_OnScreenScore;

    private GameObject m_CurrentPlayerController;

    private void Awake()
    {
        if (m_PhotonView.IsMine)
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
        }
    }

    private void Start()
    {
        if(!m_PhotonView.IsMine)
        {
            Destroy(m_Scoreboard);
            Destroy(m_OnScreenStats);
            Destroy(m_PauseMenu);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if(!m_PhotonView.IsMine)
        {
            return;
        }
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu();
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

    public void SetOnscreenScore(int score)
    {
        m_OnScreenScore.text = "Score: " + score.ToString();
    }

    public void PauseMenu()
    {
        if (m_PauseMenu.activeInHierarchy)
        {
            m_PauseMenu.SetActive(false);
            m_Scoreboard.SetActive(false);
            m_OnScreenStats.SetActive(true);
        }
        else
        {
            m_PauseMenu.SetActive(true);
            m_Scoreboard.SetActive(true);
            m_OnScreenStats.SetActive(false);
        }
    }

    public void SetCurrentPlayerController(GameObject controller)
    {
        m_CurrentPlayerController = controller;
    }

    public void ReturnToTitleScreen()
    {
        PhotonNetwork.Destroy(m_CurrentPlayerController);

        StartCoroutine(LeaveAndLoad());
    }

    private IEnumerator LeaveAndLoad()
    {
        PhotonNetwork.LeaveRoom();

        while (PhotonNetwork.InRoom)
        {
            yield return null;
        }
        while (!PhotonNetwork.IsConnected)
        {
            yield return null;
        }

        Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LoadLevel(0); // Titlescreen
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
