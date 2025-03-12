using System;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Data
{
    [CreateAssetMenu(menuName = "Data/MapSelectionData", fileName = "MapSelectionData")]
    public class MapSelectionData : ScriptableObject
    {
        public List<MapInfo> Maps;
    }
}

[Serializable]
public struct MapInfo
{
    public Sprite MapThumbnail;
    public string MapName;
    public string MapSceneName;
}