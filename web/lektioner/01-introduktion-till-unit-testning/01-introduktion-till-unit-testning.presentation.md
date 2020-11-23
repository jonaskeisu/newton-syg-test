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
    border: 1px solid black;
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
- Man testar den

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

Från kodens dokumentation:





