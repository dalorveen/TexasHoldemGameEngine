namespace TexasHoldem.AI.Champion.StatsCorrection
{
    using TexasHoldem.Logic;
    using TexasHoldem.Statistics;
    using TexasHoldem.Statistics.Indicators;

    public class VPIPCorrection : BaseStatsCorrection
    {
        private readonly VPIP vpip;

        public VPIPCorrection(IStats playingStyle, int numberOfHandsToStartCorrection)
            : base(numberOfHandsToStartCorrection)
        {
            this.vpip = playingStyle.VPIP;
        }

        public override double CorrectionFactor(IStats currentPlayerStats, GameRoundType street)
        {
            if (currentPlayerStats.VPIP.Hands >= this.NumberOfHandsToStartCorrection)
            {
                if (currentPlayerStats.VPIP.Percentage > this.vpip.Percentage)
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
