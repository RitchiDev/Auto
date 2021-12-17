using UnityEngine;
using Photon.Realtime;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace Andrich.UtilityScripts
{
    public class PlayerProperties : MonoBehaviour
    {
        public const string ScoreProperty = "PlayerScore";
        public const string DeathsProperty = "PlayerDeaths";
        public const string IsReadyProperty = "PlayerIsReady";
        public const string IsEliminatedProperty = "PlayerIsEliminated";
        public const string SelectedPrimaryMaterialProperty = "SelectedPrimaryMaterial";
        public const string SelectedSecondaryMaterialProperty = "SelectedSecondaryMaterial";
    }

    public static class ScoreExtensions
    {
        public static void SetScore(this Player player, int newScore)
        {
            PhotonHashtable score = new PhotonHashtable();  // using PUN's implementation of Hashtable
            score[PlayerProperties.ScoreProperty] = newScore;

            player.SetCustomProperties(score);  // this locally sets the score and will sync it in-game asap.
        }

        public static void AddScore(this Player player, int scoreToAddToCurrent)
        {
            int current = player.GetScore();
            current = current + scoreToAddToCurrent;

            PhotonHashtable score = new PhotonHashtable();  // using PUN's implementation of Hashtable
            score[PlayerProperties.ScoreProperty] = current;

            player.SetCustomProperties(score);  // this locally sets the score and will sync it in-game asap.
        }

        public static int GetScore(this Player player)
        {
            object score;
            if (player.CustomProperties.TryGetValue(PlayerProperties.ScoreProperty, out score))
            {
                return (int)score;
            }

            return 0;
        }
    }

    public static class DeathsExtensions
    {
        public static void SetDeaths(this Player player, int newDeaths)
        {
            PhotonHashtable deaths = new PhotonHashtable();  // using PUN's implementation of Hashtable
            deaths[PlayerProperties.DeathsProperty] = newDeaths;

            player.SetCustomProperties(deaths);  // this locally sets the deaths and will sync it in-game asap.
        }

        public static void AddDeath(this Player player, int deathsToAdd)
        {
            int current = player.GetDeaths();
            current = current + deathsToAdd;

            PhotonHashtable deaths = new PhotonHashtable();  // using PUN's implementation of Hashtable
            deaths[PlayerProperties.DeathsProperty] = current;

            player.SetCustomProperties(deaths);  // this locally sets the deaths and will sync it in-game asap.
        }

        public static int GetDeaths(this Player player)
        {
            object deaths;
            if (player.CustomProperties.TryGetValue(PlayerProperties.DeathsProperty, out deaths))
            {
                return (int)deaths;
            }

            return 0;
        }
    }

    public static class ReadyToggleExtensions
    {
        public static void SetReadyState(this Player player, bool isReady)
        {
            PhotonHashtable ready = new PhotonHashtable();  // using PUN's implementation of Hashtable
            ready[PlayerProperties.IsReadyProperty] = isReady;

            player.SetCustomProperties(ready);  // this locally sets the ready state and will sync it in-game asap.
        }

        public static bool GetIfReady(this Player player)
        {
            object ready;
            if (player.CustomProperties.TryGetValue(PlayerProperties.IsReadyProperty, out ready))
            {
                return (bool)ready;
            }

            return false;
        }
    }

    public static class EliminatedExtensions
    {
        public static void SetEliminated(this Player player, bool isEliminated)
        {
            PhotonHashtable eliminated = new PhotonHashtable();  // using PUN's implementation of Hashtable
            eliminated[PlayerProperties.IsEliminatedProperty] = isEliminated;

            player.SetCustomProperties(eliminated);  // this locally sets the eliminated state and will sync it in-game asap.
        }

        public static bool GetIfEliminated(this Player player)
        {
            object eliminated;
            if (player.CustomProperties.TryGetValue(PlayerProperties.IsEliminatedProperty, out eliminated))
            {
                return (bool)eliminated;
            }

            return false;
        }
    }

    public static class CarMaterialExtensions
    {
        public static void SetPrimaryMaterialIndex(this Player player, int index)
        {
            PhotonHashtable primaryIndex = new PhotonHashtable();  // using PUN's implementation of Hashtable
            primaryIndex[PlayerProperties.SelectedPrimaryMaterialProperty] = index;

            player.SetCustomProperties(primaryIndex);  // this locally sets the primary material index and will sync it in-game asap.
        }

        public static void ChangePrimaryMaterialIndex(this Player player, int amount)
        {
            int current = player.GetPrimaryMaterialIndex();
            current = current + amount;

            PhotonHashtable primaryIndex = new PhotonHashtable();  // using PUN's implementation of Hashtable
            primaryIndex[PlayerProperties.SelectedPrimaryMaterialProperty] = current;

            player.SetCustomProperties(primaryIndex);  // this locally sets the primary index and will sync it in-game asap.
        }

        public static int GetPrimaryMaterialIndex(this Player player)
        {
            object primaryIndex;
            if (player.CustomProperties.TryGetValue(PlayerProperties.SelectedPrimaryMaterialProperty, out primaryIndex))
            {
                return (int)primaryIndex;
            }

            return 0;
        }

        public static void SetSecondaryMaterialIndex(this Player player, int index)
        {
            PhotonHashtable secondaryIndex = new PhotonHashtable();  // using PUN's implementation of Hashtable
            secondaryIndex[PlayerProperties.SelectedSecondaryMaterialProperty] = index;

            player.SetCustomProperties(secondaryIndex);  // this locally sets the secondary material index and will sync it in-game asap.
        }

        public static void ChangeSecondaryMaterialIndex(this Player player, int amount)
        {
            int current = player.GetSecondaryMaterialIndex();
            current = current + amount;

            PhotonHashtable secondaryIndex = new PhotonHashtable();  // using PUN's implementation of Hashtable
            secondaryIndex[PlayerProperties.SelectedSecondaryMaterialProperty] = current;

            player.SetCustomProperties(secondaryIndex);  // this locally sets the secondary index and will sync it in-game asap.

        }

        public static int GetSecondaryMaterialIndex(this Player player)
        {
            object secondaryIndex;
            if (player.CustomProperties.TryGetValue(PlayerProperties.SelectedSecondaryMaterialProperty, out secondaryIndex))
            {
                return (int)secondaryIndex;
            }

            return 0;
        }
    }
}
