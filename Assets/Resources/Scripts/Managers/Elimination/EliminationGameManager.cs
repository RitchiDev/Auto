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

public class EliminationGameManager : MonoBehaviourPunCallbacks, IOnEventCallback
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

    private bool m_GameHasBeenWon;
    public bool GameHasBeenWOn => m_GameHasBeenWon;
    private Player m_PlayerWhoWon;
    public Player PlayerWhoWon => m_PlayerWhoWon;

    private string m_GameModeName;

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
        m_GameModeName = (string)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperties.GameModeNameProperty];
        //m_GameModeName = RoomManager.Instance.GameModeSettings.GameModeName; // Players who join later on get null
        Restart();
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (m_GameModeName != RoomProperties.EliminationGameModeString)
        {
            PhotonEvents.RaiseActivateAllItemBoxesEvent();
            Stop();
        }

        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (m_GameModeName != RoomProperties.EliminationGameModeString)
        {
            Stop();
        }

        base.OnPlayerLeftRoom(otherPlayer);
    }

    private void Stop()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonEvents.RaiseCheckIfGameHasBeenWonEvent(false);
            PhotonNetwork.CurrentRoom.SetIfEliminateTimerPaused(true);

            PhotonEvents.RaiseDeactivateAllRingsEvent();
            PhotonEvents.RaiseActivateAllItemBoxesEvent();
        }

        if (m_CountDownImage)
        {
            m_CountDownImage.transform.parent.gameObject.SetActive(false);
            m_CountDownImage.gameObject.SetActive(false);
        }

        if (m_CountDownText)
        {
            m_CountDownText.gameObject.SetActive(false);
        }
    }

    public void Restart()
    {
        if(m_CountdownCoroutine != null)
        {
            StopCoroutine(m_CountdownCoroutine);
        }

        m_AlivePlayers.Clear();

        if (m_GameModeName != RoomProperties.EliminationGameModeString)
        {
            Stop();

            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonEvents.RaiseCheckIfGameHasBeenWonEvent(false);

            PhotonNetwork.CurrentRoom.SetIfEliminateTimerPaused(true);

            PhotonEvents.RaiseActivateAllItemBoxesEvent();
            PhotonEvents.RaiseDeactivateAllRingsEvent();
        }

        if (m_CountDownImage)
        {
            m_CountDownImage.gameObject.SetActive(true);
        }

        if (m_CountDownText)
        {
            m_CountDownText.gameObject.SetActive(true);
        }
    }

    public void UpdatePlayerWhoIsInDanger(Player playerInDanger)
    {
        //Debug.Log("Player In Danger: " + playerInDanger);

        for (int i = 0; i < m_AlivePlayers.Count; i++)
        {
            Player owner = m_AlivePlayers[i].Owner;

            //Debug.Log("Update In Danger: " + (owner == playerInDanger));

            m_AlivePlayers[i].SetInDanger(owner == playerInDanger);
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
        if (m_GameModeName != "Elimination")
        {
            return;
        }

        if(propertiesThatChanged.ContainsKey(RoomProperties.TimeProperty))
        {
            m_CountDownText.color = PhotonNetwork.CurrentRoom.GetIfEliminateTimerPaused() ? Color.white : Color.red;

            float maxTime = PhotonNetwork.CurrentRoom.GetIfEliminateTimerPaused() ? m_TimeBeforeNextElimination : m_MaxEliminationTime;
            m_CountDownImage.fillAmount = (float)PhotonNetwork.CurrentRoom.GetTime() / maxTime;
            m_CountDownText.text = PhotonNetwork.CurrentRoom.GetTime().ToString("0");
        }

        base.OnRoomPropertiesUpdate(propertiesThatChanged);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        if (m_GameModeName != RoomProperties.EliminationGameModeString)
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
        if (m_GameHasBeenWon)
        {
            return;
        }

        if (PhotonNetwork.LocalPlayer == newMasterClient)
        {
            if (m_GameModeName != RoomProperties.EliminationGameModeString)
            {
                Stop();
                return;
            }

            if(m_CountdownCoroutine != null)
            {
                StopCoroutine(m_CountdownCoroutine);
            }

            if (PhotonNetwork.CurrentRoom.GetIfEliminateTimerPaused())
            {
                m_CountdownCoroutine = TimeBeforeEliminateStartsCountdown();
            }
            else
            {
                m_CountdownCoroutine = EliminateCountdown();
            }

            if(m_CountdownCoroutine != null)
            {
                StartCoroutine(m_CountdownCoroutine);
            }
        }

        base.OnMasterClientSwitched(newMasterClient);
    }

    private IEnumerator TimeBeforeEliminateStartsCountdown()
    {
        PhotonNetwork.CurrentRoom.SetIfEliminateTimerPaused(true);

        PhotonEvents.RaiseDeactivateAllRingsEvent();
        //RaiseDeactivateAllRingsEvent();
        //RingManager.Instance.DeactiveAllRings();

        m_CurrentTime = m_TimeBeforeNextElimination;

        while (m_CurrentTime >= 0)
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

        PhotonEvents.RaiseActivateAllRingsEvent();
        //RaiseActivateAllRingsEvent();
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

        PhotonEvents.RaiseDeactivateAllRingsEvent();
        PhotonEvents.RaiseDeactivateAllItemBoxesEvent();
        //RaiseDeactivateAllRingsEvent();
        //RaiseDeactivateAllItemBoxesEvent();

        //RingManager.Instance.DeactiveAllRings();

        PhotonEvents.RaiseCheckIfGameHasBeenWonEvent(true, m_AlivePlayers[0].Owner);
        //PhotonNetwork.CurrentRoom.SetPlayerWhoWon(m_AlivePlayers[0].Owner);
        //PhotonNetwork.CurrentRoom.SetIfGameHasBeenWon(true);
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

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (eventCode)
        {
            case PhotonEventCodes.CheckIfGameHasBeenWonEventCode:

                object[] data = (object[])photonEvent.CustomData;

                m_GameHasBeenWon = (bool)data[0];
                m_PlayerWhoWon = (Player)data[1];

                if (m_GameHasBeenWon)
                {
                    PhotonEvents.RaiseDeactivateAllItemBoxesEvent();
                    //RaiseDeactivateAllItemBoxesEvent();

                    m_CountDownImage.gameObject.SetActive(false);

                    if (BackgroundMusicStarter.Instance)
                    {
                        BackgroundMusicStarter.Instance.StopMusic();
                    }
                }

                break;

            default:
                break;
        }
    }
}
