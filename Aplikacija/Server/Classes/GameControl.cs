using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Timers;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;

namespace Server.Classes
{
    public class GameControl : IDisposable
    {
        public string gameId;
        public List<Player> players;
        public List<string> winConds=new List<string>();
        public Deck userCards;
        public string gameMap;
        public string gameType;

        public int turnDuration;
        public ActiveMap map;
        public Player onTurn;
        public string phase;

        public List<Player> disconnectedPlayers = new List<Player>();
        private List<Player> dead = new List<Player>();
        public int draftTanks;
        private Map mapModel;
        private static Random random = new Random();
        private List<int> nums = new List<int>();
        private List<int> usersTanks;
        private int countLostInAttack = 0;
        private int countLostInDefense = 0;
        private bool cardDrawn = false;
        public static Timer clock = new Timer();
        private readonly IHubContext<GameHub> gameHubContext;
        private string[] colors = { "#cc0000", "#33cc33", "#0066ff", "#ffff00", "#cc00cc", "#ffffff" };

        List<string> worldWinConds = new List<string> { "Defeat blue army", "Conquer Asia and South America", "Conquer North America and Australia",
            "Defeat yellow army","Defeat green army","Defeat red army", "Defeat purple army", "Defeat white army", "Conquer North America and Africa"
            };
        List<string> romeWinConds = new List<string> { "Defeat blue army", "Defeat yellow army", "Defeat green army", "Defeat red army", "Defeat purple army", "Defeat white army" ,
            "Conquer 24 territories"};

        private RizikoDbContext _ctx;
        private GameMaster gameMaster;
        private bool disposedValue;

        public GameControl() { }
        public GameControl(LobbyControl lobbyControl, IHubContext<GameHub> ghctx, RizikoDbContext ctx, GameMaster gm)
        {
            gameId = lobbyControl.lobbyId.ToString() + lobbyControl.host;
            players = lobbyControl.players;
            gameMap = lobbyControl.mapName;
            PlayersOrder(players);
            GenerateWinConds();
            gameType = lobbyControl.gameType;
            turnDuration = lobbyControl.turnDuration;
            _ctx = ctx;
            userCards = new Deck(gameMap, _ctx);
            userCards.Shuffle();
            onTurn = players[0];
            phase = "Draft";
            usersTanks = GetTanks();
            gameHubContext = ghctx;
            gameMaster = gm;
            clock.Elapsed += new ElapsedEventHandler(EndTurn);
            clock.Interval = turnDuration * 1000;
            clock.AutoReset = false;
        }
        public void GenerateWinConds()
        {
            if (gameMap == "World")
            {
                RemoveUnusableWinConds();
                List<int> rnds = GetRandomNumbers(worldWinConds.Count());
                for (int i = 0; i < players.Count(); i++)
                {
                    winConds.Add(worldWinConds[rnds[i]]);
                    if (winConds[i] == "Defeat red army" && players[i].color == colors[0])
                        SwapWinConds(winConds[i]);
                    else if (winConds[i] == "Defeat green army" && players[i].color == colors[1])
                        SwapWinConds(winConds[i]);
                    else if (winConds[i] == "Defeat blue army" && players[i].color == colors[2])
                        SwapWinConds(winConds[i]);
                    else if (winConds[i] == "Defeat yellow army" && players[i].color == colors[3])
                        SwapWinConds(winConds[i]);
                    else if (winConds[i] == "Defeat purple army" && players[i].color == colors[4])
                        SwapWinConds(winConds[i]);
                    else if (winConds[i] == "Defeat white army" && players[i].color == colors[5])
                        SwapWinConds(winConds[i]);
                }
            }
            else if (gameMap == "Rome")
            {
                RemoveUnusableWinConds();
                List<int> rnds = GetRandomNumbers(romeWinConds.Count());
                for (int i = 0; i < players.Count(); i++)
                {
                    winConds.Add(romeWinConds[rnds[i]]);
                    if (winConds[i] == "Defeat red army" && players[i].color == colors[0])
                        SwapWinConds(winConds[i]);
                    else if (winConds[i] == "Defeat green army" && players[i].color == colors[1])
                        SwapWinConds(winConds[i]);
                    else if (winConds[i] == "Defeat blue army" && players[i].color == colors[2])
                        SwapWinConds(winConds[i]);
                    else if (winConds[i] == "Defeat yellow army" && players[i].color == colors[3])
                        SwapWinConds(winConds[i]);
                    else if (winConds[i] == "Defeat purple army" && players[i].color == colors[4])
                        SwapWinConds(winConds[i]);
                    else if (winConds[i] == "Defeat white army" && players[i].color == colors[5])
                        SwapWinConds(winConds[i]);
                }
            }
        }

        internal void UpdateDbContext(RizikoDbContext context)
        {
            _ctx = context;
        }

