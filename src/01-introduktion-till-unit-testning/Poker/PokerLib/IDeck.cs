using System.Collections.Generic;

namespace PokerLib
{
    interface IDeck
    {
        Card DrawCard();

        void Shuffle();

        void ReturnCards(IEnumerable<Card> cards);
    }
}