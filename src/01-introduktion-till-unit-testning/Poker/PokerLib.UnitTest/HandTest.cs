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
    public class HandTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CardsCanBeAddedToHand()
        {
            Card[] cards = new Card[] { 
                (Clubs, Two), (Diamonds, Three), (Hearts, Four), 
                 (Spades, Five), (Clubs, Six)
            };
            Hand hand = new Hand();
            CollectionAssert.IsEmpty(hand);
            for (int i = 0; i < 5; ++i)
            {
                hand.AddCard(cards[i]);
                CollectionAssert.AreEqual(cards.Take(i + 1), hand, 
                    "After inserting " + (i + 1) +" cards.");
            }
        }

        
        [Test]
        public void MaximumFiveCardsInAHand()
        {
            Card[] cards = new Card[] { 
                (Clubs, Two), (Diamonds, Three), (Hearts, Four), 
                (Spades, Five), (Clubs, Six)
            };
            Hand hand = new Hand();
            for (int i = 0; i < 5; ++i)
                hand.AddCard(cards[i]);
            Assert.Throws<HandException>(
                () => hand.AddCard(new Card(Spades, Ace))
            );
        }

        
        [Test]
        public void NoDuplicatesInAHand()
        {
            Card[] cards = new Card[] { 
                (Clubs, Two), (Diamonds, Three), (Hearts, Four), 
                (Spades, Five)
            };
            Hand hand = new Hand();
            for (int i = 0; i < 4; ++i)
                hand.AddCard(cards[i]);
            for (int i = 0; i < 4; ++i)
                Assert.Throws<HandException>(
                    () => hand.AddCard(cards[i])
                );
        }

        internal static Card[] StringToCards(string text)
        {
            List<Card> cards = new List<Card>();
            int i = 0; 
            while(i < text.Length)
            {
                Suite suite = (text[i]) switch {
                    '♣' => Clubs, '♦' => Diamonds, '♥' => Hearts, '♠'=> Spades,
                    _ => throw new NotImplementedException(),
                };
                var rankString = text.Substring(i + 1);
                var rankFunc = new Dictionary<string, Func<string, Rank>>() {
                    {@"^J",  _ => Jack}, {@"^Q", _ => Queen}, {@"^K", _ => King}, 
                    {@"^A", _ => Ace}, { @"^\d+", str => (Rank)int.Parse(str) }
                };
                var func = rankFunc.Where(func => Regex.IsMatch(rankString, func.Key)).First();
                cards.Add((suite, func.Value(Regex.Match(rankString, func.Key).Value)));
                i += Regex.IsMatch(rankString, @"^\d\d") ? 3 : 2;
            }
            return cards.ToArray();
        }

        [Test, Sequential]
        public void HandCanBePair(
            [Values(Two, Seven, Jack, Ace)] Rank pairRank, 
            [Values(Clubs, Hearts, Diamonds, Clubs)] Suite suite1, 
            [Values(Diamonds, Spades, Hearts, Spades)] Suite suite2)
        {
            // Arrange
            Hand hand = new Hand();
            foreach(var card in StringToCards("♥3♣10♠Q"))
            {
                hand.AddCard(card);
            }
            // Act
            hand.AddCard(new Card(suite1, pairRank));
            hand.AddCard(new Card(suite2, pairRank));

            // Assert
            Assert.AreEqual(Pair, hand.HandType);
        }

        [Test, Sequential]
        public void HandCanBeHighCard(
            [Values("♣2♦3♥4♠5♣7", "♣5♦7♥10♠Q♣A", "♥6♠9♣10♦J♣K" )] string cards)
        {
            Hand hand = new Hand(); 
            foreach (Card card in StringToCards(cards))
            {
                hand.AddCard(card);
            }
            Assert.AreEqual(HighCard, hand.HandType);
        }

        
        internal static Suite PseudoRandomSuite(Rank rank) => (Suite)(((int)rank % 4) + 1);

        [Test, Combinatorial]
        public void HandCanBeTwoPairs(
            [Values(Four, Five, Six)] Rank firstPairRank, 
            [Values(Two, Ten, Ace)] Rank secondPairRank, 
            [Values(Three, Seven, Eight, Nine, Jack, Queen, King)] Rank extraCardRank)
        {
            Hand hand = new Hand();
            hand.AddCard((Clubs, firstPairRank));
            hand.AddCard((Hearts, firstPairRank));
            hand.AddCard((Diamonds, secondPairRank));
            hand.AddCard((Spades, secondPairRank));
            hand.AddCard((PseudoRandomSuite(extraCardRank), extraCardRank));
            Assert.AreEqual(TwoPairs, hand.HandType);
        }

        [Test, Combinatorial]
        public void HandCanBeThreeOfAKind(
            [Values(Three, Five, Jack, King)] Rank threeOfAKindRank, 
            [Values(Two, Four, Eight, Nine, Ten)] Rank firstExtraCardRank,
            [Values(Seven, Queen, Ace)] Rank secondExtraCardRank)
        {
            Hand hand = new Hand();
            hand.AddCard((Clubs, threeOfAKindRank));
            hand.AddCard((Diamonds, threeOfAKindRank));
            hand.AddCard((Spades, threeOfAKindRank));
            hand.AddCard((PseudoRandomSuite(firstExtraCardRank), firstExtraCardRank));
            hand.AddCard((PseudoRandomSuite(secondExtraCardRank), secondExtraCardRank));
            Assert.AreEqual(ThreeOfAKind, hand.HandType);
        }

        [Test, Sequential]
        public void HandCanBeStraight(
            [Range(1, 10)] int start)
        {
            Hand hand = new Hand();
            for (int i = start; i < start + 5; ++i)
            {
                Rank rank = i == 1 ? Ace : (Rank)i;
                hand.AddCard((PseudoRandomSuite(rank), rank));
            }
            Assert.AreEqual(Straight, hand.HandType);
        }

        [Test, Combinatorial]
        public void HandCanBeFlush(
            [Values(Two, Six, Ten)] Rank rank1, 
            [Values(Three, Seven)] Rank rank2,
            [Values(Eight, Queen)] Rank rank3,
            [Values(Five, Nine)] Rank rank4,
            [Values(Ace, King)] Rank rank5,
            [Values] Suite suite)
        {
            Hand hand = new Hand();
            hand.AddCard((suite, rank1));
            hand.AddCard((suite, rank2));
            hand.AddCard((suite, rank3));
            hand.AddCard((suite, rank4));
            hand.AddCard((suite, rank5));
            Assert.AreEqual(Flush, hand.HandType);
        }

        [Test, Combinatorial]
        public void HandCanBeFullHouse(
            [Values(Two, Four, Six, Eight, Ten, Queen, Ace)] Rank pairRank, 
            [Values(Three, Five, Seven, Nine, Jack, King)] Rank threeOfAKindRank)
        {
            Hand hand = new Hand();
            hand.AddCard((Clubs, pairRank));
            hand.AddCard((Diamonds, pairRank));
            hand.AddCard((Diamonds, threeOfAKindRank));
            hand.AddCard((Hearts, threeOfAKindRank));
            hand.AddCard((Spades, threeOfAKindRank));
            Assert.AreEqual(FullHouse, hand.HandType);
        }

        [Test, Combinatorial]
        public void HandCanBeFourOfAKind(
            [Values(Two, Four, Six, Eight, Ten, Queen, Ace)] Rank fourOfAKindRank,
            [Values(Three, Five, Seven, Nine, Jack, King)] Rank extraCardRank
        )
        {
            Hand hand = new Hand();
            hand.AddCard((PseudoRandomSuite(extraCardRank), extraCardRank));
            hand.AddCard((Clubs, fourOfAKindRank));
            hand.AddCard((Diamonds, fourOfAKindRank));
            hand.AddCard((Hearts, fourOfAKindRank));
            hand.AddCard((Spades, fourOfAKindRank));
            Assert.AreEqual(FourOfAKind, hand.HandType);
        }

        [Test, Combinatorial]
        public void HandCanBeStraightFlush(
            [Values(Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten)] Rank startRank,
            [Values] Suite suite)
        {
            int i = (int)startRank; 
            Hand hand = new Hand();
            hand.AddCard((suite, (Rank)i));
            i = i == 14 ? 2 : i + 1;
            hand.AddCard((suite, (Rank)(i++)));
            hand.AddCard((suite, (Rank)(i++)));
            hand.AddCard((suite, (Rank)(i++)));
            hand.AddCard((suite, (Rank)(i++)));
            Assert.AreEqual(StraightFlush, hand.HandType);
        }
    }
}