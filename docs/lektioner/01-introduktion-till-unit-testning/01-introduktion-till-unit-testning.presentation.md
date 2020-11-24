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

# Introduktion till Unit-tester

<!-- slide -->

## Hur *vet* man att kod fungerar?

- Man bevisar att den fungerar (matematiskt), eller..
- Man *testar* koden. 
- I industrin är den andra metoden betydligt vanligare.

<!-- slide -->

## Varför testa?

- Värdet i mjukvara ligger i leverans av korrekt mjukvara som uppfyller användarens krav.
- Det är kostsamt och dåligt för produktens rykte att användarna hittar fel i mjukvaran.
- Det är därför viktigt att säkerställa så gott det går att mjukvaran fungerar *innan* leverans. 

<!-- slide -->

## Manuell testning

- Ett sätt att testa mjukvara är manuell testning. 
- Mjukvara har som regel väldigt många användningsscenarion och förändras ofta.
  - Detta gör manuell testning väldigt dyrt.
- Manuell testning är också långsamt, vilket försenar leverans av mjukvaran.

<!-- slide -->

## Autmatisk testning

- Ett annat sätt att testa mjukvara är *automatiskt* testning.
- Automatiskt testning är kod vars syfte är att testar att koden för användarens mjukvara uppfyller alla krav.
- Automatiska tester: 
  - Minskar kostnaden för projektet. 
  - Ökar kvaliteten på produkten. 
  - Tillåter produkten att släppas snabbare och oftare.
- Det är systemutvecklaren ansvar att producera den automatiska testningen.


<!-- slide -->

### Exempel

Antag följande ``Hand``-klass i ett pokerbibliotek:

<center style="zoom:0.9">

```plantuml
left to right direction

class Hand {
    + HandType HandType
    + IEnumerable<Card> Cards
    + AddCard(Card card)
    + RemoveCard(Card card)
    + int CompareTo(Hand)
    + IEnumerator<Card> GetEnumerator()
    + IEnumerator GetEnumerator()
}

class "IComparable<Hand>"  << (I, Yellow) >> {
}

class "IEnumerable<Card>"  << (I, Yellow) >> {
    
}

class Exception {
}

class HandException {
}

Exception <|-- HandException

"IComparable<Hand>" <|.. Hand
"IEnumerable<Card>" <|.. Hand

HandType *-- Hand
Hand --o "0..5" Card
HandException <-- Hand

class Card << (S, AliceBlue) >> {
    + Suite Suite
    + Rank Rank
}

Card --* Suite
Card --* Rank

enum Rank
{
    Two = 2
    Three
    Four
    Five
    Six 
    Seven
    Eight
    Nine
    Ten
    Jack
    Queen
    King
    Ace
}
enum Suite
{
    Clubs = 0
    Diamon
    Hearts
    Spades
}


enum HandType
{
    Incomplete = -1
    HighCard
    Pair
    TwoPairs
    ThreeOfAKind
    Straight
    Flush
    FullHouse
    FourOfAKind
    StraightFlush
}
```

</center>

<!-- slide -->

Vi tittar på förväntat betende för klassen ``Hand`` i kodens dokumentation. 

<!-- slide -->

För att testa att klassen fungerar, bör vi testa att klassens egenskaper och metoder fungerar som avsett för en god spridning av testfall.

<!-- slide -->

## Hur skapar man tester?

- Testerna skrivs med hjälp av kod.
- Testkod skall inte blandas med kod för användaren.
  - Kod för användaren kallas *produktionskod*.
- Testkod läggs i ett separat projekt.
- För att underlätta skrivandet av testkod används speciella ramverk för testkod.

<!-- slide -->

## NUnit

- Det finns ett beprövat ramverk för att skriva testkod för .NET som heter *NUnit* 
- Det finns också ett nyare ramverk som heter xUnit.net som vinner populäritet. 
- Vi kommer använda NUnit för att det har längre användning i industrin och är betydligt bättre dokumenterat än xUnit.

<!-- slide -->

## Skapa ett projekt med NUnit

