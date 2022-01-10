using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace Andrich.UtilityScripts
{
    public class RoomProperties
    {
        public const string TimeProperty = "CurrentEliminateTime";
        public const string TimerIsPausedProperty = "TimerIsPaused";
        //public const string GameHasBeenWonProperty = "GameHasBeenWon";
        //public const string PlayerWhoWonProperty = "PlayerWhoWon";
    }

    public static class TimerExtensions
    {
        public static void SetTime(this Room room, double newTime)
        {
            PhotonHashtable time = new PhotonHashtable();  // using PUN's implementation of Hashtable
            time[RoomProperties.TimeProperty] = newTime;

            room.SetCustomProperties(time);  // this locally sets the time and will sync it in-game asap.
        }

        public static double GetTime(this Room room)
        {
            object time;
            if (room.CustomProperties.TryGetValue(RoomProperties.TimeProperty, out time))
            {
                return (double)time;
            }

            return 0;
        }
    }

    public static class TimerColorExtensions
    {
        public static void SetIfEliminateTimerPaused(this Room room, bool isPaused)
        {
            PhotonHashtable paused = new PhotonHashtable();  // using PUN's implementation of Hashtable
            paused[RoomProperties.TimerIsPausedProperty] = isPaused;

            room.SetCustomProperties(paused);  // this locally sets the color and will sync it in-game asap.
        }

        public static bool GetIfEliminateTimerPaused(this Room room)
        {
            object paused;
            if (room.CustomProperties.TryGetValue(RoomProperties.TimerIsPausedProperty, out paused))
            {
                return (bool)paused;
            }

            return false;
        }
    }

    //public static class HasWonExtensions
    //{
    //    public static void SetIfGameHasBeenWon(this Room room, bool gameHasBeenWon)
    //    {
    //        PhotonHashtable won = new PhotonHashtable();  // using PUN's implementation of Hashtable
    //        won[RoomProperties.GameHasBeenWonProperty] = gameHasBeenWon;

    //        room.SetCustomProperties(won);  // this locally sets whether the game has been won and will sync it in-game asap.
    //    }

    //    public static bool GetIfGameHasBeenWon(this Room room)
    //    {
    //        object won;
    //        if (room.CustomProperties.TryGetValue(RoomProperties.GameHasBeenWonProperty, out won))
    //        {
    //            return (bool)won;
    //        }

    //        return false;
    //    }

    //    public static void SetPlayerWhoWon(this Room room, Player playerWhoWon)
    //    {
    //        PhotonHashtable player = new PhotonHashtable();  // using PUN's implementation of Hashtable
    //        player[RoomProperties.PlayerWhoWonProperty] = playerWhoWon;

    //        room.SetCustomProperties(player);  // this locally the name of the player who won and will sync it in-game asap.
    //    }

    //    public static Player GetPlayerWhoWon(this Room room)
    //    {
    //        object player;
    //        if (room.CustomProperties.TryGetValue(RoomProperties.PlayerWhoWonProperty, out player))
    //        {
    //            return (Player)player;
    //        }

    //        return null;
    //    }
    //}
}
