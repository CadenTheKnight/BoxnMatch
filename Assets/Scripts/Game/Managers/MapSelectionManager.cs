using UnityEngine;
using Assets.Scripts.Game.Data;

namespace Assets.Scripts.Game.Managers
{
    public class MapSelectionManager : MonoBehaviour
    {
        [SerializeField] private MapSelectionData mapSelectionData;

        public MapInfo GetMapInfo(int mapIndex)
        {
            if (mapSelectionData == null || mapIndex < 0 || mapIndex >= mapSelectionData.Maps.Count)
                Debug.LogError($"Invalid map index: {mapIndex}");

            return mapSelectionData.Maps[mapIndex];
        }
    }
}