namespace TexasHoldem.AI.Champion.StatsCorrection
{
    using TexasHoldem.Logic;
    using TexasHoldem.Statistics;
    using TexasHoldem.Statistics.Indicators;

    public class PFRCorrection : BaseStatsCorrection
    {
        //private readonly PFR pfr;

        public PFRCorrection(IStats playingStyle, int numberOfHandsToStartCorrection)
            : base(numberOfHandsToStartCorrection)
        {
            //this.pfr = playingStyle.PFR;
        }

        public override double CorrectionFactor(IStats currentPlayerStats, GameRoundType street)
        {
            //if (currentPlayerStats.PFR.Hands >= this.NumberOfHandsToStartCorrection)
            //{
            //    if (currentPlayerStats.PFR.Percentage > this.pfr.Percentage)
            //    {
            //        return this.Decrease(0.25);
            //    }
            //    else
            //    {
            //        return this.Increase(0.05);
            //    }
            //}

            return 1.0;
        }
    }
}
