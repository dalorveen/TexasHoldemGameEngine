namespace TexasHoldem.Logic.Players
{
    using System.Collections.Generic;

    using TexasHoldem.Logic.Cards;

    public interface IEndHandContext
    {
        Dictionary<string, ICollection<Card>> ShowdownCards { get; }

        int MoneyLeft { get; }

        GameRoundType LastGameRoundType { get; }
    }
}