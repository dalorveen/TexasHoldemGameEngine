namespace TexasHoldem.Logic.Players
{
    using System.Collections.Generic;
    using TexasHoldem.Logic.Cards;

    public class EndHandContext : IEndHandContext
    {
        public EndHandContext(Dictionary<string, ICollection<Card>> showdownCards, int moneyLeft)
        {
            this.ShowdownCards = showdownCards;
            this.MoneyLeft = moneyLeft;
        }

        public Dictionary<string, ICollection<Card>> ShowdownCards { get; private set; }

        public int MoneyLeft { get; private set; }
    }
}