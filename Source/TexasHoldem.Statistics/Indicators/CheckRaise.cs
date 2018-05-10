namespace TexasHoldem.Statistics.Indicators
{
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class CheckRaise : BaseIndicator<CheckRaise>
    {
        public CheckRaise()
            : base(0)
        {
        }

        public CheckRaise(int hands, int totalTimesCheckRaised, int totalCheckRaisedOpportunities)
            : base(hands)
        {
            this.TotalTimesCheckRaised = totalTimesCheckRaised;
            this.TotalCheckRaisedOpportunities = totalCheckRaisedOpportunities;
        }

        public bool IsOpportunity { get; private set; }

        public int TotalTimesCheckRaised { get; private set; }

        public int TotalCheckRaisedOpportunities { get; private set; }

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

        public override void Update(IGetTurnContext context, string playerName)
        {
            if (context.RoundType != GameRoundType.PreFlop)
            {
                var actionsOfCurrentRound = context.PreviousRoundActions.Where(p => p.Round == context.RoundType);

                if (actionsOfCurrentRound.Count(p => p.PlayerName == playerName) == 1)
                {
                    if (actionsOfCurrentRound
                        .TakeWhile(p => p.Action.Type != PlayerActionType.Raise)
                        .Any(p => p.PlayerName == playerName))
                    {
                        this.TotalCheckRaisedOpportunities++;
                        this.IsOpportunity = true;
                    }
                }
            }
            else
            {
                this.IsOpportunity = false;
            }
        }

        public override void Update(IGetTurnContext context, PlayerAction madeAction, string playerName)
        {
            if (this.IsOpportunity && madeAction.Type == PlayerActionType.Raise)
            {
                this.TotalTimesCheckRaised++;
            }

            this.IsOpportunity = false;
        }

        public override string ToString()
        {
            return $"{this.Amount:0.00}%";
        }

        public override CheckRaise DeepClone()
        {
            var copy = new CheckRaise(this.Hands, this.TotalTimesCheckRaised, this.TotalCheckRaisedOpportunities);
            copy.IsOpportunity = this.IsOpportunity;
            return copy;
        }

        public override CheckRaise Sum(CheckRaise other)
        {
            return new CheckRaise(
                this.Hands + other.Hands,
                this.TotalTimesCheckRaised + other.TotalTimesCheckRaised,
                this.TotalCheckRaisedOpportunities + other.TotalCheckRaisedOpportunities);
        }
    }
}