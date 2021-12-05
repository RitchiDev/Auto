// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PunPlayerScores.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities,
// </copyright>
// <summary>
//  Scoring system for PhotonPlayer
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// Scoring system for PhotonPlayer
    /// </summary>
    public class PunPlayerScores : MonoBehaviour
    {
        public const string PlayerScoreProp = "score";
        public const string PlayerEliminatedProp = "eliminated";
    }

    public static class ScoreExtensions
    {
        public static void SetScore(this Player player, int newScore)
        {
            Hashtable score = new Hashtable();  // using PUN's implementation of Hashtable
            score[PunPlayerScores.PlayerScoreProp] = newScore;

            player.SetCustomProperties(score);  // this locally sets the score and will sync it in-game asap.
        }

        public static void AddScore(this Player player, int scoreToAddToCurrent)
        {
            int current = player.GetScore();
            current += scoreToAddToCurrent;

            Hashtable score = new Hashtable();  // using PUN's implementation of Hashtable
            score[PunPlayerScores.PlayerScoreProp] = current;

            player.SetCustomProperties(score);  // this locally sets the score and will sync it in-game asap.
        }

        public static int GetScore(this Player player)
        {
            //Debug.Log(player);
            
            object score;
            if (player.CustomProperties.TryGetValue(PunPlayerScores.PlayerScoreProp, out score))
            {
                return (int)score;
            }

            return 0;
        }

        public static void SetEliminated(this Player player, bool eliminated)
        {
            Hashtable eliminatedState = new Hashtable();  // using PUN's implementation of Hashtable
            eliminatedState[PunPlayerScores.PlayerEliminatedProp] = eliminated;

            player.SetCustomProperties(eliminatedState);  // this locally sets the eliminated (state) and will sync it in-game asap.
        }

        public static bool GetIfEliminated(this Player player)
        {
            object eliminated;
            if (player.CustomProperties.TryGetValue(PunPlayerScores.PlayerEliminatedProp, out eliminated))
            {
                return (bool)eliminated;
            }

            return false;
        }
    }
}