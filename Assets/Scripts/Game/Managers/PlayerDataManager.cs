using UnityEngine;
using Assets.Scripts.Game.Data;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Managers;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Manages player data operations.
    /// </summary>
    public class PlayerDataManager : Singleton<PlayerDataManager>
    {
        public const bool DEFAULT_READY_STATUS = false;

        /// <summary>
        /// Creates a Player object with the current player's name and other properties.
        /// </summary>
        /// <returns>The Player object for lobby operations.</returns>
        public Player GetPlayer()
        {
            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerPrefs.GetString("PlayerName")) },
                    { "IsReady", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, DEFAULT_READY_STATUS.ToString()) },
                }
            };
        }
    }
}