namespace TexasHoldem.Logic.Players
{
    using System.Collections.Generic;

    using TexasHoldem.Logic.Cards;

    public interface IStartHandContext
    {
        Card FirstCard { get; }
        string DealerName { get; }
        int HandNumber { get; }
        int MoneyLeft { get; }
        Card SecondCard { get; }
        int SmallBlind { get; }
        int ActionPriority { get; }
    }
}