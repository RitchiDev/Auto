using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using Andrich.UtilityScripts;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class EliminateManager : MonoBehaviourPunCallbacks
{
    public static EliminateManager Instance { get; private set; }
    [SerializeField] private List<EliminationPlayerController> m_AlivePlayers = new List<EliminationPlayerController>();

    [SerializeField] private Image m_CountDownImage;
    [SerializeField] private TMP_Text m_CountDownText;
    [SerializeField] private float m_MaxEliminationTime = 60f;
    [SerializeField] private float m_TimeBeforeNextElimination = 5f;
    private EliminationPlayerController m_PlayerControllerToEliminate;
    private double m_CurrentTime;
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

        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetIfToDoElimination(false);
        }
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(PhotonTimeBeforeEliminateStartsCountdown());
        }
    }

    public override void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged)
    {
        m_CountDownText.color = PhotonNetwork.CurrentRoom.GetIfEliminateTimerPaused() ? Color.white : Color.red;

        m_CountDownImage.fillAmount = (float)PhotonNetwork.CurrentRoom.GetTime() / m_MaxEliminationTime;
        m_CountDownText.text = PhotonNetwork.CurrentRoom.GetTime().ToString("0");

        if(PhotonNetwork.CurrentRoom.GetIfToDoElimination())
        {
            //if (PhotonNetwork.IsMasterClient)
            //{
            //    PhotonNetwork.CurrentRoom.SetIfToDoElimination(false);
            //}

            EliminatePlayer();
        }

        base.OnRoomPropertiesUpdate(propertiesThatChanged);
    }

    private IEnumerator PhotonTimeBeforeEliminateStartsCountdown()
    {
        RingManager.Instance.DeactiveAllRings();

        PhotonNetwork.CurrentRoom.SetIfEliminateTimerPaused(true);

        m_CurrentTime = m_MaxEliminationTime;

        while (m_CurrentTime > 0)
        {
            m_CurrentTime -= PhotonNetwork.Time;
            PhotonNetwork.CurrentRoom.SetTime(m_CurrentTime);
            yield return null;
        }

        StartCoroutine(PhotonEliminateCountdown());
    }

    private IEnumerator PhotonEliminateCountdown()
    {
        RingManager.Instance.ActivateAllRings();
        RingManager.Instance.SetNew500RingActive();

        PhotonNetwork.CurrentRoom.SetIfEliminateTimerPaused(false);

        m_CurrentTime = m_MaxEliminationTime;
        while (m_CurrentTime >= 0)
        {
            m_CurrentTime -= Time.deltaTime;
            PhotonNetwork.CurrentRoom.SetTime(m_CurrentTime);
            yield return null;
        }

        if (m_AlivePlayers.Count <= 1)
        {
            Debug.Log("Not Enough Alive Players");
            yield return null;
        }

        CheckForElimination();
        StartCoroutine(PhotonTimeBeforeEliminateStartsCountdown());
    }


    public void AddAlivePlayer(EliminationPlayerController playerController, Player player)
    {
        playerController.SetPlayer(player);
        m_AlivePlayers.Add(playerController);
    }

    public void RemoveAlivePlayer(EliminationPlayerController playerController)
    {
        m_AlivePlayers.Remove(playerController);
    }

    private void EliminatePlayer()
    {
        if (m_PlayerControllerToEliminate)
        {
            Debug.Log(m_PlayerControllerToEliminate.Player.NickName + ": Eliminated");
            m_PlayerControllerToEliminate.Player.SetEliminated(true);
            m_PlayerControllerToEliminate.Eliminate();
        }
    }

    private void CheckForElimination()
    {
        Debug.Log("Check for elimination");

        if(m_AlivePlayers.Count <= 1)
        {
            return;
        }

        m_PlayerControllerToEliminate = m_AlivePlayers[0];

        for (int i = 0; i < m_AlivePlayers.Count; i++)
        {
            Debug.Log(m_AlivePlayers.Count);
            EliminationPlayerController currentPlayerController = m_AlivePlayers[i];
            Debug.Log(currentPlayerController.Player.NickName);

            Debug.Log("The score of current checked player: " + currentPlayerController.Player.NickName + " (" + currentPlayerController.Player.GetScore() + ")" + " and player with lowest score: " + m_PlayerControllerToEliminate.Player.NickName + " (" + m_PlayerControllerToEliminate.Player.GetScore() + ")");

            if (currentPlayerController.Player.GetScore() < m_PlayerControllerToEliminate.Player.GetScore())
            {
                Debug.Log("The score of: " + currentPlayerController.Player.NickName + " is lower than: " + m_PlayerControllerToEliminate.Player.NickName);
                m_PlayerControllerToEliminate = currentPlayerController;
            }
        }

        PhotonNetwork.CurrentRoom.SetIfToDoElimination(true);
    }

    private IEnumerator TimeBeforeEliminateStartsCountdown()
    {
        RingManager.Instance.DeactiveAllRings();

        m_CountDownText.color = Color.white;

        float totalTime = m_TimeBeforeNextElimination;
        while (totalTime >= 0)
        {
            m_CountDownImage.fillAmount = totalTime / m_MaxEliminationTime;
            totalTime -= Time.deltaTime;
            m_CountDownText.text = totalTime.ToString("0");

            yield return null;
        }

        StartCoroutine(EliminateCountdown());
    }

    private IEnumerator EliminateCountdown()
    {
        RingManager.Instance.ActivateAllRings();
        RingManager.Instance.SetNew500RingActive();

        m_CountDownText.color = Color.red;

        float totalTime = m_MaxEliminationTime;
        while (totalTime >= 0)
        {
            m_CountDownImage.fillAmount = totalTime / m_MaxEliminationTime;
            totalTime -= Time.deltaTime;
            m_CountDownText.text = totalTime.ToString("0");

            yield return null;
        }

        //EliminatePlayer();
        StartCoroutine(TimeBeforeEliminateStartsCountdown());
    }
}
