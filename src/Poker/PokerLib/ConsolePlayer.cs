namespace PokerLib
{
    class ConsolePlayer : IPlayerLogic
    {
        public Card[] ChooseCardsForExchange(Player player)
        {
            return UI.ChooseCardsForExchange(player);
        }
    }
}