using System.Collections.Generic;
using System.Linq;

namespace PokerLib
{
    class Dealer 
    {
        private IDeck Deck { get; set; }

        public Dealer(IDeck deck)
        {
            Deck = deck;
        }

        public void Deal(IEnumerable<Player> players)
        {
            Deck.Shuffle();
            for (int i = 0; i < 5; ++i)
            {
                foreach (Player player in players)
                {
                    player.RecieveCard(Deck.DrawCard());
                }
            }
        }

        public void ReplaceCards(Player player)
        {
            while (player.Hand.Count() < 5)
            {
                player.RecieveCard(Deck.DrawCard());
            }
        }

        public void CollectAllCards(List<Card> graveyard, IEnumerable<Player> players)
        {
            Deck.ReturnCards(graveyard);
            graveyard.Clear();
            foreach (Player player in players)
            {
                Deck.ReturnCards(player.GiveBackHand());
            }
        }

        public IEnumerable<Player> SelectPlayersWithBestHand(IEnumerable<Player> players)
        {
            var sortedPlayers = players.OrderByDescending(p => p.Hand);
            return players.TakeWhile(p => p.Hand.CompareTo(players.First().Hand) == 0);
        }
    }
}