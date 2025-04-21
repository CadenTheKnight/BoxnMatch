// // using Steamworks;
// using Assets.Scripts.Game.Types;
// using System.Collections.Generic;
// using Unity.Services.Lobbies.Models;

// namespace Assets.Scripts.Game.Data
// {
//     /// <summary>
//     /// Represents the data of player in the lobby system.
//     /// </summary>
//     public class PlayerData
//     {
//         // public CSteamID SteamId { get; set; }
//         public Team Team { get; set; }
//         public ReadyStatus ReadyStatus { get; set; }
//         public ConnectionStatus ConnectionStatus { get; set; }

//         // public PlayerData(CSteamID steamId)
//         // {
//         //     SteamId = steamId;
//         //     Team = Team.Blue;
//         //     ReadyStatus = ReadyStatus.NotReady;
//         //     ConnectionStatus = ConnectionStatus.Disconnected;
//         // }

//         public Dictionary<string, PlayerDataObject> Serialize()
//         {
//             return new Dictionary<string, PlayerDataObject>
//             {
//                 // { "SteamId", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, SteamId.ToString()) },
//                 { "Team", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, ((int)Team).ToString()) },
//                 { "ReadyStatus", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, ((int)ReadyStatus).ToString()) },
//                 { "ConnectionStatus", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, ((int)ConnectionStatus).ToString()) }
//             };
//         }
//     }
// }