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
    public class Tests
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
                CollectionAssert.AreEqual(cards.Take(i + 1), hand);
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

        static Card[] StringToCards(string text)
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

        [Test]
        public void HandCanBePair()
        {
            var cardSets = new string[] {
                "♣2♦2♥3♥4♥5", "♠3♥7♠7♠8♠9", "♣9♣10♦J♥J♣Q", "♦J♦Q♦K♣A♠A" 
            };
            foreach (var cardSet in cardSets.Select(cs => StringToCards(cs)))
            {
                Hand hand = new Hand();
                foreach (var card in cardSet)
                    hand.AddCard(card);
                TestContext.WriteLine(String.Join(", ", hand.Select(c => c.Suite + ":" + c.Rank)));
                Assert.AreEqual(Pair, hand.HandType);
            }   
        }
    }


}