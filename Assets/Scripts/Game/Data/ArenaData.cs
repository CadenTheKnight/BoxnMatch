using UnityEngine;

namespace Assets.Scripts.Game.Data
{
    [System.Serializable]
    public class ArenaData
    {
        [SerializeField] private string arenaId;
        [SerializeField] private string arenaName;
        [SerializeField] private Sprite arenaPreviewImage;
        [SerializeField] private string sceneToLoad;

        public string ArenaId => arenaId;
        public string ArenaName => arenaName;
        public Sprite ArenaPreviewImage => arenaPreviewImage;
        public string SceneToLoad => sceneToLoad;
    }

    [CreateAssetMenu(fileName = "ArenaDatabase", menuName = "Game/Arena Database")]
    public class ArenaDatabase : ScriptableObject
    {
        [SerializeField] private ArenaData[] arenas;

        public ArenaData GetArenaById(string id)
        {
            foreach (var arena in arenas)
            {
                if (arena.ArenaId == id)
                    return arena;
            }
            return null;
        }

        public ArenaData GetArenaByIndex(int index)
        {
            if (index >= 0 && index < arenas.Length)
                return arenas[index];
            return null;
        }

        public int ArenaCount => arenas.Length;
    }
}