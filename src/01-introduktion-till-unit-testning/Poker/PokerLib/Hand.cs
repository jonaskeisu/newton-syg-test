using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static PokerLib.HandType;
using static PokerLib.Rank;

namespace PokerLib
{
    /// <summary>
    /// Class of poker hand objects.
    /// </summary>
    public class Hand : IComparable<Hand>, IEnumerable<Card>
    {
        private HandType handType = HandType.Incomplete;
        private List<Card> cards = new List<Card>();

        /// <summary>
        /// Type of the hand.
        /// </summary>
        /// <value>Type of the hand.</value>
        public HandType HandType { get; private set; }

        /// <summary>
        /// Add a card to the hand.
        /// </summary>
        /// <param name="card">The card to add to the hand.</param>
        /// <exception cref="PokerLib.HandException">The hand already has five
        /// cards.</exception>
        /// <exception cref="PokerLib.HandException">The hand already contains a
        /// duplicate of the added card.</exception>
        public void AddCard(Card card)
        {
            if (cards.Count == 5)
                throw new HandException("Hand already full.");
            if (cards.Contains(card))
                throw new HandException("Duplicate card.");
            if (card.Suite == 0)
                throw new HandException("Card not initialized.");
            cards.Add(card);
            cards = cards.OrderBy(c => c.Rank).ThenBy(c => c.Suite).ToList();
            HandType = CalculateHandType();

        }

        private HandType CalculateHandType()
        {
            if (cards.Count < 5)
                return Incomplete;

            bool isStraight(IEnumerable<Card> cards) => cards.SkipLast(1).Zip(cards.Skip(1)).All(
                cs => cs.Second.Rank == cs.First.Rank + 1
            );

            bool flush = cards.GroupBy(c => c.Suite).Count() == 1;

            bool straight = isStraight(cards)
                || (cards.First().Rank == Two && cards.Last().Rank == Ace && isStraight(cards.Take(4)));

            var groupSizes = cards.GroupBy(c => c.Rank).Select(g => g.Count()).ToList();
            groupSizes.Sort();

            return (flush, straight) switch
            {
                (true, true) => StraightFlush,
                (true, false) => Flush,
                (false, true) => Straight,
                _ => (groupSizes.Count) switch
                {
                    2 => groupSizes[0] == 1 ? FourOfAKinds : FullHouse,
                    3 => groupSizes.Contains(3) ? ThreeOfAKind : TwoPairs,
                    4 => Pair,
                    _ => HighCard,
                },
            };
        }

        /// <summary>
        /// Remove a card from the hand.
        /// </summary>
        /// <param name="card">The card to remove.</param>
        /// <exception cref="PokerLib.HandException">The removed card is not in the hand.</exception>
        public void RemoveCard(Card card)
        {
            if (!cards.Contains(card))
                throw new HandException("Hand doesn't contain removed card.");
            cards.Remove(card);
        }

        /// <summary>
        /// Compare this hand to another hand.
        /// </summary>
        /// <param name="other">The other hand.</param>
        /// <returns>
        /// <list>
        ///     <item>
        ///         <term>Less than zero</term>
        ///          <description>This hand preceedes the other
        ///          hand.</description>
        ///     </item>
        ///     <item>
        ///         <term>Zero</term>
        ///         <description>Hands are equal.</description>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero</term>
        ///         <description>This hand follows the other hand.</description>
        ///     </item>
        /// </list>
        /// </returns>
        public int CompareTo(Hand other)
        {
            if (HandType == Incomplete || other.HandType == Incomplete)
                throw new HandException("Can't compare incomplete hands");

            if (HandType != other.HandType)
                return HandType.CompareTo(HandType);

            List<Rank> findRankGroups(List<Card> cards) =>
                cards
                .GroupBy(c => c.Rank)
                .OrderBy(g => g.Count())
                .ThenBy(g => g.First().Rank)
                .Select(g => g.First().Rank)
                .Reverse()
                .ToList();

            var ranks = findRankGroups(cards);
            var otherRanks = findRankGroups(other.cards);

            return ranks.Zip(otherRanks).Aggregate(0, (c, rs) => c == 0 ? rs.First.CompareTo(rs.Second) : c);
        }


        /// <summary>
        /// Get enumerator of hand cards.
        /// </summary>
        /// <returns>Enumerator of hand cards.</returns>
        public IEnumerator<Card> GetEnumerator()
        {
            return cards.GetEnumerator();
        }

        /// <summary>
        /// Get enumerator of hand cards.
        /// </summary>
        /// <returns>Enumerator of hand cards.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return cards.GetEnumerator();
        }
    }
}