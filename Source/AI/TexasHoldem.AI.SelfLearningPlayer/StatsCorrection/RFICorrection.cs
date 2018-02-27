namespace TexasHoldem.AI.Champion.StatsCorrection
{
    using System;
    using System.Collections.Generic;

    using TexasHoldem.Logic;
    using TexasHoldem.Statistics;
    using TexasHoldem.Statistics.Indicators;

    public class RFICorrection : BaseStatsCorrection
    {
        private readonly PositionStorage<RFI> rfi;

        private readonly Dictionary<SeatNames, double> coefficients;

        public RFICorrection(IStats playingStyle, int numberOfHandsToStartCorrection)
            : base(numberOfHandsToStartCorrection)
        {
            this.rfi = playingStyle.RFI;
            this.coefficients = new Dictionary<SeatNames, double>();

            for (int i = 0; i < Enum.GetNames(typeof(SeatNames)).Length; i++)
            {
                this.coefficients.Add((SeatNames)i, 1.0);
            }
        }

        public override double CorrectionFactor(IStats currentPlayerStats, GameRoundType street)
        {
            if (currentPlayerStats.RFI.Hands >= this.NumberOfHandsToStartCorrection)
            {
                var currentPosition = currentPlayerStats.RFI.CurrentPosition;

                if (currentPlayerStats.RFI.IndicatorByPositions[currentPosition.Value].Percentage
                    > this.rfi.IndicatorByPositions[currentPosition.Value].Percentage)
                {
                    return this.Decrease(currentPosition.Value);
                }
                else
                {
                    return this.Increase(currentPosition.Value);
                }
            }

            return 1.0;
        }

        private double Decrease(SeatNames position)
        {
            if (this.coefficients[position] > 1.0)
            {
                // reset
                this.coefficients[position] = 1.0;
            }

            this.coefficients[position] -= 0.3;

            if (this.coefficients[position] < 0)
            {
                this.coefficients[position] = 0;
            }

            return this.coefficients[position];
        }

        private double Increase(SeatNames position)
        {
            if (this.coefficients[position] < 1.0)
            {
                // reset
                this.coefficients[position] = 1.0;
            }

            this.coefficients[position] += 0.1;

            return this.coefficients[position];
        }
    }
}