using System.Collections.Generic;

namespace PokerLib
{
    delegate void ChooseCardsForExachange(Player player, IEnumerable<Card> cardsToExchange);

    class Game
    {
        private List<Player> players = new List<Player>();

        public Game(string[] playerNames)
        {
            foreach (string name in playerNames)
            {
                players.Add(new Player(name, 0, new ConsolePlayer()));
            }
        }

        public void Play()
        {
            Dealer dealer = new Dealer(new Deck());

            while (true)
            {
                // ..
            }

        }
    }
}