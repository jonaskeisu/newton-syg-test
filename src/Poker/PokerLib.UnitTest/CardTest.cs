using System.Collections.Generic;
using NUnit.Framework;
using PokerLib;
using static PokerLib.Suite;
using static PokerLib.Rank;
using static PokerLib.HandType;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace PokerLib.UnitTest
{
    [TestFixture]
    public class CardTest
    {

        [Test, Combinatorial]
        public void CanConstructCard(
            [Values(Clubs, Diamonds, Hearts, Spades)] Suite suite, 
            [Values(
                Two, Three, Four, Five, Six, Seven, Eight, 
                Nine, Ten, Jack, Queen, King, Ace)] Rank rank)
        {
            Card card = new Card(suite, rank);
            Assert.AreEqual(suite, card.Suite);
            Assert.AreEqual(rank, card.Rank);
        }

        [Test, Sequential]
        public void CardCannotHaveInvalidSuite([Values(0, 5)] int x)
        {
            Assert.Throws<ArgumentException>( () => new Card((Suite)x, Two));
        }

        [Test, Sequential]
        public void CardCannotHaveInvalidRank([Values(1, 15)] int x)
        {
            Assert.Throws<ArgumentException>( () => new Card(Clubs, (Rank)x));
        }
    }
}