namespace TexasHoldem.AI.Champion.StatsCorrection
{
    using TexasHoldem.Logic;
    using TexasHoldem.Statistics;
    using TexasHoldem.Statistics.Indicators;

    public class ThreeBetCorrection : BaseStatsCorrection
    {
        private readonly ThreeBet threeBet;

        public ThreeBetCorrection(IStats playingStyle, int numberOfHandsToStartCorrection)
            : base(numberOfHandsToStartCorrection)
        {
            this.threeBet = playingStyle.ThreeBet;
        }

        public override double CorrectionFactor(IStats currentPlayerStats, GameRoundType street)
        {
            if (currentPlayerStats.VPIP.Hands >= this.NumberOfHandsToStartCorrection)
            {
                if (currentPlayerStats.ThreeBet.Percentage(Logic.GameRoundType.PreFlop)
                    > this.threeBet.Percentage(GameRoundType.PreFlop))
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
