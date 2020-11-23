using System;

namespace PokerLib
{
    public record Card
    {
        public Suite Suite { get; }
        public Rank Rank { get; }
        public Card(Suite suite, Rank rank)
        {
            if (suite < Suite.Clubs || suite > Suite.Spades)
            {
                throw new ArgumentException("Invalid card suite.");
            }
            if (rank < Rank.Two || rank > Rank.Ace)
            {
                throw new ArgumentException("Invalid card rank.");
            }
            Suite = suite;
            Rank = rank;
        }

        public static implicit operator Card((Suite suite, Rank rank) card ) 
            => new Card(card.suite, card.rank);
    }
}