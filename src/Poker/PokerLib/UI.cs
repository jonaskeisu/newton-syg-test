using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerLib
{
    static class UI
    {
        public static Card[] ChooseCardsForExchange(Player player)
        {
            Console.WriteLine(player.Name);
            // PrintHandToConsole(player.Hand);
            string line = Console.ReadLine();
            int[] indexOfCardsForExchange = new int[0]; 
            // .. tolka line för att få ut index på korten som skall bytas
            List<Card> cardsForExchange = new List<Card>();
            foreach (int i in indexOfCardsForExchange)
            {
                cardsForExchange.Add(player.Hand.Skip(i).First());
            }
            return cardsForExchange.ToArray();
        }
    }
    
}