        public void RemoveUnusableWinConds()
        {
            bool exist;
            foreach (string color in colors)
            {
                exist = false;
                foreach (Player player in players)
                {
                    if (player.color == color)
                        exist = true;
                }
                if (!exist) //ako ne postoji player sa color, nadje koja je to boja i izbaci taj zadatak
                {
                    if (gameMap == "Rome")
                    {
                        if (color == colors[0])
                            romeWinConds.Remove("Defeat red army");
                        if (color == colors[1])
                            romeWinConds.Remove("Defeat green army");
                        if (color == colors[2])
                            romeWinConds.Remove("Defeat blue army");
                        if (color == colors[3])
                            romeWinConds.Remove("Defeat yellow army");
                        if (color == colors[4])
                            romeWinConds.Remove("Defeat purple army");
                        if (color == colors[5])
                            romeWinConds.Remove("Defeat white army");
                    }
                    else if(gameMap == "World")
                    {
                        if (color == colors[0])
                            worldWinConds.Remove("Defeat red army");
                        if (color == colors[1])
                            worldWinConds.Remove("Defeat green army");
                        if (color == colors[2])
                            worldWinConds.Remove("Defeat blue army");
                        if (color == colors[3])
                            worldWinConds.Remove("Defeat yellow army");
                        if (color == colors[4])
                            worldWinConds.Remove("Defeat purple army");
                        if (color == colors[5])
                            worldWinConds.Remove("Defeat white army");
                    }
                }
            }
        }
        public void SwapWinConds(string winCnd)
        {
            for (int i = 0; i < winConds.Count; i++)
            {
                if (winCnd == winConds[i])
                {
                    if (gameMap == "World")
                    {
                        for (int k = 0; k < worldWinConds.Count; k++)
                        {
                            bool assigned = false;
                            for (int j = 0; j < winConds.Count; j++)
                                if (worldWinConds[k] == winConds[j])
                                    assigned = true;
                            if(!assigned)
                            {
                                winConds[i] = worldWinConds[k];
                                return;
                            }
                                    
                        }
                    }
                    else if (gameMap == "Rome")
                    {
                        for (int k = 0; k < romeWinConds.Count; k++)
                        {
                            bool assigned = false;
                            for (int j = 0; j < winConds.Count; j++)
                                if (romeWinConds[k] == winConds[j])
                                    assigned = true;
                            if (!assigned)
                            {
                                winConds[i] = romeWinConds[k];
                                return;
                            }

                        }
                    }
                    else
                        Console.WriteLine("Error: Wrong map name");
                }
                else
                    continue;
            }
        }
        public List<int> GetRandomNumbers(int count)
        {
            List<int> randomNumbers = new List<int>();
            int number;
            for (int i = 0; i < count; i++)
            {

                do
                    number = random.Next(count);
                while (randomNumbers.Contains(number));

                randomNumbers.Add(number);
            }

            return randomNumbers;
        }
        public async Task StartGame(LobbyMaster lm)
        {
            for (int i = 0; i < lm.activeLobbies.Count; i++)//brise se lobby kad se udje u game
                if (lm.activeLobbies[i].lobbyId.ToString() + lm.activeLobbies[i].host == gameId)
                    lm.activeLobbies.RemoveAt(i);
            ArrangeTerritories();
            ArrangeTanks();
            
            await StartTurn();
        }
        public async Task EndInsertIntoDB()
        {
            try
            {
                gameMaster.GiveMeContext(this);
                Game game = new Game();
                Leaderboard leaderboard = new Leaderboard();
                User user = new User();
                if (gameType == "Casual")
                {
                    game.IsCasual = true;
                    game.IsFriendly = false;
                    game.IsRanked = false;
                    game.Map = _ctx.Map.Where(x => x.MapName == map.mapName).Include(x => x.Continents).ThenInclude(x => x.Provinces).ThenInclude(x => x.ConnectedTo).FirstOrDefault();
                    user = _ctx.User.Where(x => x.UserName == onTurn.username).FirstOrDefault();
                    game.Winner = user;
                    foreach (Player player in players)
                    {
                        user = _ctx.User.Where(x => x.UserName == player.username).FirstOrDefault();
                        game.Players.Add(user);
                    }
                    foreach (Player player in dead)
                    {
                        user = _ctx.User.Where(x => x.UserName == player.username).FirstOrDefault();
                        game.Players.Add(user);
                    }
                }
                else if (gameType == "Friendly")
                {
                    game.IsCasual = false;
                    game.IsFriendly = true;
                    game.IsRanked = false;
                    game.Map = _ctx.Map.Where(x => x.MapName == map.mapName).Include(x => x.Continents).ThenInclude(x => x.Provinces).ThenInclude(x => x.ConnectedTo).FirstOrDefault();
                    
                    game.Winner = user;
                    foreach (Player player in players)
                    {
                        user = _ctx.User.Where(x => x.UserName == player.username).FirstOrDefault();
                        game.Players.Add(user);
                    }
                    foreach (Player player in dead)
                    {
                        user = _ctx.User.Where(x => x.UserName == player.username).FirstOrDefault();
                        game.Players.Add(user);
                    }
                }
                else
                {
                    game.IsCasual = false;
                    game.IsFriendly = false;
                    game.IsRanked = true;
                    game.Map = _ctx.Map.Where(x => x.MapName == map.mapName).Include(x => x.Continents).ThenInclude(x => x.Provinces).ThenInclude(x => x.ConnectedTo).FirstOrDefault();
                    user = _ctx.User.Where(x => x.UserName == onTurn.username).FirstOrDefault();
                    game.Winner = user;
                    if (!user.UserName.Contains("Guest"))
                    {
                        leaderboard = _ctx.Leaderboard.Where(x => x.User.UserName == game.Winner.UserName).FirstOrDefault();
                        leaderboard.ELO += 30;
                        _ctx.Leaderboard.Update(leaderboard);
                    }
                    foreach (Player player in players)
                    {
                        user = _ctx.User.Where(x => x.UserName == player.username).FirstOrDefault();
                        if (!user.UserName.Contains("Guest"))
                        {
                            leaderboard = _ctx.Leaderboard.Where(x => x.User.UserName == player.username).FirstOrDefault();
                            leaderboard.ELO -= 5;
                            _ctx.Leaderboard.Update(leaderboard);
                        }
                        game.Players.Add(user);
                    }
                    foreach (Player player in dead)
                    {
                        user = _ctx.User.Where(x => x.UserName == player.username).FirstOrDefault();
                        if (!user.UserName.Contains("Guest"))
                        {
                            leaderboard = _ctx.Leaderboard.Where(x => x.User.UserName == player.username).FirstOrDefault();
                            leaderboard.ELO -= 15;
                            _ctx.Leaderboard.Update(leaderboard);
                        }
                        game.Players.Add(user);
                    }
                }

                _ctx.Game.Add(game);
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task LoadMap()
        {
            
            if (gameMap == "World")
            {
                mapModel = _ctx.Map.Where(x => x.MapName == "World").Include(x => x.Continents).ThenInclude(x => x.Provinces).ThenInclude(x => x.ConnectedTo).FirstOrDefault();
                map = new ActiveMap(mapModel);
            }
            else if (gameMap == "Rome")
            {
                mapModel = _ctx.Map.Where(x => x.MapName == "Rome").Include(x => x.Continents).ThenInclude(x => x.Provinces).ThenInclude(x => x.ConnectedTo).FirstOrDefault();
                map = new ActiveMap(mapModel);
            }
            else
                Console.WriteLine("Error");
            
      
        }
        public void NextPhase()
        {
            if (phase == "Draft")
            {
                phase = "Attack";
                NotifyEveryone();
            }
            else if (phase == "Attack")
            {
                phase = "Fortify";
                NotifyEveryone();
            }
            else
            {
                EndTurn();
            }
                
        }
        public void ArrangeTerritories()
        {
            nums.Clear();
            int j = 0;
            foreach (ActiveContinent continent in map.continents)
            {
                nums = GetRandomNumbers(players.Count);
                for (int i = 0; i < continent.territories.Count; i++)
                {
                    if (j == players.Count)
                        j = 0;
                    continent.territories[i].currentHolder = players[nums[j++]];
                }
            }
            nums.Clear();
        }
        public List<int> GetTanks()
        {
            List<int> t;
            if (players.Count == 6)
                t = new List<int>() { 20, 20, 20, 20, 20, 20 };
            else if (players.Count == 5)
                t = new List<int>() { 25, 25, 25, 25, 25 };
            else if (players.Count == 4)
                t = new List<int>() { 30, 30, 30, 30 };
            else if (players.Count == 3)
                t = new List<int>() { 35, 35, 35 };
            else if (players.Count == 2)
                t = new List<int>() { 50, 50 };
            else if (players.Count == 1)
                t = new List<int>() { 100 };
            else
            { t = null; Console.WriteLine("Error"); }
            return t;
        }
        public int GetNumberOfProvinces(ActiveMap m)
        {
            int numOfProvinces = 0;
            foreach (ActiveContinent c in m.continents)
                foreach (Territory t in c.territories)
                    numOfProvinces++;
            return numOfProvinces;
        }
        public void ArrangeTanks()
        {
            int rnd;
            foreach (ActiveContinent continent in map.continents)
            {
                foreach (Territory terr in continent.territories)
                {
                    terr.tanks = 1;
                    for (int i = 0; i < players.Count; i++)
                        if (terr.currentHolder.username == players[i].username)
                            usersTanks[i]--;
                }
            }
            for (int i = 0; i < players.Count; i++)
            {
                while (usersTanks[i] != 0)
                {
                    foreach (ActiveContinent continent in map.continents)
                    {
                        foreach (Territory terr in continent.territories)
                        {
                            rnd = random.Next(2);
                            if (rnd == 1 && usersTanks[i] > 0 && players[i].username == terr.currentHolder.username)
                            {
                                terr.tanks++;
                                usersTanks[i]--;
                            }
                        }
                    }
                }
            }
        }
        public void PlayersOrder(List<Player> ps)
        {
            int n = ps.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Player player = ps[k];
                ps[k] = ps[n];
                ps[n] = player;
            }
            onTurn = ps[0];
        }
        public bool CheckWin(string winCond)
        {
            int countHold = 0;
            int countExist = 0;
            bool hasAsia = false, hasAfrica = false, hasSouthAmerica = false, hasNorthAmerica = false, hasEurope = false, hasAustralia = false;
            if (winCond == "Defeat red army")
            {
                foreach (Player player in players)
                    if (player.color == colors[0])//znaci da je jos u partiju igrac sa tom bojom
                        return false;
                return true;
            }
            else if (winCond == "Defeat green army")
            {
                foreach (Player player in players)
                    if (player.color == colors[1])
                        return false;
                return true;
            }
            else if (winCond == "Defeat blue army")
            {
                foreach (Player player in players)
                    if (player.color == colors[2])
                        return false;
                return true;
            }
            else if (winCond == "Defeat yellow army")
            {
                foreach (Player player in players)
                    if (player.color == colors[3])
                        return false;
                return true;
            }
            else if (winCond == "Defeat purple army")
            {
                foreach (Player player in players)
                    if (player.color == colors[4])
                        return false;
                return true;
            }
            else if (winCond == "Defeat white army")
            {
                foreach (Player player in players)
                    if (player.color == colors[5])
                        return false;
                return true;
            }
            else
            {
                if (gameMap == "World")
                {
                    if (winCond == "Conquer Asia and South America")
                    {
                        foreach (ActiveContinent continent in map.continents)
                        {
                            if (continent.continentName == "Asia")
                            {
                                foreach (Territory terr in continent.territories)
                                {
                                    countExist++;
                                    if (onTurn.username == terr.currentHolder.username)
                                        countHold++;
                                }
                                if (countHold == countExist)
                                    hasAsia = true;
                                countExist = 0;
                                countHold = 0;
                            }
                            if (continent.continentName == "South America")
                            {
                                foreach (Territory terr in continent.territories)
                                {
                                    countExist++;
                                    if (onTurn.username == terr.currentHolder.username)
                                        countHold++;
                                }
                                if (countHold == countExist)
                                    hasSouthAmerica = true;
                                countExist = 0;
                                countHold = 0;
                            }
                        }
                        if (hasAsia && hasSouthAmerica)
                            return true;
                        return false;
                    }
                    else if (winCond == "Conquer North America and Australia")
                    {
                        foreach (ActiveContinent continent in map.continents)
                        {
                            if (continent.continentName == "North America")
                            {
                                foreach (Territory terr in continent.territories)
                                {
                                    countExist++;
                                    if (onTurn.username == terr.currentHolder.username)
                                        countHold++;
                                }
                                if (countHold == countExist)
                                    hasNorthAmerica = true;
                                countExist = 0;
                                countHold = 0;
                            }
                            if (continent.continentName == "Australia")
                            {
                                foreach (Territory terr in continent.territories)
                                {
                                    countExist++;
                                    if (onTurn.username == terr.currentHolder.username)
                                        countHold++;
                                }
                                if (countHold == countExist)
                                    hasAustralia = true;
                                countExist = 0;
                                countHold = 0;
                            }
                        }
                        if (hasNorthAmerica && hasAustralia)
                            return true;
                        return false;
                    }
                    else if (winCond == "Conquer North America and Africa")
                    {
                        foreach (ActiveContinent continent in map.continents)
                        {
                            if (continent.continentName == "North America")
                            {
                                foreach (Territory terr in continent.territories)
                                {
                                    countExist++;
                                    if (onTurn.username == terr.currentHolder.username)
                                        countHold++;
                                }
                                if (countHold == countExist)
                                    hasNorthAmerica = true;
                                countExist = 0;
                                countHold = 0;
                            }
                            if (continent.continentName == "Africa")
                            {
                                foreach (Territory terr in continent.territories)
                                {
                                    countExist++;
                                    if (onTurn.username == terr.currentHolder.username)
                                        countHold++;
                                }
                                if (countHold == countExist)
                                    hasAfrica = true;
                                countExist = 0;
                                countHold = 0;
                            }
                        }
                        if (hasNorthAmerica && hasAfrica)
                            return true;
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (gameMap == "Rome")
                {
                    if(winCond == "Conquer 24 territories")
                    {
                        foreach(ActiveContinent continent in map.continents)
                        {
                            foreach(Territory terr in continent.territories)
                            {
                                if (onTurn.username == terr.currentHolder.username)
                                    countHold++;
                                if (countHold > 23)
                                    return true;
                                
                            }
                        }
                        countHold = 0;
                    }
                    return false;
                }
                else
                {
                    Console.WriteLine("Wrong map name");
                    return false;
                }
            }
            
        }
        public async Task StartTurn()
        {
            
            phase = "Draft";
            cardDrawn = false;
            draftTanks = CalculateTanks(onTurn);
            NotifyEveryone();
            clock.Start();
        }
        public async Task CheckVictory()
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (onTurn.username == players[i].username && CheckWin(winConds[i]))
                {
                    await EndInsertIntoDB();
                    await gameHubContext.Clients.Group(gameId).SendAsync("Win", onTurn);
                    gameMaster.DestroyMe(this);

                }
            }
        }
        private async void CheckLose()
        {
            List<Player> toRemove=new List<Player>();
            foreach (Player p in players)
            {
                bool hasTerritory = false;
                foreach (ActiveContinent c in map.continents)
                {
                    foreach (Territory t in c.territories)
                    {
                        if (t.currentHolder.username == p.username)
                            hasTerritory = true;
                    }
                }
                if(!hasTerritory)
                {
                    dead.Add(p);
                    toRemove.Add(p);
                    winConds.RemoveAt(players.IndexOf(p)); 
                    await gameHubContext.Clients.User(p.username).SendAsync("Lose", p);
                }
            }
            if(toRemove.Count>0)
            {
                foreach(Player pl in toRemove)
                    players.Remove(pl);
                NotifyEveryone();
            }
        }
        public async void EndTurn(Object source, ElapsedEventArgs e)
        {
            CheckLose();
            await CheckVictory();
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].username == onTurn.username && i == players.Count - 1)
                    onTurn = players[0];
                if (players[i].username == onTurn.username && i != players.Count - 1)
                    onTurn = players[++i];
            }
            await StartTurn();
        }
        public async Task EndTurn()
        {
            CheckLose();
            await CheckVictory();
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].username == onTurn.username && i == players.Count - 1)
                    onTurn = players[0];
                if (players[i].username == onTurn.username && i != players.Count - 1)
                    onTurn = players[++i];
            }
            clock.Stop();
            await StartTurn();
        }
        public async void NotifyEveryone()
        {
            foreach (Player player in players)
            {
                UserGameState ugs = FormUserGameState(player.username);
                await gameHubContext.Clients.User(player.username).SendAsync("UpdateGameState", ugs);
            }
        }
        public async void NotifyPlayer(Player p)
        {
            foreach (Player player in players)
            {
                if (p.username == player.username)
                {
                    UserGameState ugs = FormUserGameState(p.username);
                    await gameHubContext.Clients.User(p.username).SendAsync("UpdateGameState", ugs);
                }
            }
        }
        public int GetPlayersPosition(Player player)
        {
            for (int i = 0; i < players.Count; i++)
                if (player.username == players[i].username)
                    return i;
            return -1;
        }
        public UserGameState FormInitUserGameState(string username)
        {
            List<Card> playerHand = new List<Card>();
            for (int i = 0; i < userCards.deck.Length; i++)
                if (userCards.deck[i].holder != null && userCards.deck[i].holder.username == username)
                    playerHand.Add(userCards.deck[i]);
            string winCond = "";
            for (int i = 0; i < players.Count(); i++)
            {
                if (players[i].username == username)
                {
                    winCond = winConds[i];
                }
            }
            UserGameState ugs = new UserGameState(this.gameId, this.players, winCond, playerHand, this.map, this.turnDuration, this.onTurn, this.phase, this.draftTanks);
            return ugs;
        }
        public UserGameState FormUserGameState(string username)
        {
            List<Card> playerHand = new List<Card>();
            for (int i = 0; i < userCards.deck.Length; i++)
                if (userCards.deck[i].holder != null && userCards.deck[i].holder.username == username)
                    playerHand.Add(userCards.deck[i]);
            UserGameState ugs = new UserGameState(this.gameId, this.players, playerHand, this.map, this.turnDuration, this.onTurn, this.phase, this.draftTanks);
            return ugs;
        }
        public int CalculateTanks(Player player)
        {
            int tanks = 0;
            int count = 0;
            foreach (ActiveContinent continent in map.continents)
            {
                foreach (Territory terr in continent.territories)
                {
                    if (player.username == terr.currentHolder.username)
                        count++;
                }
            }
            tanks = count / 3;
            count = 0;
            if (tanks < 3)
                tanks = 3;
            if (map.mapName == "World")
            {
                foreach (ActiveContinent continent in map.continents)
                {
                    if (continent.continentName == "Asia")
                    {
                        foreach (Territory terr in continent.territories)
                            if (player.username == terr.currentHolder.username)
                                count++;
                        if (count == continent.territories.Count)
                            tanks += 7;
                        count = 0;
                    }
                    if (continent.continentName == "Africa")
                    {
                        foreach (Territory terr in continent.territories)
                            if (player.username == terr.currentHolder.username)
                                count++;
                        if (count == continent.territories.Count)
                            tanks += 3;
                        count = 0;
                    }
                    if (continent.continentName == "Europe")
                    {
                        foreach (Territory terr in continent.territories)
                            if (player.username == terr.currentHolder.username)
                                count++;
                        if (count == continent.territories.Count)
                            tanks += 5;
                        count = 0;
                    }
                    if (continent.continentName == "North America")
                    {
                        foreach (Territory terr in continent.territories)
                            if (player.username == terr.currentHolder.username)
                                count++;
                        if (count == continent.territories.Count)
                            tanks += 5;
                        count = 0;
                    }
                    if (continent.continentName == "South America")
                    {
                        foreach (Territory terr in continent.territories)
                            if (player.username == terr.currentHolder.username)
                                count++;
                        if (count == continent.territories.Count)
                            tanks += 2;
                        count = 0;
                    }
                    if (continent.continentName == "Australia")
                    {
                        foreach (Territory terr in continent.territories)
                            if (player.username == terr.currentHolder.username)
                                count++;
                        if (count == continent.territories.Count)
                            tanks += 2;
                        count = 0;
                    }
                }
            }
            else if (map.mapName == "Rome")
            {
                foreach (ActiveContinent continent in map.continents)
                {
                    if (continent.continentName == "Africa")
                    {
                        foreach (Territory terr in continent.territories)
                            if (player.username == terr.currentHolder.username)
                                count++;
                        if (count == continent.territories.Count)
                            tanks += 3;
                        count = 0;
                    }
                    if (continent.continentName == "Middle East")
                    {
                        foreach (Territory terr in continent.territories)
                            if (player.username == terr.currentHolder.username)
                                count++;
                        if (count == continent.territories.Count)
                            tanks += 8;
                        count = 0;
                    }
                    if (continent.continentName == "Balkans")
                    {
                        foreach (Territory terr in continent.territories)
                            if (player.username == terr.currentHolder.username)
                                count++;
                        if (count == continent.territories.Count)
                            tanks += 7;
                        count = 0;
                    }
                    if (continent.continentName == "Italia")
                    {
                        foreach (Territory terr in continent.territories)
                            if (player.username == terr.currentHolder.username)
                                count++;
                        if (count == continent.territories.Count)
                            tanks += 4;
                        count = 0;
                    }
                    if (continent.continentName == "Galia")
                    {
                        foreach (Territory terr in continent.territories)
                            if (player.username == terr.currentHolder.username)
                                count++;
                        if (count == continent.territories.Count)
                            tanks += 5;
                        count = 0;
                    }
                    if (continent.continentName == "Spain")
                    {
                        foreach (Territory terr in continent.territories)
                            if (player.username == terr.currentHolder.username)
                                count++;
                        if (count == continent.territories.Count)
                            tanks += 3;
                        count = 0;
                    }
                    if (continent.continentName == "Britannia")
                    {
                        foreach (Territory terr in continent.territories)
                            if (player.username == terr.currentHolder.username)
                                count++;
                        if (count == continent.territories.Count)
                            tanks += 2;
                        count = 0;
                    }
                }
            }
            else
            {
                Console.WriteLine("Error");
                return -1;
            }
            return tanks;
        }
        public void Draft(string t, int tanks)
        {
            if(tanks<=draftTanks)
            {
                foreach (ActiveContinent continent in map.continents)
                    foreach (Territory terr in continent.territories)
                        if (terr.name == t)
                            terr.tanks += tanks;
                draftTanks -= tanks;
            }
            if (draftTanks == 0)
                phase = "Attack";
            NotifyEveryone();
        }
        public bool CheckPlayersHand(Player p)
        {
            int count = 0;
            for (int i = 0; i < userCards.deck.Length; i++)
            {
                if (userCards.deck[i].holder!=null && userCards.deck[i].holder.username == p.username)
                    count++;
            }
            if (count < 6)
                return true;
            else
                return false;
        }
        public async Task Attack(Player player, string fromName, string toName, int tanks)
        {
            countLostInAttack = 0;
            countLostInDefense = 0;
            Territory from = FindTerritory(fromName);
            Territory to = FindTerritory(toName);
            if (player != to.currentHolder && from.currentHolder != to.currentHolder && from.tanks >= 2)
            {
                if (tanks == 0)//bira se blitz
                {
                    while (!AttackBlitz(from, to)) ; //prebacen jedan tenk ako je blitz uspesan
                    //explode
                    if (countLostInDefense != 0)
                        await gameHubContext.Clients.Group(gameId).SendAsync("Explode", to.name, countLostInDefense);
                    if (countLostInAttack != 0)
                        await gameHubContext.Clients.Group(gameId).SendAsync("Explode", from.name, countLostInAttack);
                    if (from.currentHolder == to.currentHolder)//osvojio teritoriju
                    {
                        if (cardDrawn == false)
                        {
                            for (int i = 0; i < userCards.deck.Length; i++)//dobije karticu
                            {
                                if (userCards.deck[i].valid == 1)
                                {
                                    userCards.deck[i].AttachHolder(player);
                                    cardDrawn = true;
                                    break;
                                }
                            }
                        }
                        NotifyEveryone();//svima se salje ugs
                        await gameHubContext.Clients.User(onTurn.username).SendAsync("AttackSuccesful", from.name, to.name);//poziva se transfer preko AttackSuccesful
                        NotifyPlayer(player);//zbog karticu
                        return; //nema razlog dva puta da salje NotifyEveryone??
                    }
                    NotifyEveryone();
                    await gameHubContext.Clients.User(onTurn.username).SendAsync("AttackUnsuccesful");
                }
                else
                {
                    if(!NormalAttack(from,to,tanks))//samo napao ali nije osvojio teritoriju i moze jos da napada
                    {
                        NotifyEveryone();
                    }
                    else
                    {
                        if (to.tanks == 0)//osvojio teritoriju
                        {
                            to.currentHolder = from.currentHolder;
                            to.tanks++;
                            from.tanks--;
                            if (cardDrawn == false)
                            {
                                for (int i = 0; i < userCards.deck.Length; i++)
                                {
                                    if (userCards.deck[i].valid == 1)
                                    {
                                        userCards.deck[i].AttachHolder(player);
                                        cardDrawn = true;
                                        break;
                                    }
                                }
                            }
                            NotifyEveryone();
                            await gameHubContext.Clients.User(onTurn.username).SendAsync("AttackSuccesful", from.name, to.name);
                            NotifyPlayer(player);
                        }
                        else//nije osvojio
                        {
                            await gameHubContext.Clients.User(onTurn.username).SendAsync("AttackUnsuccesful");
                            NotifyEveryone();
                        }
                    }
                }
            }
            CheckLose();
            await CheckVictory();
        }
        public List<int> RollDice(int tanks)
        {
            List<int> values = new List<int>();
            int rnd1,rnd2,rnd3;
            if (tanks > 2)
            {
                rnd1 = random.Next(6) + 1;
                rnd2 = random.Next(6) + 1;
                rnd3 = random.Next(6) + 1;
                values.Add(rnd1); values.Add(rnd2); values.Add(rnd3);
                values.Sort();
                values.Reverse();
                return values;
            }
            else if (tanks == 2)
            {
                rnd1 = random.Next(6) + 1;
                rnd2 = random.Next(6) + 1;
                values.Add(rnd1); values.Add(rnd2);
                values.Sort();
                values.Reverse();
                return values;
            }
            else
            {
                rnd1 = random.Next(6) + 1;
                values.Add(rnd1);
                values.Sort();
                values.Reverse();
                return values;
            }
        }
        public bool AttackBlitz(Territory from, Territory to)
        {
            bool endOfAttack = false;
            List<int> attackingPlayerDices = RollDice(from.tanks-1);
            List<int> defendingPlayerDices = RollDice(to.tanks);
            int attackLost = 0;
            int defenseLost = 0;
            for (int i = 0; i < attackingPlayerDices.Count; i++)
            {
                if (i < defendingPlayerDices.Count)
                {
                    if (defendingPlayerDices[i] - attackingPlayerDices[i] < 0)//defenselost++ i attack lost++ su bili zamenjeni
                    {
                        defenseLost++;
                        countLostInDefense++;
                    }
                    else
                    {
                        attackLost++;
                        countLostInAttack++;
                    }
                }
            }
            foreach(ActiveContinent continent in map.continents)
            {
                foreach(Territory terr in continent.territories)
                {
                    if (terr.name == from.name)
                    { 
                        from.tanks -= attackLost;
                        if (from.tanks == 1)
                        {
                            gameHubContext.Clients.User(onTurn.username).SendAsync("AttackUnsuccesful");
                            endOfAttack = true;
                        }  
                    }
                    if (terr.name == to.name)
                    { 
                        to.tanks -= defenseLost;
                        if (to.tanks == 0)
                        {
                            endOfAttack = true;
                            to.currentHolder = from.currentHolder;
                            to.tanks++; //da se prebaci jedan tenk
                            from.tanks--;
                        }
                    }
                }
            }
            return endOfAttack;
        }
        public void Fortify(string from, string to, int tanks)
        {
            foreach(ActiveContinent continent in map.continents)
            {
                foreach(Territory terr in continent.territories)
                {
                    if (terr.name == from)
                    {
                        foreach (Territory neighbour in terr.neighbors)
                        {
                            Territory neigh = FindTerritory(neighbour.name);
                            if (neighbour.name == to && neigh.currentHolder.username == terr.currentHolder.username && (terr.tanks-tanks) > 0)
                            {
                                terr.tanks -= tanks;
                                neigh.tanks += tanks;
                            }
                        }
                    }
                }
            }
            NotifyEveryone();
            EndTurn();
        }
        public Territory FindTerritory(string name)
        {
            foreach (ActiveContinent c in map.continents)
            {
                foreach (Territory t in c.territories)
                {
                    if (t.name == name)
                        return t;
                }
            }
            return null;
        }
        public void Transfer(string from,string to,int tanks)
        {
            foreach (ActiveContinent continent in map.continents)
            {
                foreach (Territory terr in continent.territories)
                {
                    if (terr.name == from)
                    {
                        foreach (Territory neighbour in terr.neighbors)
                        {
                            Territory neigh = FindTerritory(neighbour.name);
                            if (neighbour.name == to && neigh.currentHolder.username == terr.currentHolder.username)
                            {
                                terr.tanks -= tanks;
                                neigh.tanks += tanks;
                            }
                        }
                    }
                }
            }
            NotifyEveryone();
        }
        public bool NormalAttack(Territory from, Territory to, int tanks)
        {
            bool attackEnded = false;
            List<int> attackingPlayerDices = RollDice(tanks);
            List<int> defendingPlayerDices = RollDice(to.tanks);
            SendDiceValues(attackingPlayerDices, defendingPlayerDices, from);
            int attackLost = 0;
            int defenseLost = 0;
            for (int i = 0; i < attackingPlayerDices.Count; i++)
            {
                if (i < defendingPlayerDices.Count)
                {
                    if (defendingPlayerDices[i] - attackingPlayerDices[i] < 0)
                        defenseLost++;
                    else
                        attackLost++;
                }
            }
            //explode
            if(defenseLost != 0)
                gameHubContext.Clients.Group(gameId).SendAsync("Explode", to.name, defenseLost);
            if (attackLost != 0)
                gameHubContext.Clients.Group(gameId).SendAsync("Explode", from.name, attackLost);
            foreach (ActiveContinent continent in map.continents)
            {
                foreach (Territory terr in continent.territories)
                {
                    if (terr.name == from.name)
                        from.tanks -= attackLost;
                    if (terr.name == to.name)
                        to.tanks -= defenseLost;
                    if (from.tanks == 1 || to.tanks == 0)
                        attackEnded = true;
                }
            }
            return attackEnded;
        }
        public async Task SendDiceValues(List<int> attackDice, List<int> defenseDice, Territory from)
        {
            foreach (Player player in players)
                if (player.username == from.currentHolder.username)
                    await gameHubContext.Clients.User(player.username).SendAsync("DiceRoll", attackDice, defenseDice);
        }
        public void SwapCards(List<Card> cards)
        {
            int numOfTanks;
            if (cards.Count == 3)
            {
                foreach(ActiveContinent continent in map.continents)
                {
                    foreach(Territory terr in continent.territories)
                    {
                        if (terr.name == cards[0].territoryName && onTurn.username == terr.currentHolder.username)
                            terr.tanks += 2;
                        if (terr.name == cards[1].territoryName && onTurn.username == terr.currentHolder.username)
                            terr.tanks += 2;
                        if (terr.name == cards[2].territoryName && onTurn.username == terr.currentHolder.username)
                            terr.tanks += 2;
                    }
                }
                int tankCards = 0;
                int soliderCards = 0;
                int planeCards = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    if (cards[i].type == "Tank")
                        tankCards++;
                    else if (cards[i].type == "Plane")
                        planeCards++;
                    else
                        soliderCards++;
                }
                if (tankCards == 3)
                    numOfTanks = 6;
                else if (soliderCards == 3)
                    numOfTanks = 4;
                else if (planeCards == 3)
                    numOfTanks = 8;
                else
                    numOfTanks = 10;
                draftTanks += numOfTanks;
                for (int i = 0; i < cards.Count; i++)
                    foreach(Card c in userCards.deck)
                        if (c.territoryName == cards[i].territoryName)
                            c.DettachHolder();
            }
            else
                draftTanks += 0;
            NotifyEveryone();
        }

        internal void StopTimer()
        {
            clock.Stop();
            clock.Close();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~GameControl()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}