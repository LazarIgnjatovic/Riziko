using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Server.Models;

namespace Server.Classes
{
    public class GameMaster
    {
        public List<GameControl> activeGames = new List<GameControl>();
        public RizikoDbContext dbContext;
        public GameMaster(RizikoDbContext ctx)
        {
            dbContext = ctx;
        }
        internal void DestroyMe(GameControl gameControl)
        {
            gameControl.StopTimer();
            Console.WriteLine(gameControl.gameId + " destroyed");
            activeGames.Remove(gameControl);
            gameControl.Dispose();
        }

        internal void GiveMeContext(GameControl ctrl)
        {
            ctrl.UpdateDbContext(dbContext);
        }
    }
}
