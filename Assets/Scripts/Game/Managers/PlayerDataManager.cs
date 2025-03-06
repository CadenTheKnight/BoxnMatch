using UnityEngine;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Core;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Manages player data operations.
    /// </summary>
    public class PlayerDataManager : Singleton<PlayerDataManager>
    {
        public const bool DEFAULT_READY_STATUS = false;

        /// <summary>
        /// Creates a Player object with the current player's name, ready status, current team, and ...? .
        /// </summary>
        /// <returns>The Player object for lobby operations.</returns>
        public Player GetNewPlayer()
        {
            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerPrefs.GetString("PlayerName")) },
                    { "IsReady", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, DEFAULT_READY_STATUS.ToString()) },
                    { "TeamId", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "0") },
                }
            };
        }
    }
}