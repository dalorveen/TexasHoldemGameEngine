namespace TexasHoldem.AI.Champion.StatsCorrection
{
    using TexasHoldem.Logic;
    using TexasHoldem.Statistics;
    using TexasHoldem.Statistics.Indicators;

    public class RFICorrection : BaseStatsCorrection
    {
        //private readonly PositionStorage<RFI> rfi;

        public RFICorrection(IStats playingStyle, int numberOfHandsToStartCorrection)
            : base(numberOfHandsToStartCorrection)
        {
            //this.rfi = playingStyle.RFI;
        }

        public override double CorrectionFactor(IStats currentPlayerStats, GameRoundType street)
        {
            //if (currentPlayerStats.RFI.Hands >= this.NumberOfHandsToStartCorrection)
            //{
            //    var currentPosition = currentPlayerStats.RFI.CurrentPosition;
            //
            //    if (currentPlayerStats.RFI.IndicatorByPositions[currentPosition.Value].Percentage
            //        > this.rfi.IndicatorByPositions[currentPosition.Value].Percentage)
            //    {
            //        return this.Decrease(currentPosition.Value, 0.25);
            //    }
            //    else
            //    {
            //        return this.Increase(currentPosition.Value, 0.1);
            //    }
            //}

            return 1.0;
        }
    }
}