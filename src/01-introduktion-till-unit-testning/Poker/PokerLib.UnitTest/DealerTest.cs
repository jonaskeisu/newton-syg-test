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
    public class DealerTest
    {
        class MockDeck : IDeck
        {
            private List<Card> cards = new List<Card>();

            public bool DealerShuffledTheDeckBeforeDrawingCards { get; private set; }

            private bool DealerHasDrawnCardsFromDeck; 

            public MockDeck(int numberOfPlayers, 
                params string[] hands)
            {
                Card[][] handCards = hands.Select(h => HandTest.StringToCards(h)).ToArray();
                for (int i = 0; i < 5; ++i)
                {
                    for (int j = 0; j < numberOfPlayers; ++j)
                    {
                        cards.Add(handCards[j][i]);
                    }
                }

            }

            public Card DrawCard()
            {
                DealerHasDrawnCardsFromDeck = true;
                Card cardToReturn = cards[0];
                cards = cards.Skip(1).ToList();
                return cardToReturn;
            }

            public void ReturnCards(IEnumerable<Card> cards)
            {
            }

            public void Shuffle()
            {
                if (!DealerHasDrawnCardsFromDeck)
                {
                    DealerShuffledTheDeckBeforeDrawingCards = true;
                }
            }
        }

        [Test, Sequential]
        public void DealerCanDeal([Values(0, 1, 2)] int playerIndex)
        {
            // Arrange
            string[] hands = new string[]
                {
                    "♣2♦3♥4♠5♣7", "♣5♦7♥10♠Q♣A", "♥6♠9♣10♦J♣K"
                };

            MockDeck mockDeck = new MockDeck(3, hands);

            Dealer dealer = new Dealer(mockDeck);

            Player[] players = new Player[]
            {
                new Player("Jonas", 0, new ConsolePlayer()),
                new Player("Karl", 0, new ConsolePlayer()),
                new Player("Anna", 0, new ConsolePlayer())
            };

            // Act
            dealer.Deal(players);

            // Assert 
            Assert.AreEqual(players[playerIndex].Hand.Count(), 5);

            Assert.IsTrue(mockDeck.DealerShuffledTheDeckBeforeDrawingCards);

            CollectionAssert.AreEqual(
                HandTest.StringToCards(hands[playerIndex]), players[playerIndex].Hand);
        }
    }
}
