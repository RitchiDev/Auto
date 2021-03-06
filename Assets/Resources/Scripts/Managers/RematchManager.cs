using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andrich.UtilityScripts;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class RematchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static RematchManager Instance { get; private set; }
    private int m_NumberOfPlayersWhoVotedRematch;
    public int NumberOfPlayersWhoVotedRematch => m_NumberOfPlayersWhoVotedRematch;

    private bool m_GameHasBeenWon;

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
        if (changedProps.ContainsKey(PlayerProperties.IsReadyProperty))
        {
            m_NumberOfPlayersWhoVotedRematch = GetAmountOfPlayersWhoAreReady();

            if (m_GameHasBeenWon)
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

        MiniMapManager.Instance.Restart();

        if (PhotonNetwork.IsMasterClient)
        {
            Player[] playersInroom = PhotonNetwork.PlayerList;
            for (int i = 0; i < playersInroom.Length; i++)
            {
                playersInroom[i].SetLoadedAndReadyState(false);
            }

            Projectile[] projectiles = FindObjectsOfType<Projectile>();
            for (int i = 0; i < projectiles.Length; i++)
            {
                PhotonNetwork.Destroy(projectiles[i].gameObject);
            }
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

    public int GetAmountOfPlayersWhoAreReady()
    {
        Player[] playersInroom = PhotonNetwork.PlayerList;

        int amountOfPlayersWhoVotedRematch = 0;

        for (int i = 0; i < playersInroom.Length; i++)
        {
            //if (playersInroom[i].GetIfVotedRematch())
            if (playersInroom[i].GetIfReady())
            {
                amountOfPlayersWhoVotedRematch++;
            }
        }

        return amountOfPlayersWhoVotedRematch;
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (photonEvent.Code)
        {
            case PhotonEventCodes.CheckIfGameHasBeenWonEventCode:

                object[] winData = (object[])photonEvent.CustomData;

                m_GameHasBeenWon = (bool)winData[0];

                break;

            default:
                break;
        }
    }
}


