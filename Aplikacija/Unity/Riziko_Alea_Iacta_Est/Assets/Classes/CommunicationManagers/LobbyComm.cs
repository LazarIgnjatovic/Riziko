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
    public class LobbyComm
    {
        LobbyUI ui;
        public string TOKEN;
        public HubConnection conn;
        public LobbyComm(LobbyUI u)
        {
            ui = u;
            TOKEN = PlayerPrefs.GetString("token");
            Init();
            
        }
        public async void Init()
        {
            conn = new HubConnectionBuilder()
               .WithUrl("http://c143e997c3e0.ngrok.io/LobbyHub", options =>
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

            conn.On<LobbyControl>("ReceiveLobby", (lobby) =>
            {
                ui.GetLobby(lobby);
            });
            conn.On<int>("NoLobbyFound", (waitFor) =>
            {
                ui.Reatempt(waitFor);
            });
            conn.On<string>("WrongCode", (mess) =>
             {
                 ui.WrongCode(mess);
             });
            conn.On<string>("HostLeft", (mess) =>
            {
                ui.HostLeft();
            });
            conn.On<string>("GameStarted", (map) =>
            {
                ui.GameStarted(map);
            });

            try
            {
                await conn.StartAsync();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }        
        }
        public async void RequestLobby(Player user,string gameType,bool isHost)
        {
            try
            {
                await conn.InvokeAsync("SendLobby", user, gameType, isHost);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public async void RequestLobbyByCode(Player user,string joinCode)
        {
            try
            {
                await conn.InvokeAsync("SearchByCode", user,joinCode);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public async void LeaveLobby(Player user)
        {
            try
            {
                await conn.InvokeAsync("LeaveLobby", user);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public async void UpdateLobby(LobbyControl l)
        {
            try
            {
                await conn.InvokeAsync("UpdateLobby", l);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        public async void StartGame(LobbyControl l)
        {
            try
            {
                await conn.InvokeAsync("StartGame", l);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
    }
}
