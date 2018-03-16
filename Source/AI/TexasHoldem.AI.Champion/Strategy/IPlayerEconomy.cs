namespace TexasHoldem.AI.Champion.Strategy
{
    using System.Collections.Generic;

    using TexasHoldem.AI.Champion.PokerMath;

    public interface IPlayerEconomy
    {
        HandStrength Hero { get; }

        IReadOnlyList<HandStrength> Opponents { get; }

        bool NutHand { get; }

        bool BestHand { get; }

        int TiedHandsWithHero { get; }

        double OptimalInvestment(int pot);
    }
}
