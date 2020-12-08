using System;

#nullable enable

namespace PokerLib
{
    class Card
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

        public static implicit operator Card((Suite suite, Rank rank) card)
            => new Card(card.suite, card.rank);

        public override bool Equals(Object? obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Card c = (Card)obj;
                return (Suite == c.Suite) && (Rank == c.Rank);
            }
        }

        public static bool operator ==(Card lhs, Card rhs)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Card lhs, Card rhs)
        {
            return !(lhs == rhs);
        }
    }
}