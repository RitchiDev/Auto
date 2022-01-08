using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using Andrich.UtilityScripts;
using ExitGames.Client.Photon;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class EliminationGameManager : MonoBehaviourPunCallbacks
{
    public static EliminationGameManager Instance { get; private set; }
    [SerializeField] private List<EliminationPlayerController> m_AlivePlayers = new List<EliminationPlayerController>();

    [SerializeField] private Image m_CountDownImage;
    [SerializeField] private TMP_Text m_CountDownText;
    [SerializeField] private float m_MaxEliminationTime = 60f;
    [SerializeField] private float m_TimeBeforeNextElimination = 5f;
    private EliminationPlayerController m_PlayerControllerToEliminate;
    private double m_CurrentTime;

    private IEnumerator m_CountdownCoroutine;

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

        //Restart();
    }

    private void Start()
    {
        Restart();
    }

    public void Restart()
    {
        if(m_CountdownCoroutine != null)
        {
            StopCoroutine(m_CountdownCoroutine);
        }

        m_AlivePlayers.Clear();

        if (PhotonNetwork.IsMasterClient)
        {
            //StopAllCoroutines();
            PhotonNetwork.CurrentRoom.SetIfGameHasBeenWon(false);
            RaiseActivateAllItemBoxesEvent();
            RaiseDeactivateAllRingsEvent();
        }

        if (RoomManager.Instance.GameModeSettings.GameModeName != "Elimination")
        {
            if (m_CountDownImage)
            {
                m_CountDownImage.SetActive(false);
            }

            if (m_CountDownText)
            {
                m_CountDownText.SetActive(false);
            }

            return;
        }

        if (m_CountDownImage)
        {
            m_CountDownImage.SetActive(true);
        }

        if (m_CountDownText)
        {
            m_CountDownText.SetActive(true);
        }
    }

    private bool GetIfAllPlayersLoadedLevel()
    {
        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetIfLoadedAndReady())
            {
                continue;
            }

            return false;
        }

        return true;
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

        if(propertiesThatChanged.ContainsKey(RoomProperties.TimeProperty))
        {
            m_CountDownText.color = PhotonNetwork.CurrentRoom.GetIfEliminateTimerPaused() ? Color.white : Color.red;

            float maxTime = PhotonNetwork.CurrentRoom.GetIfEliminateTimerPaused() ? m_TimeBeforeNextElimination : m_MaxEliminationTime;
            m_CountDownImage.fillAmount = (float)PhotonNetwork.CurrentRoom.GetTime() / maxTime;
            m_CountDownText.text = PhotonNetwork.CurrentRoom.GetTime().ToString("0");
        }

        if(propertiesThatChanged.ContainsKey(RoomProperties.GameHasBeenWonProperty))
        {
            if(PhotonNetwork.CurrentRoom.GetIfGameHasBeenWon())
            {
                RaiseDeactivateAllItemBoxesEvent();

                m_CountDownImage.SetActive(false);

                if(BackgroundMusicStarter.Instance)
                {
                    BackgroundMusicStarter.Instance.StopMusic();
                }
            }
        }

        base.OnRoomPropertiesUpdate(propertiesThatChanged);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        if (RoomManager.Instance.GameModeSettings.GameModeName != "Elimination")
        {
            return;
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if(changedProps.ContainsKey(PlayerProperties.LoadedLevelProperty))
        {
            //Debug.Log("Changed LoadedLevelProperty");
            if(GetIfAllPlayersLoadedLevel())
            {
                Debug.Log("Started Game");
                //Debug.Log("Alive Players: " + m_AlivePlayers.Count);
                m_CountdownCoroutine = TimeBeforeEliminateStartsCountdown();
                StartCoroutine(m_CountdownCoroutine);
            }
        }

        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if(PhotonNetwork.CurrentRoom.GetIfGameHasBeenWon())
        {
            return;
        }

        if (PhotonNetwork.LocalPlayer == newMasterClient)
        {
            if(PhotonNetwork.CurrentRoom.GetIfEliminateTimerPaused())
            {
                m_CountdownCoroutine = TimeBeforeEliminateStartsCountdown();
            }
            else
            {
                m_CountdownCoroutine = EliminateCountdown();
            }

            StartCoroutine(m_CountdownCoroutine);
        }

        base.OnMasterClientSwitched(newMasterClient);
    }

    private IEnumerator TimeBeforeEliminateStartsCountdown()
    {
        PhotonNetwork.CurrentRoom.SetIfEliminateTimerPaused(true);

        RaiseDeactivateAllRingsEvent();
        //RingManager.Instance.DeactiveAllRings();

        m_CurrentTime = m_TimeBeforeNextElimination;

        while (m_CurrentTime > 0)
        {
            m_CurrentTime -= Time.deltaTime;

            SetTime(m_CurrentTime);

            yield return null;
        }

        CleanUpAlivePlayerList();

        CheckForWin();

        if (m_AlivePlayers.Count >= 2)
        {
            m_CountdownCoroutine = EliminateCountdown();
            StartCoroutine(m_CountdownCoroutine);
        }
    }

    private IEnumerator EliminateCountdown()
    {
        PhotonNetwork.CurrentRoom.SetIfEliminateTimerPaused(false);

        RaiseActivateAllRingsEvent();
        //RingManager.Instance.ActivateAllRings();
        //RingManager.Instance.SetNew500RingActive();

        m_CurrentTime = m_MaxEliminationTime;

        while (m_CurrentTime >= 0)
        {
            m_CurrentTime -= Time.deltaTime;

            SetTime(m_CurrentTime);

            yield return null;
        }

        CleanUpAlivePlayerList();

        CheckForElimination();

        m_CountdownCoroutine = TimeBeforeEliminateStartsCountdown();
        StartCoroutine(m_CountdownCoroutine);
    }

    private void RaiseActivateAllRingsEvent()
    {
        //Debug.Log("Raise Activate All Rings Event");

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(EventCodes.ActivateAllRingsEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    private void RaiseDeactivateAllRingsEvent()
    {
        //Debug.Log("Raise Deactivate All Rings Event");
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(EventCodes.DeactivateAllRingsEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }


    private void RaiseDeactivateAllItemBoxesEvent()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(EventCodes.DeactivateAllItemBoxesEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    private void RaiseActivateAllItemBoxesEvent()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(EventCodes.ActivateAllItemBoxesEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    private void SetTime(double time)
    {
        if(!PhotonNetwork.IsConnected)
        {
            return;
        }

        if (PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.CurrentRoom.SetTime(time);
        }
    }

    private void CleanUpAlivePlayerList()
    {
        int indexToRemove = 0;
        for (int i = 0; i < m_AlivePlayers.Count; i++)
        {
            if(m_AlivePlayers[i] == null)
            {
                indexToRemove = i;
            }
        }

        if(indexToRemove > 0)
        {
            m_AlivePlayers.RemoveAt(indexToRemove);
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
            m_PlayerControllerToEliminate.Owner.SetIfEliminated(true);
            m_PlayerControllerToEliminate.Eliminate();
        }
    }

    private void CheckForWin()
    {
        if(m_AlivePlayers.Count >= 2)
        {
            return;
        }

        RaiseDeactivateAllRingsEvent();
        RaiseDeactivateAllItemBoxesEvent();
        //RingManager.Instance.DeactiveAllRings();

        PhotonNetwork.CurrentRoom.SetPlayerWhoWon(m_AlivePlayers[0].Owner);
        PhotonNetwork.CurrentRoom.SetIfGameHasBeenWon(true);
    }

    private void CheckForElimination()
    {
        //Debug.Log("Check for elimination");

        if(m_AlivePlayers.Count <= 1)
        {
            Debug.Log("Not enough players for elimination");
            return;
        }

        m_PlayerControllerToEliminate = m_AlivePlayers[0];

        for (int i = 0; i < m_AlivePlayers.Count; i++)
        {
            //Debug.Log(m_AlivePlayers.Count);
            EliminationPlayerController currentPlayerController = m_AlivePlayers[i];
            //Debug.Log(currentPlayerController.Player.NickName);

            //Debug.Log("The score of current checked player: " + currentPlayerController.Player.NickName + " (" + currentPlayerController.Player.GetScore() + ")" + " and player with lowest score: " + m_PlayerControllerToEliminate.Player.NickName + " (" + m_PlayerControllerToEliminate.Player.GetScore() + ")");

            if (currentPlayerController.Owner.GetScore() < m_PlayerControllerToEliminate.Owner.GetScore())
            {
                //Debug.Log("The score of: " + currentPlayerController.Player.NickName + " is lower than: " + m_PlayerControllerToEliminate.Player.NickName);
                m_PlayerControllerToEliminate = currentPlayerController;
            }
        }

        EliminatePlayer();
    }
}
