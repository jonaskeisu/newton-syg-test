using System.Collections.Generic;
using System.Linq;

namespace PokerLib
{
    class Player
    {
        private IPlayerLogic PlayerLogic { get; set; }

        public Hand Hand { get; private set; }

        public string Name { get; private set; }

        public int Wins { get; private set; }

        public Player(string name, int wins, IPlayerLogic playerLogic)
        {
            Hand = new Hand();
            Name = name;
            Wins = wins; 
            PlayerLogic = playerLogic;
        }

        public void RecieveCard(Card card)
        {
            Hand.AddCard(card);
        }

        public IEnumerable<Card> GiveBackHand()
        {
            IEnumerable<Card> cards = Hand;
            Hand = new Hand();
            return cards; 
        }

        public void ThrowCards(List<Card> graveyard)
        {
            Card[] cardsToExchange = PlayerLogic.ChooseCardsForExchange(this);
            foreach (Card card in cardsToExchange)
            {
                Hand.RemoveCard(card);
                graveyard.Add(card);
            }
        }
    }
}