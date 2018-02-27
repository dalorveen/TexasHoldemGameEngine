namespace TexasHoldem.AI.Champion.StatsCorrection
{
    using TexasHoldem.Logic;
    using TexasHoldem.Statistics;
    using TexasHoldem.Statistics.Indicators;

    public class FourBetCorrection : BaseStatsCorrection
    {
        private readonly StreetStorage<FourBet> fourBet;

        public FourBetCorrection(IStats playingStyle, int numberOfHandsToStartCorrection)
            : base(numberOfHandsToStartCorrection)
        {
            this.fourBet = playingStyle.FourBet;
        }

        public override double CorrectionFactor(IStats currentPlayerStats, GameRoundType street)
        {
            if (currentPlayerStats.VPIP.Hands >= this.NumberOfHandsToStartCorrection)
            {
                if (currentPlayerStats.FourBet.IndicatorByStreets[GameRoundType.PreFlop].Percentage
                    > this.fourBet.IndicatorByStreets[GameRoundType.PreFlop].Percentage)
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