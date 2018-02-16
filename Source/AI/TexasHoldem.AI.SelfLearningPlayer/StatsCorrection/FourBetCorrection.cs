namespace TexasHoldem.AI.Champion.StatsCorrection
{
    using TexasHoldem.Logic;
    using TexasHoldem.Statistics;
    using TexasHoldem.Statistics.Indicators;

    public class FourBetCorrection : BaseStatsCorrection
    {
        private readonly FourBet fourBet;

        public FourBetCorrection(IStats playingStyle, int numberOfHandsToStartCorrection)
            : base(numberOfHandsToStartCorrection)
        {
            this.fourBet = playingStyle.FourBet;
        }

        public override double CorrectionFactor(IStats currentPlayerStats, GameRoundType street)
        {
            if (currentPlayerStats.VPIP.Hands >= this.NumberOfHandsToStartCorrection)
            {
                if (currentPlayerStats.FourBet.Percentage(Logic.GameRoundType.PreFlop)
                    > this.fourBet.Percentage(GameRoundType.PreFlop))
                {
                    return this.Decrease(street);
                }
                else
                {
                    return this.Increase(street);
                }
            }

            return 1.0;
        }
    }
}