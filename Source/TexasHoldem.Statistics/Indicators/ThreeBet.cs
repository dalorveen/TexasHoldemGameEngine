namespace TexasHoldem.Statistics.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class ThreeBet : BaseIndicator<ThreeBet>
    {
        private Dictionary<GameRoundType, int> totalTimes3BetByStreet;
        private Dictionary<GameRoundType, int> total3BetOpportunitiesByStreet;

        public ThreeBet()
            : base(0)
        {
            this.totalTimes3BetByStreet = new Dictionary<GameRoundType, int>();
            this.total3BetOpportunitiesByStreet = new Dictionary<GameRoundType, int>();

            foreach (var item in Enum.GetValues(typeof(GameRoundType)).Cast<GameRoundType>())
            {
                this.totalTimes3BetByStreet.Add(item, 0);
                this.total3BetOpportunitiesByStreet.Add(item, 0);
            }
        }

        public bool IsOpportunity { get; private set; }

        public int TotalTimes3Bet
        {
            get
            {
                return this.totalTimes3BetByStreet.Sum(s => s.Value);
            }
        }

        public int Total3BetOpportunities
        {
            get
            {
                return this.total3BetOpportunitiesByStreet.Sum(s => s.Value);
            }
        }

        /// <summary>
        /// Gets the percentage of time a player re-raised a raiser
        /// </summary>
        /// <value>Percentages of 3Bet</value>
        public override double Amount
        {
            get
            {
                return this.Total3BetOpportunities == 0
                    ? 0 : ((double)this.TotalTimes3Bet / (double)this.Total3BetOpportunities) * 100.0;
            }
        }

        public double AmountByStreet(GameRoundType street)
        {
            return this.total3BetOpportunitiesByStreet[street] == 0
                    ? 0
                    : ((double)this.totalTimes3BetByStreet[street]
                        / (double)this.total3BetOpportunitiesByStreet[street]) * 100.0;
        }

        public override void Update(IGetTurnContext context, IStatsContext statsContext)
        {
            var raises = context.PreviousRoundActions.Count(x => x.Round == context.RoundType
                && x.Action.Type == PlayerActionType.Raise);

            if (raises == (context.RoundType == GameRoundType.PreFlop ? 1 : 2))
            {
                this.total3BetOpportunitiesByStreet[context.RoundType]++;
                this.IsOpportunity = true;
            }
        }

        public override void Update(IGetTurnContext context, PlayerAction madeAction, IStatsContext statsContext)
        {
            if (this.IsOpportunity && madeAction.Type == PlayerActionType.Raise)
            {
                this.totalTimes3BetByStreet[context.RoundType]++;
            }

            this.IsOpportunity = false;
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
