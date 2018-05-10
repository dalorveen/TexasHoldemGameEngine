namespace TexasHoldem.AI.Champion.PokerMath
{
    using System.Collections.Generic;

    public interface ICalculator
    {
        ICollection<HandStrength> Equity();

        ICollection<HandStrength> OnlyCurrentRound();
    }
}
