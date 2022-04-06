using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Server.Models;
using System.Security.Claims;
using Server.Classes;
using Server.Classes.DataTransfer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Server.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GameHub : Hub
    {
        private static Random _random = new Random();
        private GameMaster gameMaster;
        public RizikoDbContext _context;
        public GameHub(GameMaster gm, RizikoDbContext ctx)
        {
            gameMaster = gm;
            _context = ctx;
        }

        public async Task SendInitUserGameState()
        {
            string username = this.Context.User?.Identity?.Name;
            GameControl game = new GameControl();
            foreach (GameControl g in gameMaster.activeGames)
            {
                foreach (Player p in g.players)
                    if (p.username == username)
                        game = g;
            }

            UserGameState ugs = game.FormInitUserGameState(username);
            await Groups.AddToGroupAsync(Context.ConnectionId, game.gameId);// nepotrebno???
            await Clients.Client(Context.ConnectionId).SendAsync("InitGameState", ugs);
        }
        public async Task UpdateUserGameStates(string username)
        {
            GameControl game = new GameControl();
            foreach (GameControl g in gameMaster.activeGames)
            {
                foreach (Player p in g.players)
                    if (p.username == username)
                        game = g;
            }
            foreach (Player p in game.players)
            {
                UserGameState ugs = game.FormUserGameState(p.username);
                await Clients.User(p.username).SendAsync("UpdateGameState", ugs);
            }
        }
        public async Task Draft(string terrName, int tanks)
        {
            string username = this.Context.User?.Identity?.Name;
            foreach (GameControl game in gameMaster.activeGames)
            {
                foreach (Player p in game.players)
                {
                    if (p.username == username && game.onTurn.username == username)
                    {
                        if (game.CheckPlayersHand(p))
                        {
                            game.Draft(terrName, tanks);
                        }
                        else
                        {
                            await Clients.User(p.username).SendAsync("Notify", "Please swap your cards!");
                        }
                    }
                }
            }
        }
        public async Task Attack(string from, string to, int tanks)
        {
            string username = this.Context.User?.Identity?.Name;
            foreach (GameControl game in gameMaster.activeGames)
                foreach (Player p in game.players)
                    if (p.username == username && game.onTurn.username == username)
                        game.Attack(p, from, to, tanks);
        }
        public async Task Fortify(string from, string to, int tanks)
        {
            string username = this.Context.User?.Identity?.Name;
            foreach (GameControl game in gameMaster.activeGames)
            {
                foreach (Player p in game.players)
                {
                    if (p.username == username && game.onTurn.username == username)
                    {
                        game.Fortify(from, to, tanks);
                    }
                }
            }
        }
        public async Task NextPhase()
        {
            string username = this.Context.User?.Identity?.Name;
            foreach (GameControl game in gameMaster.activeGames)
                foreach (Player p in game.players)
                    if (p.username == username && game.onTurn.username == username)
                        game.NextPhase();
        }
        public async Task Transfer(string from, string to, int tanks)
        {
            string username = this.Context.User?.Identity?.Name;
            foreach (GameControl game in gameMaster.activeGames)
                foreach (Player player in game.players)
                    if (username == player.username && game.onTurn.username == username)
                        game.Transfer(from, to, tanks);
        }
        public async Task AttackPending(string from, string to)
        {
            string username = this.Context.User?.Identity?.Name;
            GameControl game = new GameControl();
            foreach (GameControl g in gameMaster.activeGames)
                foreach (Player player in g.players)
                    if (username == player.username)
                        game = g;
            await Clients.Group(game.gameId).SendAsync("ReceiveAttackPending", from, to);
        }
        public async Task EndCurrentAttack()
        {
            string username = this.Context.User?.Identity?.Name;
            GameControl game = new GameControl();
            foreach (GameControl g in gameMaster.activeGames)
                foreach (Player player in g.players)
                    if (username == player.username)
                        game = g;
            await Clients.Group(game.gameId).SendAsync("ReceiveEndCurrentAttack");
        }
        public async Task SwapCards(List<Card> cards)
        {
            string username = this.Context.User?.Identity?.Name;
            foreach (GameControl game in gameMaster.activeGames)
                foreach (Player p in game.players)
                    if (p.username == username && game.onTurn.username == username && game.phase == "Draft")
                        game.SwapCards(cards);
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            string username = this.Context.User?.Identity?.Name;
            GameControl gameToDestroy = null;
            foreach (GameControl game in gameMaster.activeGames)
            {
                foreach (Player player in game.players)
                {
                    if (player.username == username)
                    {
                        game.disconnectedPlayers.Add(player);
                        if (game.disconnectedPlayers.Count == game.players.Count)
                        {
                            gameToDestroy = game;
                        }
                    }
                }
            }
            if (gameToDestroy != null)
                gameMaster.DestroyMe(gameToDestroy);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
