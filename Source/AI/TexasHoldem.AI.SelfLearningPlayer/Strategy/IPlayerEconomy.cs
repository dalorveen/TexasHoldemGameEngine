namespace TexasHoldem.AI.SelfLearningPlayer.Strategy
{
    using System.Collections.Generic;

    using TexasHoldem.AI.SelfLearningPlayer.PokerMath;

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
