using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerLib
{
    class Deck : IDeck
    {
        private List<Card> cards = new List<Card>();
        public Deck()
        {
            foreach(Suite suite in Enum.GetValues(typeof(Suite)))
            {
                foreach(Rank rank in Enum.GetValues(typeof(Suite)))
                {
                    cards.Add(new Card(suite, rank));
                }
            }
        }

        public Card DrawCard()
        {
            Card card = cards[0];
            cards.Skip(1).ToList();
            return card;
        }

        public void Shuffle()
        {
            Random rng = new Random();
            for (int i = 0; i < cards.Count; ++i)
            {
                int j = rng.Next(cards.Count);
                Card tmp = cards[i];
                cards[i] = cards[j];
                cards[j] = tmp;
            }
        }

        public void ReturnCards(IEnumerable<Card> cards)
        {
            this.cards.AddRange(cards);
        }
    }
}