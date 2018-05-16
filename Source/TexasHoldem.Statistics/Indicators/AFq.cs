namespace TexasHoldem.Statistics.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class AFq : BaseIndicator<AFq>
    {
        private Dictionary<GameRoundType, int> totalTimesRaisedByStreet;
        private Dictionary<GameRoundType, int> totalTimesCalledByStreet;
        private Dictionary<GameRoundType, int> totalTimesFoldedByStreet;

        public AFq()
            : base(0)
        {
            this.totalTimesRaisedByStreet = new Dictionary<GameRoundType, int>();
            this.totalTimesCalledByStreet = new Dictionary<GameRoundType, int>();
            this.totalTimesFoldedByStreet = new Dictionary<GameRoundType, int>();

            foreach (var item in Enum.GetValues(typeof(GameRoundType)).Cast<GameRoundType>())
            {
                this.totalTimesRaisedByStreet.Add(item, 0);
                this.totalTimesCalledByStreet.Add(item, 0);
                this.totalTimesFoldedByStreet.Add(item, 0);
            }
        }

        public int TotalTimesRaised
        {
            get
            {
                return this.totalTimesRaisedByStreet.Sum(s => s.Value);
            }
        }

        public int TotalTimesCalled
        {
            get
            {
                return this.totalTimesCalledByStreet.Sum(s => s.Value);
            }
        }

        public int TotalTimesFolded
        {
            get
            {
                return this.totalTimesFoldedByStreet.Sum(s => s.Value);
            }
        }

        /// <summary>
        /// Gets the measure of how frequently a player is aggressive
        /// </summary>
        /// <value>Percentages of aggression frequency</value>
        public override double Amount
        {
            get
            {
                return this.TotalTimesRaised == 0
                    ? 0
                    : ((double)this.TotalTimesRaised /
                        (double)(this.TotalTimesRaised + this.TotalTimesCalled + this.TotalTimesFolded)) * 100.0;
            }
        }

        public double AmountByStreet(GameRoundType street)
        {
            return this.totalTimesRaisedByStreet[street] == 0
                    ? 0
                    : ((double)this.totalTimesRaisedByStreet[street] /
                        (double)(this.totalTimesRaisedByStreet[street]
                            + this.totalTimesCalledByStreet[street]
                            + this.totalTimesFoldedByStreet[street])) * 100.0;
        }

        public override void Update(IGetTurnContext context, PlayerAction madeAction, IStatsContext statsContext)
        {
            if (madeAction.Type == PlayerActionType.Raise)
            {
                this.totalTimesRaisedByStreet[context.RoundType]++;
            }
            else if (madeAction.Type == PlayerActionType.CheckCall)
            {
                this.totalTimesCalledByStreet[context.RoundType]++;
            }
            else if (madeAction.Type == PlayerActionType.Fold)
            {
                this.totalTimesFoldedByStreet[context.RoundType]++;
            }
        }

        public override string ToString()
        {
            return this.ToStreetFormat(
                this.AmountByStreet(GameRoundType.PreFlop),
                this.AmountByStreet(GameRoundType.Flop),
                this.AmountByStreet(GameRoundType.Turn),
                this.AmountByStreet(GameRoundType.River));
        }
    }
}
