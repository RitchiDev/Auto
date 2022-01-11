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
        public const string GameModeNameProperty = "GameModeName";

        public const string EliminationGameModeString = "Elimination";
        public const string FreeRoamGameModeString = "Free Roam";
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
}
