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
    public class HandCompareTest
    {
        static Dictionary<HandType, Card[]> exampleHands = new Dictionary<HandType, string>()
            {
                { HighCard, "♣2♦3♥4♠5♣7" },
                { Pair, "♣2♦2♥4♠5♣7" },
                { TwoPairs, "♣2♦2♥4♠4♣7" },
                { ThreeOfAKind, "♣2♦2♥2♠5♣7" },
                { Straight, "♣2♦3♥4♠5♣6" }, 
                { Flush, "♣2♣3♣4♣5♣7" }, 
                { FullHouse, "♣2♦2♥4♠4♣4" },
                { FourOfAKind, "♣2♦2♥2♠2♣7" },
                { StraightFlush, "♣2♣3♣4♣5♣6" }
            }.ToDictionary(kv => kv.Key, kv => HandTest.StringToCards(kv.Value));

        [SetUp]
        public void Setup()
        {
        }

        [Test, Combinatorial]
        public void HandTypesAreCorrectlyOrdered(
            [Values(HighCard, Pair, TwoPairs, ThreeOfAKind, Straight, Flush, FullHouse, FourOfAKind, StraightFlush)] HandType handType1, 
            [Values(HighCard, Pair, TwoPairs, ThreeOfAKind, Straight, Flush, FullHouse, FourOfAKind, StraightFlush)] HandType handType2
        )
        {
            HandType[] handTypes = new HandType[] { handType1, handType2 };
            Hand[] hands = new Hand[] { new Hand(), new Hand() };

            for (int i = 0; i < 2; i++)
            {
                Card[] cards = exampleHands[handTypes[i]];
                foreach (Card card in cards)
                {
                    hands[i].AddCard(card);
                }
                Assert.AreEqual(hands[i].HandType, handTypes[i]);
            }
            
            int comp = hands[0].CompareTo(hands[1]);

            switch (handTypes[0].CompareTo(handTypes[1]))
            {
                case int c when c < 0: 
                    Assert.Less(comp, 0);
                    break;
                case int c when c > 0: 
                    Assert.Greater(comp, 0);
                    break;
                default:
                    Assert.AreEqual(comp, 0);
                    break;
            }
        }

        [Test, Sequential]
        public void CannotCompareHandWithNull(
            [Values(HighCard, Pair, TwoPairs, ThreeOfAKind, Straight, Flush, FullHouse, FourOfAKind, StraightFlush)] HandType handType)
        {
            Hand hand = new Hand();
            foreach (Card card in exampleHands[handType])
            {
                hand.AddCard(card);
            }
            Assert.AreEqual(handType, hand.HandType);
            Assert.Throws<HandException>(() => hand.CompareTo(null));
        }

        [Test, Sequential]
        public void CannotCompareHandIncompleteHand(
            [Values(HighCard, Pair, TwoPairs, ThreeOfAKind, Straight, Flush, FullHouse, FourOfAKind, StraightFlush)] HandType handType)
        {
            Hand hand = new Hand();
            Hand hand2 = new Hand();
            foreach (Card card in exampleHands[handType])
            {
                hand.AddCard(card);
            }
            foreach (Card card in exampleHands[handType].Take(4))
            {
                hand2.AddCard(card);
            }

            Assert.AreEqual(handType, hand.HandType);
            Assert.AreEqual(Incomplete, hand2.HandType);
            Assert.Throws<HandException>(() => hand.CompareTo(hand2));
            Assert.Throws<HandException>(() => hand2.CompareTo(hand));
        }

    }
}
