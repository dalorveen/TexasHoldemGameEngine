namespace TexasHoldem.Statistics.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class CheckRaise : BaseIndicator<CheckRaise>
    {
        private Dictionary<GameRoundType, int> totalTimesCheckRaisedByStreet;
        private Dictionary<GameRoundType, int> totalCheckRaisedOpportunitiesByStreet;

        public CheckRaise()
            : base(0)
        {
            this.totalTimesCheckRaisedByStreet = new Dictionary<GameRoundType, int>();
            this.totalCheckRaisedOpportunitiesByStreet = new Dictionary<GameRoundType, int>();

            foreach (var item in Enum.GetValues(typeof(GameRoundType)).Cast<GameRoundType>())
            {
                this.totalTimesCheckRaisedByStreet.Add(item, 0);
                this.totalCheckRaisedOpportunitiesByStreet.Add(item, 0);
            }
        }

        public bool IsOpportunity { get; private set; }

        public int TotalTimesCheckRaised
        {
            get
            {
                return this.totalTimesCheckRaisedByStreet.Sum(s => s.Value);
            }
        }

        public int TotalCheckRaisedOpportunities
        {
            get
            {
                return this.totalCheckRaisedOpportunitiesByStreet.Sum(s => s.Value);
            }
        }

        /// <summary>
        /// Gets the percentage of times the player check raised when they had the opportunity
        /// </summary>
        /// <value>Percentages of check raised</value>
        public override double Amount
        {
            get
            {
                return this.TotalCheckRaisedOpportunities == 0
                    ? 0 : ((double)this.TotalTimesCheckRaised / (double)this.TotalCheckRaisedOpportunities) * 100.0;
            }
        }

        public double AmountByStreet(GameRoundType street)
        {
            return this.totalCheckRaisedOpportunitiesByStreet[street] == 0
                    ? 0 : ((double)this.totalTimesCheckRaisedByStreet[street]
                        / (double)this.totalCheckRaisedOpportunitiesByStreet[street]) * 100.0;
        }

        public override void Update(IGetTurnContext context, IStatsContext statsContext)
        {
            if (context.RoundType != GameRoundType.PreFlop)
            {
                var actionsOfCurrentRound = context.PreviousRoundActions.Where(p => p.Round == context.RoundType);

                if (actionsOfCurrentRound.Count(p => p.PlayerName == statsContext.PlayerName) == 1)
                {
                    if (actionsOfCurrentRound
                        .TakeWhile(p => p.Action.Type != PlayerActionType.Raise)
                        .Any(p => p.PlayerName == statsContext.PlayerName))
                    {
                        this.totalCheckRaisedOpportunitiesByStreet[context.RoundType]++;
                        this.IsOpportunity = true;
                    }
                }
            }
            else
            {
                this.IsOpportunity = false;
            }
        }

        public override void Update(IGetTurnContext context, PlayerAction madeAction, IStatsContext statsContext)
        {
            if (this.IsOpportunity && madeAction.Type == PlayerActionType.Raise)
            {
                this.totalTimesCheckRaisedByStreet[context.RoundType]++;
            }

            this.IsOpportunity = false;
        }

        public override string ToString()
        {
            return this.ToStreetFormat(
                this.AmountByStreet(GameRoundType.Flop),
                this.AmountByStreet(GameRoundType.Turn),
                this.AmountByStreet(GameRoundType.River));
        }
    }
}