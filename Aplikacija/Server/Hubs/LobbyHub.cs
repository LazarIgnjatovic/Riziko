using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Server.Classes;
using Server.Classes.DataTransfer;
using Server.Models;

namespace Server.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LobbyHub : Hub
    {
        private static Random random = new Random();
        private LobbyMaster lobbyMaster;
        private GameMaster gameMaster;
        private RizikoDbContext _context;
        private readonly IHubContext<GameHub> gameHubContext;
        private string[] colors = { "#cc0000", "#33cc33", "#0066ff", "#ffff00", "#cc00cc", "#ffffff" };
        public LobbyHub(LobbyMaster lm, GameMaster gm, RizikoDbContext ctx, IHubContext<GameHub> ghctx)
        {
            _context = ctx;
            lobbyMaster = lm;
            gameMaster = gm;
            gameHubContext = ghctx;
        }
        public async Task LeaveLobby(Player user)
        {
            foreach(LobbyControl lobby in lobbyMaster.activeLobbies)
            {
                if (user.username == lobby.host)
                {
                    string lobbyName = lobby.lobbyId.ToString() + lobby.host;
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyName);//**********
                    await DeleteLobby(lobby);
                    await Clients.Group(lobbyName).SendAsync("HostLeft", "Host left, Lobby is no more");
                    
                    return;
                }
                else
                {
                    foreach (Player player in lobby.players)
                    {
                        if (user.username == player.username)
                        {
                            lobby.players.Remove(player);
                            string lobbyName = lobby.lobbyId.ToString() + lobby.host;
                            await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyName);//**********
                            await Clients.Group(lobbyName).SendAsync("ReceiveLobby", lobby);
                            return;
                        }
                    }
                }
            }
        }
        public async Task DeleteLobby(LobbyControl lobbyControl)
        {
            lobbyMaster.DestroyMe(lobbyControl);
        }
        public async Task UpdateLobby(LobbyControl lobbyControl)
        {
            for (int i = 0; i < lobbyMaster.activeLobbies.Count(); i++) 
            {
                if(lobbyMaster.activeLobbies[i].lobbyId == lobbyControl.lobbyId)
                {
                    lobbyMaster.activeLobbies[i] = lobbyControl;
                    await Clients.Group(lobbyControl.lobbyId.ToString()+lobbyControl.host).SendAsync("ReceiveLobby", lobbyControl);
                }
            }
        }
        public async Task SendLobby(Player user,string gameType, bool isHost)
        {
            if (isHost == true)
            {
                LobbyControl lobby = CreateLobby(user, gameType);
                string lobbyName = lobby.lobbyId.ToString() + lobby.host;//**********
                await Groups.AddToGroupAsync(Context.ConnectionId, lobbyName);
                await Clients.Caller.SendAsync("ReceiveLobby", lobby);
            }
            else
            {
                LobbyControl lobby = JoinLobby(user, gameType);
                if (lobby == null)
                    await Clients.Caller.SendAsync("NoLobbyFound", 5);
                else
                {
                    string lobbyName = lobby.lobbyId.ToString() + lobby.host;//**********
                    await Groups.AddToGroupAsync(Context.ConnectionId, lobbyName);
                    await Clients.Group(lobbyName).SendAsync("ReceiveLobby", lobby);
                }
            }
        }
        public async Task SearchByCode(Player player,string joinCode)
        {
            foreach (LobbyControl lobby in lobbyMaster.activeLobbies)
            {
                if (joinCode == lobby.joinCode)
                {
                    ColorChecker(lobby, player);
                    lobby.players.Add(player);
                    string lobbyName = lobby.lobbyId.ToString() + lobby.host;
                    await Groups.AddToGroupAsync(Context.ConnectionId, lobbyName);
                    await Clients.Group(lobbyName).SendAsync("ReceiveLobby", lobby);
                    return;
                }
            }
            await Clients.Caller.SendAsync("WrongCode", "Wrong code!");
        }
        public async Task StartGame(LobbyControl lobby)
        {
            GameControl game = new GameControl(lobby,gameHubContext,_context,gameMaster);
            gameMaster.activeGames.Add(game);
            await game.LoadMap();
            await game.StartGame(lobbyMaster);
            string lobbyName = lobby.lobbyId.ToString() + lobby.host;
            await Clients.Group(lobbyName).SendAsync("GameStarted", lobby.mapName);
        }
        public LobbyControl CreateLobby(Player user, string gameType)
        {
            Random _random = new Random();
            int rnd = _random.Next(10000);
            for (int i = 0; i < lobbyMaster.activeLobbies.Count(); i++)
            {
                if (lobbyMaster.activeLobbies[i].lobbyId == rnd)
                {
                    rnd = _random.Next(10000);
                    i = 0;
                }
            }
            if (gameType == "Friendly")
            {
                string joinCode = GenerateCode();
                LobbyControl lobby = new LobbyControl(rnd, user.username, gameType, joinCode);
                lobby.players.Add(user);

                lobbyMaster.activeLobbies.Add(lobby);
                return lobby;
            }
            else
            {
                LobbyControl lobby = new LobbyControl(rnd, user.username, gameType);
                lobby.players.Add(user);
                lobbyMaster.activeLobbies.Add(lobby);
                return lobby;
            }
        }
        public LobbyControl JoinLobby(Player p, string gameType)
        {
            foreach (LobbyControl lobby in lobbyMaster.activeLobbies)
                foreach (Player player in lobby.players)
                    if (player.username == p.username)
                        return lobby;

            foreach (LobbyControl lobby in lobbyMaster.activeLobbies)
            {
                if (gameType == lobby.gameType && lobby.players.Count() < 6)
                {
                    ColorChecker(lobby, p);
                    lobby.players.Add(p);
                    return lobby;
                }
            }
            return null;
        }

        private void ColorChecker(LobbyControl lobby, Player p)
        {
            bool taken = false;
            foreach (Player player in lobby.players)
            {
                if (p.color == player.color)
                    taken = true;
            }
            if (taken)
            {
                foreach (string color in colors)//boje
                {
                    bool colorTaken = false;
                    foreach (Player player in lobby.players)
                    {
                        if (color == player.color)
                            colorTaken = true;
                    }
                    if (!colorTaken)
                    {
                        p.color = color;
                        break;
                    }
                }
            }
        }

        public string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 4)
              .Select(s => s[random.Next(chars.Length)]).ToArray());
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            string username = this.Context.User?.Identity?.Name;
            LobbyControl l = new LobbyControl();
            foreach (LobbyControl lobby in lobbyMaster.activeLobbies)
            {
                foreach (Player player in lobby.players)
                {
                    if (player.username == username)
                    { 
                        lobby.disconnectedPlayers.Add(player);
                        if (lobby.disconnectedPlayers.Count == lobby.players.Count)
                        {
                            l = lobby;
                            DeleteLobby(lobby);
                        }
                    }
                }
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
