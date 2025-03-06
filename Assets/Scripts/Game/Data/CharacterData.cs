using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Data
{
    [System.Serializable]
    public class CharacterData
    {
        [SerializeField] private string characterId;
        [SerializeField] private string characterName;
        [SerializeField] private Sprite characterPortrait;
        [SerializeField] private GameObject characterPrefab;
        [SerializeField] private string teamColorSlot = "Body";

        public string CharacterId => characterId;
        public string CharacterName => characterName;
        public Sprite CharacterPortrait => characterPortrait;
        public GameObject CharacterPrefab => characterPrefab;
        public string TeamColorSlot => teamColorSlot;
    }

    [CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Game/Character Database")]
    public class CharacterDatabase : ScriptableObject
    {
        [SerializeField] private CharacterData[] characters;

        public CharacterData GetCharacterById(string id)
        {
            foreach (var character in characters)
            {
                if (character.CharacterId == id)
                    return character;
            }
            return null;
        }

        public int CharacterCount => characters.Length;
    }
}