namespace TexasHoldem.AI.Champion.StatsCorrection
{
    using TexasHoldem.Logic;
    using TexasHoldem.Statistics;
    using TexasHoldem.Statistics.Indicators;

    public class ThreeBetCorrection : BaseStatsCorrection
    {
        private readonly StreetStorage<ThreeBet> threeBet;

        public ThreeBetCorrection(IStats playingStyle, int numberOfHandsToStartCorrection)
            : base(numberOfHandsToStartCorrection)
        {
            this.threeBet = playingStyle.ThreeBet;
        }

        public override double CorrectionFactor(IStats currentPlayerStats, GameRoundType street)
        {
            if (currentPlayerStats.ThreeBet.Hands >= this.NumberOfHandsToStartCorrection)
            {
                if (currentPlayerStats.ThreeBet.IndicatorByStreets[GameRoundType.PreFlop].Percentage
                    > this.threeBet.IndicatorByStreets[GameRoundType.PreFlop].Percentage)
                {
                    return this.Decrease(0.25);
                }
                else
                {
                    return this.Increase(0.05);
                }
            }

            return 1.0;
        }
    }
}
