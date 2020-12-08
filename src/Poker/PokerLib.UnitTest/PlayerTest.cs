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
    public class PlayerTest
    {
        class MockPlayerLogic : IPlayerLogic
        {
            private List<int> indicesOfCardsForExchange = new List<int>();

            public MockPlayerLogic(IEnumerable<int> indices)
            {
                indicesOfCardsForExchange = indices.ToList();
            }

            public Card[] ChooseCardsForExchange(Player player) 
                => indicesOfCardsForExchange.Select(i => player.Hand.Skip(i).First()).ToArray();
        }

        [Test, Combinatorial]
        public void PlayerCanThrowCards(
            [Values(true, false)] bool exchangeFirstCard, 
            [Values(true, false)] bool exchangeSecondCard,
            [Values(true, false)] bool exchangeThirdCard,
            [Values(true, false)] bool exchangeFourthCard,
            [Values(true, false)] bool exchangeFifthCard
        )
        {
            // Arrange
            bool[] exchangeCard  = new bool[] { 
                exchangeFirstCard,
                exchangeSecondCard,
                exchangeThirdCard, 
                exchangeFourthCard, 
                exchangeFifthCard
            };

            List<int> indicesOfCardsForExchange = 
                Enumerable.Range(0,5).Zip(exchangeCard).Where(pair =>
                    pair.Second).Select(pair => pair.First).ToList();

            MockPlayerLogic playerLogic = new MockPlayerLogic(indicesOfCardsForExchange);
            Player player = new Player("Jonas", 0, playerLogic);
            Card[] initialCards = HandTest.StringToCards("♣2♦3♥4♠5♣7");
            foreach (Card card in initialCards)
            {
                player.RecieveCard(card);
            }
            List<Card> graveyard = new List<Card>();

            // Act
            player.ThrowCards(graveyard);
            
            // Assert
            int noOfExchangedCards = indicesOfCardsForExchange.Count;

            Assert.AreEqual(5 - noOfExchangedCards, player.Hand.Count());

            Card[] replacementCards = 
                HandTest.StringToCards("♣5♦7♥10♠Q♣A")
                .Take(noOfExchangedCards)
                .ToArray();

            foreach (Card card in replacementCards)
            {
                player.RecieveCard(card);
            }

            Card[] playerShouldNowHowTheseCards = 
                initialCards
                .Zip(exchangeCard)
                .Where(pair => !pair.Second)
                .Select(pair => pair.First)
                .Concat(replacementCards)
                .OrderBy(c => c.Rank)
                .ThenBy(c => c.Suite)
                .ToArray();

            CollectionAssert.AreEqual(playerShouldNowHowTheseCards, player.Hand);
        }

        [Test]
        public void PlayerCanGiveBackHand()
        {
            // Arrange
            Player player = new Player("Jonas", 0, new ConsolePlayer());

            Card[] initialCards = HandTest.StringToCards("♣2♦3♥4♠5♣7");
            foreach (Card card in initialCards)
            {
                player.RecieveCard(card);
            }

            // Act

            Card[] cardsGivenBack = player.GiveBackHand().ToArray();

            // Assert

            Assert.AreEqual(0, player.Hand.Count());

            CollectionAssert.AreEqual(initialCards, cardsGivenBack);
        }
    }
}