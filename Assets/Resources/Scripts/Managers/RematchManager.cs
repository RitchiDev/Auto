using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andrich.UtilityScripts;
using Photon.Realtime;

public class RematchManager : MonoBehaviourPunCallbacks
{
    public static RematchManager Instance { get; private set; }
    private int m_NumberOfPlayersWhoVotedRematch;
    public int NumberOfPlayersWhoVotedRematch => m_NumberOfPlayersWhoVotedRematch;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("An instance of: " + ToString() + " already existed!");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        m_NumberOfPlayersWhoVotedRematch = GetAmountOfPlayersWhoAreReady();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey(PlayerProperties.VotedRematchProperty))
        {
            m_NumberOfPlayersWhoVotedRematch = GetAmountOfPlayersWhoAreReady();

            if (PhotonNetwork.CurrentRoom.GetIfGameHasBeenWon())
            {
                if (m_NumberOfPlayersWhoVotedRematch >= PhotonNetwork.CurrentRoom.PlayerCount)
                {
                    //PhotonNetwork.CurrentRoom.SetIfGameHasBeenWon(false);

                    RestartGame();
                }
            }
        }

        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    private void RestartGame()
    {
        Debug.Log("Restarting Game...");

        BackgroundMusicStarter.Instance.Restart();

        if (PhotonNetwork.IsMasterClient)
        {
            Player[] playersInroom = PhotonNetwork.PlayerList;
            for (int i = 0; i < playersInroom.Length; i++)
            {
                playersInroom[i].SetLoadedAndReadyState(false);
            }

            RaiseDeactivateAllRingsEvent();
            RaiseDeactivateAllItemBoxesEvent();
            //RingManager.Instance.Restart();
        }

        if(EliminationGameManager.Instance)
        {
            EliminationGameManager.Instance.Restart();
        }

        PlayerManager[] playerManager = FindObjectsOfType<PlayerManager>();

        for (int i = 0; i < playerManager.Length; i++)
        {
            playerManager[i].Restart();
        }
    }

    private void RaiseDeactivateAllRingsEvent()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(EventCodes.DeactivateAllRingsEventCode, null, raiseEventOptions, ExitGames.Client.Photon.SendOptions.SendReliable);
    }

    private void RaiseDeactivateAllItemBoxesEvent()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(EventCodes.DeactivateAllItemBoxesEventCode, null, raiseEventOptions, ExitGames.Client.Photon.SendOptions.SendReliable);
    }

    public int GetAmountOfPlayersWhoAreReady()
    {
        Player[] playersInroom = PhotonNetwork.PlayerList;

        int amountOfPlayersWhoVotedRematch = 0;

        for (int i = 0; i < playersInroom.Length; i++)
        {
            if (playersInroom[i].GetIfVotedRematch())
            {
                amountOfPlayersWhoVotedRematch++;
            }
        }

        return amountOfPlayersWhoVotedRematch;
    }
}


