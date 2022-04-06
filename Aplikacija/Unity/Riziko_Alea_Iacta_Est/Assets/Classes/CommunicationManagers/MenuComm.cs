using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using UnityEngine.UI;
using System;
using Microsoft.Extensions.DependencyInjection;
using Assets.Classes.DataTransfer;

namespace Assets.Classes
{
    public class MenuComm
    {
        MenuUI ui;
        string TOKEN;
        public HubConnection conn;
        public List<Exception> exceptionList;

        public MenuComm(MenuUI u)
        {
            ui = u;
            Init();
        }

        public async void Init()
        {
            conn = new HubConnectionBuilder()
               .WithUrl("http://c143e997c3e0.ngrok.io/MenuHub", options =>
               {
                   options.AccessTokenProvider = () => Task.FromResult(TOKEN);
               })
               .AddNewtonsoftJsonProtocol()
               .WithAutomaticReconnect()
               .Build();

            conn.Closed += async (error) =>
            {
                await Task.Delay(new System.Random().Next(0, 5) * 1000);
                await conn.StartAsync();
            };
            conn.Reconnected += async (error) => 
            {
                ui.EndLoad();
            };
            conn.Reconnecting += async (error) =>
            {
                ui.StartLoad("Connection lost. Reconnecting...");
            };

            conn.On<bool,string>("Notify", (isGood,mess) =>
            {
                ui.Notify(isGood, mess);
                ui.ClosePannels();
                
            });
            conn.On<string, string, SettingsTransfer,int>("UserInfo", (username,token,settings,elo) =>
               {
                   ui.SaveUser(username, token, settings, elo.ToString());
                   TOKEN = token;
                   Settings set = new Settings(settings);
                   ui.UserInfo(username, set,elo.ToString());
               });
            conn.On<string, string, SettingsTransfer>("GuestInfo", (username, token, settings) =>
            {
                ui.SaveUser(username, token, settings, "???");
                PlayerPrefs.SetString("guestUsername", username);
                TOKEN = token;
                Settings set = new Settings(settings);
                ui.UserInfo(username, set, "???");
            });
            conn.On<List<LeaderboardTransfer>>("Leaderboard", (list) =>
             {
                 List<LeaderboardTransfer> leaderboard = list;
                 ui.ShowLeaderboard(leaderboard);
             });
            conn.On("LoginFailed", () =>
            {
                if (ui.user != null)
                    ui.Notify(false, "Login failed! Please check your creditentials.");
                else
                    ui.GuestLogin();
            });
            conn.On<string>("HasReconnect", (map) => {
                ui.Reconnect(map);
            });////////////////////

            try
            {
                await conn.StartAsync();
            }
            catch(Exception ex)
            {
                ui.Notify(false, "Cannot connect to server. Check your connection and restrart the game.");
            }
            
        }

        public async void Login(string username, string pass)
        {
            try
            {
                ui.StartLoad("Waiting for server...");
                PlayerPrefs.SetString("username", username);
                PlayerPrefs.SetString("password", pass);
                await conn.InvokeAsync("Login",username,pass);
                ui.EndLoad();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public async void Register(string username, string pass, string mail)
        {
            try
            {
                ui.StartLoad("Waiting for server...");
                await conn.InvokeAsync("Register", username, pass,mail);
                ui.EndLoad();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public async void UpdateSettings(Settings sett,string username,string elo)
        {
            try
            {
                await conn.InvokeAsync("UpdateSettings", sett.ActionConfirm,sett.Avatar,sett.PrefColor,sett.Sound,username);
                ui.SaveUser(username, TOKEN, new SettingsTransfer(sett), elo);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public async void CreateGuest()
        {
            try 
            { 
                await conn.InvokeAsync("CreateGuest");
            }
            catch(Exception ex) 
            { 
                Debug.Log(ex.Message); 
            }
        }
        public async void LogOut()
        {
            try
            {
                await conn.InvokeAsync("Logout");
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public async void CheckReconnect(string user)//////////////////
        {
            try
            {
                await conn.InvokeAsync("CheckReconnect",user);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public async void GetLeaderboard(string username)
        {
            try
            {
                ui.StartLoad("Fetching Leaderboard...");
                await conn.InvokeAsync("GetLeaderboard",username);
                ui.EndLoad();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
    }
    
}