Antag att en solution med ett klassbiblioteksprojekts har skapats lokalt på datorn med kommandona:

```console
$ pwd
/Users/jonaskeisu/src
$ cd Poker
$ dotnet new sln
$ dotnet new classlib -n Poker.Lib
$ dotnet sln add Poker.Lib/Poker.Lib.csproj
```

<!-- slide -->

Använd då följande kommando i ``Poker``-katalogen för att lägga till ett testprojekt och lägga till en referens i testprojektet till produktionskoden:

```console
$ pwd
/Users/jonaskeisu/src/Poker
$ dotnet new nunit -n Poker.Lib.UnitTest
$ dotnet sln add Poker.Lib.UnitTest/Poker.Lib.UnitTest.csproj
$ cd Poker.Lib.UnitTest
$ dotnet add reference ../Poker.Lib/Poker.Lib.csproj
```

<!-- slide -->

Projektet skapat från mallen innehåller en källkodsfil med namn ``UnitTest1.cs`` som innehåller följande kod: 

```cs
using NUnit.Framework;
namespace PokerLibTest {
    public class Tests {
        [SetUp]
        public void Setup(){
        }
        [Test]
        public void Test1() {
            Assert.Pass();
        }
    }
}
```

<!-- slide -->

## Strukturen på testkod

- Koden generar av mallen innehåller en klass.
- Klassen är en så kallad *testfixtur*. 
  - En textfixtur är en klass som innehåller tester.
- Klassen innehåller två metoder: 
  - En med attributet ``[SetUp]``
  - En med attributet ``[Test]``
- En metod med attributet ``[SetUp]`` körs en gång innan varje test.
- En metoder med attributet ``[Test]`` motsvarar ett test.
  
<!-- slide -->

## Vad skall en testklass innehålla?

- En testklass innehåller på tester som hör ihop, t.ex. tester för en viss klass av objekt. 

<!-- slide -->

## Tester innehåller påståenden

- Koden i en testmetod innehåller påståenden (eng. *assertion*) om resultatet av körning av kod.
- Påståenden görs genom anrop till olika statiska metoder i t.ex. klasserna
  - ``NUnit.Framework.Assert``
  - ``NUnit.Framework.CollectionAssert`` 
  - ``NUnit.Framework.StringAssert``

<!-- slide -->

## Vilka sorters påståenden kan man göra?

- Det finns många olika metoder för olika sorters påståenden: 
  - Ett villkor är sant eller falsk. 
  - Ett påstånde om ett värde (t.ex. att det skall vara större än ett annat värde)
  - Inget exception skall kastas / ett visst exception skall kastas
  - Påståenden om sammlingar
