using Server.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Classes
{
    public class Deck
    {
        public Card[] deck;
        private Random rand = new Random();
        public int topIndex;
        public Deck(string gameMap, RizikoDbContext context)
        {
            //ovo treba iz bazu
            List<Territory> territories = new List<Territory>();
            if (gameMap == "World")
            {
                Map world = context.Map.Where(x => x.MapName == "World").Include(x => x.Continents).ThenInclude(x => x.Provinces).FirstOrDefault();
                ActiveMap map = new ActiveMap(world);
                foreach (ActiveContinent continent in map.continents)
                    foreach (Territory terr in continent.territories)
                        territories.Add(terr);
            }
            else if(gameMap == "Rome")
            {
                Map rome = context.Map.Where(x => x.MapName == "Rome").Include(x => x.Continents).ThenInclude(x => x.Provinces).FirstOrDefault();
                ActiveMap map = new ActiveMap(rome);
                foreach (ActiveContinent continent in map.continents)
                    foreach (Territory terr in continent.territories)
                        territories.Add(terr);
            }
            List<string> type = new List<string> { "Tank", "Solider", "Plane" };
            deck = new Card[territories.Count()];
            topIndex = territories.Count();
            for (int i = 0; i < deck.Count(); i++)
                deck[i] = new Card(territories[i].name, type[i/15]);
        }
        public void Shuffle()
        {
            for (int i = 0; i < deck.Length; i++)
            {
                int r = rand.Next(deck.Length);
                Card tmp = deck[i];
                deck[i] = deck[r];
                deck[r] = tmp;
            }
        }
    }
}
