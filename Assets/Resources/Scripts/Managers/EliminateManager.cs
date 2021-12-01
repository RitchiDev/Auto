using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.UtilityScripts;

public class EliminateManager : MonoBehaviour
{
    public static EliminateManager Instance { get; private set; }

    private List<GameObject> m_AlivePlayers = new List<GameObject>();

    private float m_MaxEliminationTime = 11f;
    private float m_TimeUntillElimination;

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
    }

    private void Start()
    {
        m_TimeUntillElimination = m_MaxEliminationTime;
    }

    private void Update()
    {
        if (m_AlivePlayers.Count > 0)
        {
            EliminationTimer();

        }
    }
    public void AddAlivePlayer(GameObject playerController)
    {
        m_AlivePlayers.Add(playerController);
    }

    private void EliminationTimer()
    {
        if (m_TimeUntillElimination > 0)
        {
            m_TimeUntillElimination -= Time.deltaTime;
        }

        else if (m_TimeUntillElimination < 0)
        {
            m_TimeUntillElimination = m_MaxEliminationTime;

            EliminatePlayer();
        }
    }

    private void EliminatePlayer()
    {
        EliminationPlayerController playerWithLowestScore = m_AlivePlayers[0].GetComponent<EliminationPlayerController>();
        int playerWithLowestScoreIndex = 0;


        for (int i = 0; i < m_AlivePlayers.Count; i++)
        {
            EliminationPlayerController currentCheckedPlayer = m_AlivePlayers[i].GetComponent<EliminationPlayerController>();

            if (currentCheckedPlayer.Player.GetScore() < playerWithLowestScore.Player.GetScore())
            {


                playerWithLowestScore = currentCheckedPlayer;
                playerWithLowestScoreIndex = i;
            }

            if (i >= m_AlivePlayers.Count)
            {
                Debug.Log("hoi");

                m_AlivePlayers.RemoveAt(playerWithLowestScoreIndex);
                playerWithLowestScore.Eliminate();
            }
        }
    }
}
