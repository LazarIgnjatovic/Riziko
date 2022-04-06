using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Server.Models;
using Server.Classes;
using Server.Classes.DataTransfer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Server.Hubs
{
    public class MenuHub : Hub
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private readonly Random _random = new Random();
        private RizikoDbContext _context;
        private readonly TokenConfiguration _tokenConfiguration;
        private GameMaster gameMaster;
        public MenuHub(RizikoDbContext context, SignInManager<User> signInManager,UserManager<User> userManager, TokenConfiguration tokenConfiguration,GameMaster master)
        {
            gameMaster = master;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenConfiguration = tokenConfiguration;

            InitRomeMap();
        }


        public async Task Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                SignInResult res;
                if (!username.Contains("Guest"))
                {
                    res = await _signInManager.CheckPasswordSignInAsync(user, password, false);
                }
                else
                {
                    res = await _signInManager.CheckPasswordSignInAsync(user, "GuestPass", false);
                }
                if (res.Succeeded)
                {
                    var u = _context.User.Where(p => p.UserName == username).FirstOrDefault();
                    string token = JwtAuthorization.GenerateToken(u.UserName, _tokenConfiguration);
                    Settings settings = _context.Settings.Where(p => p.User == u).FirstOrDefault();
                    SettingsTransfer set = new SettingsTransfer(settings);

                    if (!username.Contains("Guest"))
                    {
                        Leaderboard l = _context.Leaderboard.Where(l => l.User == u).FirstOrDefault();
                        await Clients.Caller.SendAsync("UserInfo", u.UserName, token, set, l.ELO);
                    }
                    else
                    {
                        await Clients.Caller.SendAsync("GuestInfo", u.UserName, token, set);
                    }

                }
                else
                {
                    if (!username.Contains("Guest"))
                        await Clients.Caller.SendAsync("LoginFailed");
                    else
                        await CreateGuest();
                }


            }
            else
            {
                if (!username.Contains("Guest"))
                    await Clients.Caller.SendAsync("LoginFailed");
                else
                    await CreateGuest();
            }

        }

        public async Task Register(string username, string password, string email)
        {
            
            Settings set = new Settings
            {
                //default
                Avatar = 0,
                Sound = 100,
                PrefColor = "#ffffff",
                ActionConfirm = true
            };
            var user = new User
            {
                UserName = username,
                Email = email,
            };
            Leaderboard lead = new Leaderboard
            {
                ELO = 1000,
                GamesPlayed = 0
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                User u = _context.User.Where(p => p.UserName == username).FirstOrDefault();
                set.User = u;
                _context.Settings.Add(set);
                lead.User = u;
                _context.Leaderboard.Add(lead);
                await _context.SaveChangesAsync();
                await Login(username, password);
            }    
        }

        public async Task Logout()
        {
            //await _signInManager.SignOutAsync();
        }
        public async Task CreateGuest()
        {
            var guest = new User
            {
                UserName = "Guest" + _random.Next(100000).ToString()
            };
            Settings set = new Settings
            {
                //default
                Avatar = 0,
                Sound = 100,
                PrefColor = "#ffffff",
                ActionConfirm = true
            };
            var result = await _userManager.CreateAsync(guest,"GuestPass");
            if (result.Succeeded)
            {
                User u = _context.User.Where(p => p.UserName == guest.UserName).FirstOrDefault();
                set.User = u;
                _context.Settings.Add(set);
                await _context.SaveChangesAsync();
                await Login(guest.UserName, null);
            }
            else
                await Clients.Caller.SendAsync("Notify",false, "Cannot create Guest user.");//da se doda fja
        }
        
        public async Task UpdateSettings(bool ActionConfirm,int Avatar,string PrefColor,float Sound,string Username)
        {
            User user = _context.User.Where(u => u.UserName == Username).FirstOrDefault();
            Settings set = _context.Settings.Where(s => s.User == user).FirstOrDefault();
            set.ActionConfirm = ActionConfirm;
            set.Avatar = Avatar;
            set.PrefColor = PrefColor;
            set.Sound = Sound;
            
            _context.Settings.Update(set);
            await _context.SaveChangesAsync();
            await Clients.Caller.SendAsync("Notify", true, "Settings updated");
        }
        public async Task GetLeaderboard(string username)
        {
            List<LeaderboardTransfer> toSend=new List<LeaderboardTransfer>();

            var lb=_context.Leaderboard.OrderByDescending(p => p.ELO).Include(b => b.User).ToList();
            for(int i=0;i<5;i++)
            {
                if (i < lb.Count())
                {
                    toSend.Add(new LeaderboardTransfer(lb.ElementAt(i).ELO, lb.ElementAt(i).User.UserName, i + 1));
                }
                else
                    toSend.Add(new LeaderboardTransfer(0, "???", i + 1));
            }
            if(!username.Contains("Guest"))
            {
                Leaderboard l = lb.Where(p => p.User.UserName == username).FirstOrDefault();
                toSend.Add(new LeaderboardTransfer(l.ELO, username, lb.IndexOf(l) + 1));
            }
            await Clients.Caller.SendAsync("Leaderboard", toSend);
        }
        public async Task DeleteGuest(string user)
        {
            if(user.Contains("Guest"))
            {
                User us = await _userManager.FindByNameAsync(user);
                await _userManager.DeleteAsync(us);
            }
        }
        public async Task CheckReconnect(string username)
        {
            foreach (GameControl game in gameMaster.activeGames)
                foreach (Player player in game.players)
                    if (player.username == username)
                        await Clients.Caller.SendAsync("HasReconnect",game.map.mapName);
        }
        public async Task InitRomeMap()
        {
            Map m = _context.Map.Where(p => p.MapName == "Rome").FirstOrDefault();
            if (m == null)
            {
                Map Rome = new Map();
                Rome.MapName = "Rome";
                Continent Africa = new Continent();
                Continent MiddleEast = new Continent();
                Continent Balkans = new Continent();
                Continent Italia = new Continent();
                Continent Gallia = new Continent();
                Continent Spain = new Continent();
                Continent Britannia = new Continent();

                Africa.BonusTanks = 3;
                Africa.ContinentName = "Africa";
                MiddleEast.BonusTanks = 8;
                MiddleEast.ContinentName = "Middle East";
                Balkans.BonusTanks = 7;
                Balkans.ContinentName = "Balkans";
                Italia.BonusTanks = 4;
                Italia.ContinentName = "Italia";
                Gallia.BonusTanks = 5;
                Gallia.ContinentName = "Gallia";
                Spain.BonusTanks = 3;
                Spain.ContinentName = "Spain";
                Britannia.BonusTanks = 2;
                Britannia.ContinentName = "Britannia";
                Rome.Continents.Add(Africa);
                Rome.Continents.Add(MiddleEast);
                Rome.Continents.Add(Balkans);
                Rome.Continents.Add(Italia);
                Rome.Continents.Add(Gallia);
                Rome.Continents.Add(Spain);
                Rome.Continents.Add(Britannia);

                Province Aegyptus = new Province();
                Province AfricaP = new Province();
                Province Cyrenacia = new Province();
                Province Mauretania = new Province();
                Aegyptus.ProvinceName = "Aegyptus";
                AfricaP.ProvinceName = "Africa";
                Cyrenacia.ProvinceName = "Cyrenacia";
                Mauretania.ProvinceName = "Mauretania";
                Africa.Provinces.Add(Aegyptus);
                Africa.Provinces.Add(AfricaP);
                Africa.Provinces.Add(Cyrenacia);
                Africa.Provinces.Add(Mauretania);

                Province Galatia = new Province();
                Galatia.ProvinceName = "Galatia";
                Province Arabia = new Province();
                Arabia.ProvinceName = "Arabia";
                Province Asia = new Province();
                Asia.ProvinceName = "Asia";
                Province Bythia = new Province();
                Bythia.ProvinceName = "Bythia";
                Province Cappadocia = new Province();
                Cappadocia.ProvinceName = "Cappadocia";
                Province Cilicia = new Province();
                Cilicia.ProvinceName = "Cilicia";
                Province Iudaea = new Province();
                Iudaea.ProvinceName = "Iudaea";
                Province Lycia = new Province();
                Lycia.ProvinceName = "Lycia";
                Province Syria = new Province();
                Syria.ProvinceName = "Syria";
                MiddleEast.Provinces.Add(Galatia);
                MiddleEast.Provinces.Add(Arabia);
                MiddleEast.Provinces.Add(Asia);
                MiddleEast.Provinces.Add(Bythia);
                MiddleEast.Provinces.Add(Cappadocia);
                MiddleEast.Provinces.Add(Cilicia);
                MiddleEast.Provinces.Add(Iudaea);
                MiddleEast.Provinces.Add(Lycia);
                MiddleEast.Provinces.Add(Syria);

                Province Dacia = new Province();
                Dacia.ProvinceName = "Dacia";
                Province Dalmatia = new Province();
                Dalmatia.ProvinceName = "Dalmatia";
                Province Dardania = new Province();
                Dardania.ProvinceName = "Dardania";
                Province Epirus = new Province();
                Epirus.ProvinceName = "Epirus";
                Province Macedonia = new Province();
                Macedonia.ProvinceName = "Macedonia";
                Province Moesia = new Province();
                Moesia.ProvinceName = "Moesia";
                Province Pannonia = new Province();
                Pannonia.ProvinceName = "Pannonia";
                Province Thracia = new Province();
                Thracia.ProvinceName = "Thracia";
                Balkans.Provinces.Add(Dacia);
                Balkans.Provinces.Add(Dalmatia);
                Balkans.Provinces.Add(Dardania);
                Balkans.Provinces.Add(Epirus);
                Balkans.Provinces.Add(Macedonia);
                Balkans.Provinces.Add(Moesia);
                Balkans.Provinces.Add(Pannonia);
                Balkans.Provinces.Add(Thracia);

                Province ItaliaP = new Province();
                ItaliaP.ProvinceName = "Italia";
                Province Naples = new Province();
                Naples.ProvinceName = "Naples";
                Province RomeP = new Province();
                RomeP.ProvinceName = "Rome";
                Province Sardinia = new Province();
                Sardinia.ProvinceName = "Sardinia";
                Province Sicilia = new Province();
                Sicilia.ProvinceName = "Sicilia";
                Italia.Provinces.Add(ItaliaP);
                Italia.Provinces.Add(Naples);
                Italia.Provinces.Add(RomeP);
                Italia.Provinces.Add(Sardinia);
                Italia.Provinces.Add(Sicilia);

                Province Aqiutania = new Province();
                Aqiutania.ProvinceName = "Aquitania";
                Province Germania = new Province();
                Germania.ProvinceName = "Germania";
                Province Lugdunensis = new Province();
                Lugdunensis.ProvinceName = "Lugdunensis";
                Province Narbonesis = new Province();
                Narbonesis.ProvinceName = "Narbonesis";
                Province Noricum = new Province();
                Noricum.ProvinceName = "Noricum";
                Province Raetia = new Province();
                Raetia.ProvinceName = "Raetia";
                Gallia.Provinces.Add(Aqiutania);
                Gallia.Provinces.Add(Germania);
                Gallia.Provinces.Add(Lugdunensis);
                Gallia.Provinces.Add(Narbonesis);
                Gallia.Provinces.Add(Noricum);
                Gallia.Provinces.Add(Raetia);

                Province Terrraconensis = new Province();
                Terrraconensis.ProvinceName = "Terraconensis";
                Province Baeltica = new Province();
                Baeltica.ProvinceName = "Baeltica";
                Province Lusitania = new Province();
                Lusitania.ProvinceName = "Lusitania";
                Spain.Provinces.Add(Terrraconensis);
                Spain.Provinces.Add(Baeltica);
                Spain.Provinces.Add(Lusitania);

                Province EastBritannia = new Province();
                EastBritannia.ProvinceName = "East Britannia";
                Province WestBritannia = new Province();
                WestBritannia.ProvinceName = "West Britannia";
                Britannia.Provinces.Add(EastBritannia);
                Britannia.Provinces.Add(WestBritannia);

                Mauretania.Connected.Add(Baeltica);
                Mauretania.Connected.Add(AfricaP);
                AfricaP.Connected.Add(Mauretania);
                AfricaP.Connected.Add(Cyrenacia);
                AfricaP.Connected.Add(Sicilia);
                Cyrenacia.Connected.Add(AfricaP);
                Cyrenacia.Connected.Add(Aegyptus);
                Aegyptus.Connected.Add(Cyrenacia);
                Aegyptus.Connected.Add(Arabia);
                Aegyptus.Connected.Add(Iudaea);
                Iudaea.Connected.Add(Aegyptus);
                Iudaea.Connected.Add(Arabia);
                Iudaea.Connected.Add(Syria);
                Arabia.Connected.Add(Iudaea);
                Arabia.Connected.Add(Syria);
                Arabia.Connected.Add(Aegyptus);///
                Syria.Connected.Add(Iudaea);
                Syria.Connected.Add(Arabia);
                Syria.Connected.Add(Cilicia);
                Syria.Connected.Add(Cappadocia);
                Cappadocia.Connected.Add(Syria);
                Cappadocia.Connected.Add(Cilicia);
                Cappadocia.Connected.Add(Bythia);
                Cappadocia.Connected.Add(Galatia);
                Cilicia.Connected.Add(Syria);
                Cilicia.Connected.Add(Cappadocia);
                Cilicia.Connected.Add(Galatia);
                Cilicia.Connected.Add(Lycia);
                Cilicia.Connected.Add(Asia);
                Lycia.Connected.Add(Cilicia);
                Lycia.Connected.Add(Asia);
                Galatia.Connected.Add(Cappadocia);
                Galatia.Connected.Add(Cilicia);
                Galatia.Connected.Add(Asia);
                Galatia.Connected.Add(Bythia);
                Asia.Connected.Add(Lycia);
                Asia.Connected.Add(Cilicia);
                Asia.Connected.Add(Galatia);
                Asia.Connected.Add(Bythia);
                Asia.Connected.Add(Thracia);
                Bythia.Connected.Add(Cappadocia);
                Bythia.Connected.Add(Galatia);
                Bythia.Connected.Add(Asia);
                Bythia.Connected.Add(Thracia);
                Thracia.Connected.Add(Bythia);
                Thracia.Connected.Add(Asia);
                Thracia.Connected.Add(Moesia);
                Thracia.Connected.Add(Dardania);
                Thracia.Connected.Add(Macedonia);
                Macedonia.Connected.Add(Thracia);
                Macedonia.Connected.Add(Dardania);
                Macedonia.Connected.Add(Dalmatia);
                Macedonia.Connected.Add(Epirus);
                Epirus.Connected.Add(Macedonia);
                Moesia.Connected.Add(Thracia);
                Moesia.Connected.Add(Dardania);
                Moesia.Connected.Add(Dacia);
                Dardania.Connected.Add(Moesia);
                Dardania.Connected.Add(Thracia);
                Dardania.Connected.Add(Macedonia);
                Dardania.Connected.Add(Dalmatia);
                Dardania.Connected.Add(Dacia);
                Dardania.Connected.Add(Pannonia);
                Dacia.Connected.Add(Moesia);
                Dacia.Connected.Add(Dardania);
                Dalmatia.Connected.Add(Macedonia);
                Dalmatia.Connected.Add(Pannonia);
                Dalmatia.Connected.Add(Dardania);
                Dardania.Connected.Add(ItaliaP);
                Pannonia.Connected.Add(Dardania);
                Pannonia.Connected.Add(Dalmatia);
                Pannonia.Connected.Add(ItaliaP);
                Pannonia.Connected.Add(Noricum);
                ItaliaP.Connected.Add(Dalmatia);
                ItaliaP.Connected.Add(Pannonia);
                ItaliaP.Connected.Add(Noricum);
                ItaliaP.Connected.Add(Raetia);
                ItaliaP.Connected.Add(Narbonesis);
                ItaliaP.Connected.Add(Sardinia);
                ItaliaP.Connected.Add(RomeP);
                RomeP.Connected.Add(ItaliaP);
                RomeP.Connected.Add(Sardinia);
                RomeP.Connected.Add(Naples);
                Sardinia.Connected.Add(ItaliaP);
                Sardinia.Connected.Add(RomeP);
                Naples.Connected.Add(RomeP);
                Naples.Connected.Add(Sicilia);
                Sicilia.Connected.Add(Naples);
                Sicilia.Connected.Add(AfricaP);
                Noricum.Connected.Add(Pannonia);
                Noricum.Connected.Add(ItaliaP);
                Noricum.Connected.Add(Raetia);
                Raetia.Connected.Add(Noricum);
                Raetia.Connected.Add(ItaliaP);
                Raetia.Connected.Add(Narbonesis);
                Raetia.Connected.Add(Germania);
                Narbonesis.Connected.Add(ItaliaP);
                Narbonesis.Connected.Add(Raetia);
                Narbonesis.Connected.Add(Germania);
                Narbonesis.Connected.Add(Aqiutania);
                Narbonesis.Connected.Add(Terrraconensis);
                Terrraconensis.Connected.Add(Narbonesis);
                Terrraconensis.Connected.Add(Aqiutania);
                Terrraconensis.Connected.Add(Baeltica);
                Terrraconensis.Connected.Add(Lusitania);
                Lusitania.Connected.Add(Terrraconensis);
                Lusitania.Connected.Add(Baeltica);
                Baeltica.Connected.Add(Terrraconensis);
                Baeltica.Connected.Add(Lusitania);
                Baeltica.Connected.Add(Mauretania);
                Aqiutania.Connected.Add(Terrraconensis);
                Aqiutania.Connected.Add(Narbonesis);
                Aqiutania.Connected.Add(Germania);
                Aqiutania.Connected.Add(Lugdunensis);
                Germania.Connected.Add(Aqiutania);
                Germania.Connected.Add(Lugdunensis);
                Germania.Connected.Add(Narbonesis);
                Germania.Connected.Add(Raetia);
                Germania.Connected.Add(EastBritannia);
                Lugdunensis.Connected.Add(WestBritannia);
                Lugdunensis.Connected.Add(Aqiutania);
                Lugdunensis.Connected.Add(Germania);
                WestBritannia.Connected.Add(EastBritannia);
                WestBritannia.Connected.Add(Lugdunensis);
                EastBritannia.Connected.Add(Germania);
                EastBritannia.Connected.Add(WestBritannia);

                _context.Map.Add(Rome);
                await _context.SaveChangesAsync();

            }
        }
    }
}
