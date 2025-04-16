using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Events;
using System.Collections.Generic;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Game.UI.Components.Options;
using Assets.Scripts.Game.UI.Components.Options.Selector;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas.CharacterCustomizationPanel
{
    public class CharacterCustomizationPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject character;
        private Material material;

        private void Start()
        {
            material = character.GetComponent<SpriteRenderer>().material;

            //UI Image components cannot render custom shaders :(
        }

        public void UpdateShader(Sprite boxSprite, CharacterShaderChannel channel)
        {
            material.SetTexture(channel.ToString(), boxSprite.texture);
        }
    }
}