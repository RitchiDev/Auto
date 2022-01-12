using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace Andrich.UtilityScripts
{
    public class PhotonEventCodes
    {
        // Never use 0
        public const byte DeactivateAllRingsEventCode = 1;
        public const byte ActivateAllRingsEventCode = 2;
        public const byte ActivateNew500RingEventCode = 3;

        public const byte DeactivateAllItemBoxesEventCode = 4;
        public const byte ActivateAllItemBoxesEventCode = 5;

        public const byte AddPlayerGotEliminatedToUIEventCode = 6;
        public const byte AddPlayerRespawnedToUIEventCode = 7;

        public const byte CheckIfGameHasBeenWonEventCode = 8;
        public const byte RaisePlayerEditedPrimaryCarColorEventCode = 9;
        public const byte RaisePlayerEditedSecondaryCarColorEventCode = 10;
    }

    public static class PhotonEvents
    {
        public static void RaiseCheckIfGameHasBeenWonEvent(bool gameHasBeenWon, Player playerWhoWon = null)
        {
            object[] eventContent = new object[] { gameHasBeenWon, playerWhoWon };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(PhotonEventCodes.CheckIfGameHasBeenWonEventCode, eventContent, raiseEventOptions, SendOptions.SendReliable);
        }

        public static void RaiseActivateAllItemBoxesEvent()
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent(PhotonEventCodes.ActivateAllItemBoxesEventCode, null, raiseEventOptions, SendOptions.SendReliable);
        }

        public static void RaiseActivateAllRingsEvent()
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent(PhotonEventCodes.ActivateAllRingsEventCode, null, raiseEventOptions, SendOptions.SendReliable);
        }

        public static void RaiseActivateNew500RingEvent(int ringToSetActive)
        {
            object[] eventContent = new object[] { ringToSetActive };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(PhotonEventCodes.ActivateNew500RingEventCode, eventContent, raiseEventOptions, SendOptions.SendReliable);
        }

        public static void RaiseDeactivateAllRingsEvent()
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent(PhotonEventCodes.DeactivateAllRingsEventCode, null, raiseEventOptions, SendOptions.SendReliable);
        }

        public static void RaiseDeactivateAllItemBoxesEvent()
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent(PhotonEventCodes.DeactivateAllItemBoxesEventCode, null, raiseEventOptions, SendOptions.SendReliable);
        }

        public static void RaiseAddEliminateToUIEvent(string name)
        {
            object[] eventContent = new object[] { name };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(PhotonEventCodes.AddPlayerGotEliminatedToUIEventCode, eventContent, raiseEventOptions, SendOptions.SendReliable);
        }

        public static void RaiseAddRespawnToUIEvent(string name, string deathCause, bool afterKO)
        {
            object[] eventContent = new object[] { name, deathCause, afterKO };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(PhotonEventCodes.AddPlayerRespawnedToUIEventCode, eventContent, raiseEventOptions, SendOptions.SendReliable);
        }

        public static void RaisePlayerEditedPrimaryCarControllerEvent(byte primaryIndex, Player playerWhoEdited)
        {
            object[] eventContent = new object[] { primaryIndex, playerWhoEdited };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(PhotonEventCodes.RaisePlayerEditedPrimaryCarColorEventCode, eventContent, raiseEventOptions, SendOptions.SendReliable);
        }

        public static void RaisePlayerEditedSecondaryCarControllerEvent(byte secondaryIndex, Player playerWhoEdited)
        {
            object[] eventContent = new object[] { secondaryIndex, playerWhoEdited };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(PhotonEventCodes.RaisePlayerEditedSecondaryCarColorEventCode, eventContent, raiseEventOptions, SendOptions.SendUnreliable);
        }
    }
}
