namespace TexasHoldem.AI.Champion.StatsCorrection
{
    using TexasHoldem.Logic;
    using TexasHoldem.Statistics;

    public abstract class BaseStatsCorrection
    {
        private readonly int numberOfHandsToStartCorrection;

        private double[] coefficient;

        private double step;

        public BaseStatsCorrection(int numberOfHandsToStartCorrection)
        {
            this.numberOfHandsToStartCorrection = numberOfHandsToStartCorrection;
            this.coefficient = new double[] { 1.0, 1.0, 1.0, 1.0 };
            this.step = 0.1;
        }

        public int NumberOfHandsToStartCorrection
        {
            get
            {
                return this.numberOfHandsToStartCorrection;
            }
        }

        public abstract double CorrectionFactor(IStats currentPlayerStats, GameRoundType street);

        protected double Decrease(GameRoundType street)
        {
            if (this.coefficient[(int)street] > 1.0)
            {
                // reset
                this.coefficient[(int)street] = 1.0;
            }

            this.coefficient[(int)street] -= this.step;

            if (this.coefficient[(int)street] < 0)
            {
                this.coefficient[(int)street] = 0;
            }

            return this.coefficient[(int)street];
        }

        protected double Increase(GameRoundType street)
        {
            if (this.coefficient[(int)street] < 1.0)
            {
                // reset
                this.coefficient[(int)street] = 1.0;
            }

            this.coefficient[(int)street] += this.step;

            return this.coefficient[(int)street];
        }
    }
}
