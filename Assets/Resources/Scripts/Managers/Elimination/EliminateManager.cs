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

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetIfToDoElimination(false);
            PhotonNetwork.CurrentRoom.SetIfGameHasBeenWon(false);
        }
    }

    private void Start()
    {
        if(RoomManager.Instance.GameModeSettings.GameModeName != "Elimination")
        {
            m_CountDownImage.transform.parent.gameObject.SetActive(false);
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(TimeBeforeEliminateStartsCountdown());
        }
    }

    public override void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged)
    {
        if(RoomManager.Instance.GameModeSettings.GameModeName != "Elimination")
        {
            if(m_CountDownImage)
            {
                m_CountDownImage.SetActive(false);
            }

            if(m_CountDownText)
            {
                m_CountDownText.SetActive(false);
            }
            return;
        }

        m_CountDownText.color = PhotonNetwork.CurrentRoom.GetIfEliminateTimerPaused() ? Color.white : Color.red;

        float maxTime = PhotonNetwork.CurrentRoom.GetIfEliminateTimerPaused() ? m_TimeBeforeNextElimination : m_MaxEliminationTime;
        m_CountDownImage.fillAmount = (float)PhotonNetwork.CurrentRoom.GetTime() / maxTime;
        m_CountDownText.text = PhotonNetwork.CurrentRoom.GetTime().ToString("0");

        if(PhotonNetwork.CurrentRoom.GetIfGameHasBeenWon())
        {
            m_CountDownImage.SetActive(false);
        }

        base.OnRoomPropertiesUpdate(propertiesThatChanged);
    }

    private IEnumerator TimeBeforeEliminateStartsCountdown()
    {
        PhotonNetwork.CurrentRoom.SetIfEliminateTimerPaused(true);

        RingManager.Instance.DeactiveAllRings();

        m_CurrentTime = m_TimeBeforeNextElimination;

        while (m_CurrentTime > 0)
        {
            m_CurrentTime -= Time.deltaTime;
            PhotonNetwork.CurrentRoom.SetTime(m_CurrentTime);
            yield return null;
        }

        CheckForWin();

        if (m_AlivePlayers.Count <= 1)
        {
            Debug.Log("Not Enough Alive Players");
            yield return null;
        }

        StartCoroutine(EliminateCountdown());
    }

    private IEnumerator EliminateCountdown()
    {
        PhotonNetwork.CurrentRoom.SetIfEliminateTimerPaused(false);

        RingManager.Instance.ActivateAllRings();
        RingManager.Instance.SetNew500RingActive();

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

        if(!PhotonNetwork.CurrentRoom.GetIfGameHasBeenWon())
        {
            StartCoroutine(TimeBeforeEliminateStartsCountdown());
        }
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
            //Debug.Log(m_PlayerControllerToEliminate.Player.NickName + ": Eliminated");
            m_PlayerControllerToEliminate.Player.SetEliminated(true);
            m_PlayerControllerToEliminate.Eliminate();
        }

        //CheckForWin();
    }

    private void CheckForWin()
    {
        if (m_AlivePlayers.Count <= 1)
        {
            PhotonNetwork.CurrentRoom.SetPlayerWhoWon(m_AlivePlayers[0].Player);
            PhotonNetwork.CurrentRoom.SetIfGameHasBeenWon(true);

            return;
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
            //Debug.Log(m_AlivePlayers.Count);
            EliminationPlayerController currentPlayerController = m_AlivePlayers[i];
            //Debug.Log(currentPlayerController.Player.NickName);

            //Debug.Log("The score of current checked player: " + currentPlayerController.Player.NickName + " (" + currentPlayerController.Player.GetScore() + ")" + " and player with lowest score: " + m_PlayerControllerToEliminate.Player.NickName + " (" + m_PlayerControllerToEliminate.Player.GetScore() + ")");

            if (currentPlayerController.Player.GetScore() < m_PlayerControllerToEliminate.Player.GetScore())
            {
                //Debug.Log("The score of: " + currentPlayerController.Player.NickName + " is lower than: " + m_PlayerControllerToEliminate.Player.NickName);
                m_PlayerControllerToEliminate = currentPlayerController;
            }
        }

        EliminatePlayer();
    }
}
