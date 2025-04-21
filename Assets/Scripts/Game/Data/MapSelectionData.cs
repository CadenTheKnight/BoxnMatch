using System;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Data
{
    [CreateAssetMenu(menuName = "Resources/Data/MapSelectionData", fileName = "MapSelectionData")]
    public class MapSelectionData : ScriptableObject
    {
        [SerializeField] private List<Map> maps = new();

        public List<Map> Maps => maps;

        public Map GetMap(int index)
        {
            if (index < 0 || index >= maps.Count)
            {
                Debug.LogWarning($"Map index {index} out of range (0-{maps.Count - 1})");
                return default;
            }
            return maps[index];
        }

        public int GetMapIndex(string mapName)
        {
            for (int i = 0; i < maps.Count; i++)
                if (maps[i].Name == mapName)
                    return i;

            Debug.LogWarning($"Map with scene name {mapName} not found.");
            return -1;
        }


        public Map GetMapBySceneName(string sceneName)
        {
            return maps.Find(map => map.Name == sceneName);
        }
    }

    [Serializable]
    public struct Map
    {
        public Sprite Thumbnail;
        public string Name;
    }
}