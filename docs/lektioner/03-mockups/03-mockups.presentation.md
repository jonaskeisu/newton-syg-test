---
presentation:
  width: 1200
  height: 600
  theme: 'serif.css'
  center: false
  slideNumber: true
---
<style type="text/css">
  .reveal h1 {
    display: inline;
    text-align: center;
    display: flex;
    flex-direction: column;
    align-items: center;
  }
  .reveal p {
    text-align: left;
  }
  .reveal ul {
    display: block;
  }
  .reveal ol {
    display: block;
  }
  .reveal section {
    resize: false;
    width: 100%;
    height: 100;
    text-align: left;
   
  }
  .reveal pre {
    zoom: 110%;
  }
  div.slides{
    # border: 1px solid black;
  }
  .reveal code {
    zoom: 90%;
  }
</style>

<!-- slide -->

# Mockups



<!-- slide -->

## Vad är en mockup?

- En *mockup* är ett simulerat objekt
- Används ofta i testsyfte

<!-- slide -->

## När behövs en mockup?

- Simulera av ett beroende som ett objekt har på omvärlden
- Ersätta komplex logik/slump med ett förutbestämt beteende som är enklare att testa
- Fokusera ett unittest på att testa rätt del av koden

<!-- slide -->

## Exempel

I följande exempel har vi ersatt typen ``Deck`` i klassen ``Dealer`` med ett gränssnitt ``IDeck``. 

```cs
// IDeck.cs

namespace PokerLib
{
    interface IDeck
    {
        Card DrawCard();

        void Shuffle();

        void ReturnCards(IEnumerable<Card> cards);
    }
}

// Dealer.cs

namespace PokerLib
{
    class Dealer 
    {
        private IDeck Deck { get; set; }

        public Dealer(IDeck deck)
        {
            Deck = deck;
        }

        public void Deal(IEnumerable<Player> players)
        {
            Deck.Shuffle();
            for (int i = 0; i < 5; ++i)
            {
                foreach (Player player in players)
                {
                    player.RecieveCard(Deck.DrawCard());
                }
            }
        }

        // ...

    }
}
```

Implementationen av ``IDeck`` som används i produktionskoden ser ut som följer:

```cs
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
```

Dock för t.ex. unittestet av ``Deal(Player[])``-metoden i ``Dealer`` är det lämpligt att skapa en mockklass för en kortlek som underlättar testningen och begränsar testet till det vi vill fokusera på (dvs utdelning av kort):

```cs
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
```

Vi kan nu skriva testet med t.ex. följande kod: 

```cs
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
```

<!-- slide -->

## Ett till exempel

Vi kan särskilja ett spelarobjekt från intelligensen som väljer ut vilka kort som spelaren vill slänga från handen genom att deklarera ett gränssnitt ``IPlayerLogic`` enligt nedan:

```cs
namespace PokerLib
{
    interface IPlayerLogic
    {
        Card[] ChooseCardsForExchange(Player player);
    }
}
```

och sedan skriva om ``Player``-klassen enligt nedan: 

```cs
namespace PokerLib
{
    class Player
    {
        private IPlayerLogic PlayerLogic { get; set; }

        public Hand Hand { get; private set; }

        public string Name { get; private set; }

        public int Wins { get; private set; }

        public Player(string name, int wins, IPlayerLogic playerLogic)
        {
            Hand = new Hand();
            Name = name;
            Wins = wins; 
            PlayerLogic = playerLogic;
        }

        public void RecieveCard(Card card)
        {
            Hand.AddCard(card);
        }

        public IEnumerable<Card> GiveBackHand()
        {
            IEnumerable<Card> cards = Hand;
            Hand = new Hand();
            return cards; 
        }

        public void ThrowCards(List<Card> graveyard)
        {
            Card[] cardsToExchange = PlayerLogic.ChooseCardsForExchange(this);
            foreach (Card card in cardsToExchange)
            {
                Hand.RemoveCard(card);
                graveyard.Add(card);
            }
        }
    }
}
```

I produktionskoden så kommer antagligen ``PlayerLogic``-objektet för spelaren använda funktioner i användargränssnittet för att låta en mänsklig spelare se sin hand och välja kort som skall slängas. Dock i t.ex. ett unittest av metoden ``ThrowCards(List<Card>)`` för klassen ``Player`` är de lämpligt att skapa en mockklass för spelarintelligens enligt nedan: 

```cs
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
```

Unittestet kan nu skrivas t.ex. enligt nedan: 

```cs
[Test, Combinatorial]
public void PlayerCanThrowCards(
    [Values(true, false)] bool exchangeFirstCard, 
    [Values(true, false)] bool exchangeSecondCard,
    [Values(true, false)] bool exchangeThirdCard,
    [Values(true, false)] bool exchangeFourthCard,
    [Values(true, false)] bool exchangeFifthCard)
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
```