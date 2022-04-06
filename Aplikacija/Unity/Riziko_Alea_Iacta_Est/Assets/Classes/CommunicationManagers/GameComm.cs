using Assets.Classes.DataTransfer;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Classes
{
    class GameComm
    {
        GameUI ui;
        public string TOKEN;
        public HubConnection conn;

        private bool reconnecting=false;
        public GameComm(GameUI u)
        {
            ui = u;
            TOKEN = PlayerPrefs.GetString("token");
            Init();
        }
        public async void Init()
        {
            conn = new HubConnectionBuilder()
               .WithUrl("http://c143e997c3e0.ngrok.io/GameHub", options =>
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

            conn.On<UserGameState>("InitGameState", (ugs) =>
            {
                ui.InitGameState(ugs);
            });
            conn.On<UserGameState>("UpdateGameState", (ugs) =>
            {
                if (ugs.gameId == ui.gameState.gameId)
                    ui.UpdateGameState(ugs);
            });
            conn.On<string,int>("Explode", (ter,loss) =>
            {
                ui.Explode(ter, loss);
            });
            conn.On<string>("Notify", (mess) =>
            {
                ui.Notify(false, mess);
            });
            conn.On<string,string>("AttackSuccesful", (from, to) =>
            {
                ui.AttackSuccesful(from,to);
            });
            conn.On<string, string>("ReceiveAttackPending", (from, to) =>
            {
                ui.ShowArrow(from, to);
            });
            conn.On("ReceiveEndCurrentAttack",() =>
            {
                ui.ClearArrows();
            });
            conn.On("AttackUnuccesful", () =>
            {
                ui.AttackUnsuccesful();
            });
            conn.On<Player>("Win", (p) =>
            {
                ui.VictoryDialog(p);
            });
            conn.On<Player>("Lose", (p) =>
            {
                ui.LoseDialog(p);
            });
            conn.On<List<int>,List<int>>("DiceRoll", (attacker,defender) =>
            {
                Debug.Log(attacker[0]);
                Debug.Log(defender[0]);
                ui.DiceRoll(attacker,defender);
            });
            try
            {
                await conn.StartAsync();
                Join();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public async void Join()
        {
            try
            {
                await conn.InvokeAsync("SendInitUserGameState");
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public async void NextPhase()
        {
            try
            {
                await conn.InvokeAsync("NextPhase");
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public async void Draft(string terName, int tanks)
        {
            try
            {
                await conn.InvokeAsync("Draft",terName,tanks);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public async void Attack(string from,string to, int tanks)
        {
            try
            {
                await conn.InvokeAsync("Attack", from, to, tanks);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public async void Fortify(string from, string to, int tanks)
        {
            try
            {
                await conn.InvokeAsync("Fortify", from, to, tanks);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public async void Transfer(string from, string to, int tanks)
        {
            try
            {
                await conn.InvokeAsync("Transfer", from, to, tanks);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public async void WantToAttack(string from, string to)
        {
            try
            {
                await conn.InvokeAsync("AttackPending", from, to);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public async void EndCurrentAttack()
        {
            try
            {
                await conn.InvokeAsync("EndCurrentAttack");
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public async void SwapCards(List<Card> cards)
        {
            try
            {
                await conn.InvokeAsync("SwapCards",cards);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
    }
}
