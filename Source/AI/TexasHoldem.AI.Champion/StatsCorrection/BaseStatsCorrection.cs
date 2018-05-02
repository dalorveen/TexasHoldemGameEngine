namespace TexasHoldem.AI.Champion.StatsCorrection
{
    using System;
    using System.Collections.Generic;

    using TexasHoldem.Logic;
    using TexasHoldem.Statistics;

    public abstract class BaseStatsCorrection
    {
        private readonly int numberOfHandsToStartCorrection;

        private readonly Dictionary<Positions, double> coefficientByPositions;

        private readonly Dictionary<GameRoundType, double> coefficientByStreets;

        private double coefficient;

        public BaseStatsCorrection(int numberOfHandsToStartCorrection)
        {
            this.numberOfHandsToStartCorrection = numberOfHandsToStartCorrection;
            this.coefficientByPositions = new Dictionary<Positions, double>();
            this.coefficientByStreets = new Dictionary<GameRoundType, double>();

            for (int i = 0; i < Enum.GetNames(typeof(Positions)).Length; i++)
            {
                this.coefficientByPositions.Add((Positions)i, 1.0);
            }

            for (int i = 0; i < 4; i++)
            {
                this.coefficientByStreets.Add((GameRoundType)i, 1.0);
            }
        }

        public int NumberOfHandsToStartCorrection
        {
            get
            {
                return this.numberOfHandsToStartCorrection;
            }
        }

        public abstract double CorrectionFactor(IStats currentPlayerStats, GameRoundType street);

        protected double Decrease(double step)
        {
            if (this.coefficient > 1.0)
            {
                // reset
                this.coefficient = 1.0;
            }

            this.coefficient -= step;

            if (this.coefficient < 0)
            {
                this.coefficient = 0;
            }

            return this.coefficient;
        }

        protected double Decrease(GameRoundType street, double step)
        {
            if (this.coefficientByStreets[street] > 1.0)
            {
                // reset
                this.coefficientByStreets[street] = 1.0;
            }

            this.coefficientByStreets[street] -= step;

            if (this.coefficientByStreets[street] < 0)
            {
                this.coefficientByStreets[street] = 0;
            }

            return this.coefficientByStreets[street];
        }

        protected double Decrease(Positions seatName, double step)
        {
            if (this.coefficientByPositions[seatName] > 1.0)
            {
                // reset
                this.coefficientByPositions[seatName] = 1.0;
            }

            this.coefficientByPositions[seatName] -= step;

            if (this.coefficientByPositions[seatName] < 0)
            {
                this.coefficientByPositions[seatName] = 0;
            }

            return this.coefficientByPositions[seatName];
        }

        protected double Increase(double step)
        {
            if (this.coefficient < 1.0)
            {
                // reset
                this.coefficient = 1.0;
            }

            this.coefficient += step;

            return this.coefficient;
        }

        protected double Increase(GameRoundType street, double step)
        {
            if (this.coefficientByStreets[street] < 1.0)
            {
                // reset
                this.coefficientByStreets[street] = 1.0;
            }

            this.coefficientByStreets[street] += step;

            return this.coefficientByStreets[street];
        }

        protected double Increase(Positions seatName, double step)
        {
            if (this.coefficientByPositions[seatName] < 1.0)
            {
                // reset
                this.coefficientByPositions[seatName] = 1.0;
            }

            this.coefficientByPositions[seatName] += step;

            return this.coefficientByPositions[seatName];
        }
    }
}
