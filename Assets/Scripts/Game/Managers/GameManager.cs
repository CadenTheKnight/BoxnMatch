using UnityEngine;
using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Events;
using Assets.Scripts.Framework.Core;
using Unity.Services.Authentication;

namespace Assets.Scripts.Game.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        public async Task StartGame(GameMode gameMode)
        {
            if (gameMode == GameMode.PvP)
            {
                GameObject relayManagerObject = new("RelayManager");
                relayManagerObject.AddComponent<RelayManager>();
                if (AuthenticationService.Instance.PlayerId == GameLobbyManager.Instance.Lobby.HostId)
                {
                    Task<string> createTask = RelayManager.Instance.CreateRelay(GameLobbyManager.Instance.Lobby.MaxPlayers);
                    string relayJoinCode = await createTask;
                    if (relayJoinCode != null) GameEvents.InvokeGameStarted(true, relayJoinCode);
                    else GameEvents.InvokeGameStarted(false, null);
                }
            }
            else GameEvents.InvokeGameStarted(true, null);
        }

        public async Task JoinGame(string relayJoinCode)
        {
            await RelayManager.Instance.JoinRelay(relayJoinCode);
        }
    }
}