// using Assets.Scripts.Game.Data;
// using UnityEngine;

// namespace Assets.Scripts.Game.Types
// {
//     public class GameSettings
//     {
//         public Map SelectedMap { get; set; } = Map.Default;
//         public int RoundCount { get; set; } = 3;
//         public int RoundTime { get; set; 
//         public GameMode Mode { get; set; } = GameMode.Teams;

//         // Validation methods
//         public void ValidateSettings()
//         {
//             RoundCount = Mathf.Clamp(RoundCount, 1, 10);
//             RoundTime = Mathf.Clamp(RoundTime, 30, 300);
//         }

//         // Network serialization
//         public Dictionary<string, string> Serialize()
//         {
//             return new Dictionary<string, string>
//         {
//             { "MapIndex", ((int)SelectedMap).ToString() },
//             { "RoundCount", RoundCount.ToString() },
//             { "RoundTime", RoundTime.ToString() },
//             { "GameMode", ((int)Mode).ToString() }
//         };
//         }

//         // Deserialization
//         public void Deserialize(Dictionary<string, DataObject> data)
//         {
//             if (data.TryGetValue("MapIndex", out var mapObj) && int.TryParse(mapObj.Value, out int mapIndex))
//                 SelectedMap = (Map)mapIndex;

//             if (data.TryGetValue("RoundCount", out var roundsObj) && int.TryParse(roundsObj.Value, out int rounds))
//                 RoundCount = rounds;

//             if (data.TryGetValue("RoundTime", out var timeObj) && int.TryParse(timeObj.Value, out int time))
//                 RoundTime = time;

//             if (data.TryGetValue("GameMode", out var modeObj) && int.TryParse(modeObj.Value, out int mode))
//                 Mode = (GameMode)mode;

//             ValidateSettings();
//         }
//     }
// }