namespace TexasHoldem.Statistics.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class FourBet : BaseIndicator<FourBet>
    {
        private Dictionary<GameRoundType, int> totalTimes4BetByStreet;
        private Dictionary<GameRoundType, int> total4BetOpportunitiesByStreet;

        public FourBet()
            : base(0)
        {
            this.totalTimes4BetByStreet = new Dictionary<GameRoundType, int>();
            this.total4BetOpportunitiesByStreet = new Dictionary<GameRoundType, int>();

            foreach (var item in Enum.GetValues(typeof(GameRoundType)).Cast<GameRoundType>())
            {
                this.totalTimes4BetByStreet.Add(item, 0);
                this.total4BetOpportunitiesByStreet.Add(item, 0);
            }
        }

        public bool IsOpportunity { get; private set; }

        public int TotalTimes4Bet
        {
            get
            {
                return this.totalTimes4BetByStreet.Sum(s => s.Value);
            }
        }

        public int Total4BetOpportunities
        {
            get
            {
                return this.total4BetOpportunitiesByStreet.Sum(s => s.Value);
            }
        }

        /// <summary>
        /// Gets the percentage of time a player re-raised a 3Bet
        /// </summary>
        /// <value>Percentages of 4Bet</value>
        public override double Amount
        {
            get
            {
                return this.Total4BetOpportunities == 0
                    ? 0 : ((double)this.TotalTimes4Bet / (double)this.Total4BetOpportunities) * 100.0;
            }
        }

        public double AmountByStreet(GameRoundType street)
        {
            return this.total4BetOpportunitiesByStreet[street] == 0
                    ? 0 : ((double)this.totalTimes4BetByStreet[street]
                        / (double)this.total4BetOpportunitiesByStreet[street]) * 100.0;
        }

        public override void Update(IGetTurnContext context, IStatsContext statsContext)
        {
            var raises = context.PreviousRoundActions.Count(x => x.Round == context.RoundType
                && x.Action.Type == PlayerActionType.Raise);

            if (raises == (context.RoundType == GameRoundType.PreFlop ? 2 : 3))
            {
                this.total4BetOpportunitiesByStreet[context.RoundType]++;
                this.IsOpportunity = true;
            }
        }

        public override void Update(IGetTurnContext context, PlayerAction madeAction, IStatsContext statsContext)
        {
            if (this.IsOpportunity && madeAction.Type == PlayerActionType.Raise)
            {
                this.totalTimes4BetByStreet[context.RoundType]++;
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
