using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Framework.Extensions
{
    /// <summary>
    /// Extension methods for the Unity Lobbies Player class
    /// </summary>
    public static class PlayerExtensions
    {
        /// <summary>
        /// Gets the player's name from their data
        /// </summary>
        public static string GetPlayerName(this Player player)
        {
            return player.Data != null && player.Data.TryGetValue("PlayerName", out var nameData)
                ? nameData.Value
                : "Unknown";
        }

        /// <summary>
        /// Checks if the player is ready
        /// </summary>
        public static bool IsReady(this Player player)
        {
            if (player.Data != null && player.Data.TryGetValue("IsReady", out var isReadyData))
                return isReadyData.Value == "true";

            return false;
        }

        /// <summary>
        /// Gets the player's team ID
        /// </summary>
        public static int GetTeamId(this Player player)
        {
            if (player.Data != null && player.Data.TryGetValue("TeamId", out var teamData))
                return int.TryParse(teamData.Value, out int teamId) ? teamId : 0;

            return 0;
        }

        /// <summary>
        /// Gets the player's character ID
        /// </summary>
        public static string GetCharacterId(this Player player)
        {
            return player.Data != null && player.Data.TryGetValue("CharacterId", out var characterData)
                ? characterData.Value
                : string.Empty;
        }

        /// <summary>
        /// Gets the player's color ID
        /// </summary>
        public static string GetColorId(this Player player)
        {
            return player.Data != null && player.Data.TryGetValue("ColorId", out var colorData)
                ? colorData.Value
                : string.Empty;
        }
    }
}