- För en komplett lista av möjliga påståenden, se denna [länk](https://docs.nunit.org/articles/nunit/writing-tests/assertions/assertion-models/classic.html).
  
<!-- slide -->

## Varför så många olika sorters påståenden?

- Genom att använda specifika metoder blir felrapporten om varför ett test misslyckas mer informativ. 

<!-- slide -->

### Exempel

Vi lägger till ett test i vår testfixtur:

```cs
[Test]
public void CardsCanBeAddedToHand() {
    Card[] cards = new Card[] { 
        (Clubs, Two), (Diamonds, Three), (Hearts, Four), 
        (Spades, Five), (Clubs, Six) };
    Hand hand = new Hand();
    CollectionAssert.IsEmpty(hand);
    for (int i = 0; i < 5; ++i) {
        hand.AddCard(cards[i]);
        CollectionAssert.AreEqual(cards.Take(i + 1), hand);
    }
}
```

<!-- slide -->

## Körning av test

Alla testar i vår solution kan köras enligt nedan:

```console
$ pwd
/Users/jonaskeisu/src/Poker
$ dotnet test
```

<!-- slide -->

Utskriften från körningen av testen blir: 

```console
Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed: 0, Passed: 2, Skipped: 0, Total: 2, Duration: 36 ms
```

Alla tester gick igenom!

<!-- slide -->

## Vad händer om ett påstande är falskt?

Vi testar genom att ändra raden: 

```cs
CollectionAssert.IsEmpty(hand);
```

till: 

```cs
CollectionAssert.IsNotEmpty(hand);
```

<!-- slide -->

Utskriften från körningen av testen blir då: 

```console
Starting test execution, please wait...
A total of 1 test files matched the specified pattern.
  Failed CardsCanBeAddedToHand [26 ms]
  Error Message:
     Expected: not <empty>
  But was:  <empty>

  Stack Trace:
     at PokerLib.UnitTest.Tests.CardsCanBeAddedToHand() in 
     /Users/jonaskeisu/[...]/UnitTest1.cs:line 32


Failed!  - Failed: 1, Passed: 1, Skipped: 0, Total: 2, Duration: 26 ms
```

<!-- slide -->

## Testa även felaktiga försättningar

- Det räcker inte att bara testa att koden fungerar under rimliga förutsättningar.
- Hantering av alla typer av felaktiga förutsättningar ingår också som regel i kraven för koden.

<!-- slide -->

### Exempel

Vi behöver lägga till ett test som kontrollerar att vi inte tillåts lägga till för många kort i en hand. 

<!-- slide -->

```cs
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
```

<!-- slide -->

Vi behöver också lägga till ett test som kontrollerar att vi inte kan lägga till dubletter av något kort till händen. 

<!-- slide -->

```cs
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
```

<!-- slide -->

## Vad skall ett testmetod innehålla? 

- Ett bra unittestmetod testar endast *en* förväntad egenskap för *en minimal beståndsdel* av koden genom ett testscenario tillsammans med olika påstående. 
- På så sätt upptäcks felet så nära källan som möjligt, vilket gör felet lättare att hitta. 

<!-- slide -->

## Vad skall man testa? 

- Det ultimata målet med unittestning är att testa *alla egenskaper* som kodens minsta beståndsdelar skall ha enligt kravspecifikationen för *alla tänkbara scenarion*. 
- .. men det är som regel omöjligt att testa alla scenarion. 
- Ett mer relalistiskt mål är att testa alla scenarion som utgör extremfall och från kodens perspektiv signifikant olika förutsättningar. 
- Att skriva tester är lätt, att välja rätt tester kräver god förståelse för både problemet som koden skall lösa och programmering. 

<!-- slide -->

### Exempel

- En nödvändig egenskap för en pokerhand har är att den skall ha korrekt handtyp i alla lägen.
- Vi behöver därför skriva en testmetod för varje möjlig handtyp som kontrollerar att den specifika handtypen identifieras korrekt under olika förutsättningar.
- Låt oss som exempel börja med identifiering av *par*. 

<!-- slide -->

Vad är olika förutsättningar för ett par i handen? 
- Paret kan bestå av olika ranker och färger. 
- Paret kan ligga på olika positioner i den sorterade handen.

Extremfall: 
- Paret ligger först i handen.
- Paret ligger sist i handen.
- Par i tvåor och par i äss.
- Par i klöver och par i spader.

<!-- slide -->

Hade varit trevligt om vi kunde ange kort med strängar, t.ex.

```cs
"♣2♦2♥3♥4♥5"
```

Låt oss implementera en funktion som översätter en sådan sträng till ett objekt av typen ``Card[]``

<!-- slide -->

<div style="zoom: 0.8">

```cs
static Card[] ToCards(string text) {
    List<Card> cards = new List<Card>();
    int i = 0; 
    while(i < text.Length) {
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
```

<!-- slide -->

Med metoden ``Card[] ToCards(string)`` kan vi skriva testet:
```cs 
[Test]
public void HandCanBePair()
{
    var cardSets = new string[] {
        "♣2♦2♥3♥4♥5", "♠3♥7♠7♠8♠9", "♣9♣10♦J♥J♣Q", "♦J♦Q♦K♣A♠A" 
    };
    foreach (var cardSet in cardSets.Select(cs => ToCards(cs))) {
        Hand hand = new Hand();
        foreach (var card in cardSet)
            hand.AddCard(card);
        Assert.AreEqual(Pair, hand.HandType);
    }   
}
```




















