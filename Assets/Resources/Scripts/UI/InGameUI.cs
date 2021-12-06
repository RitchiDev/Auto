using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class InGameUI : MonoBehaviour
{
    [SerializeField] private PhotonView m_PhotonView;

    [SerializeField] private GameObject m_Scoreboard;
    [SerializeField] private GameObject m_OnScreenStats;
    [SerializeField] private GameObject m_PauseMenu;

    [SerializeField] private TMP_Text m_OnScreenScore;

    private GameMode.Elimination.EliminationPlayerManager m_PlayerManager;

    private void Start()
    {
        if(m_PhotonView.IsMine)
        {
            m_PlayerManager = PhotonView.Find((int)m_PhotonView.InstantiationData[0]).GetComponent<GameMode.Elimination.EliminationPlayerManager>();
        }
        else
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

    public void ReturnToTitleScreen()
    {
        m_PlayerManager.ReturnToTitlescreen();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
