namespace TexasHoldem.Statistics.Indicators
{
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class ThreeBet : BaseIndicator<ThreeBet>
    {
        public ThreeBet()
            : base(0)
        {
        }

        public ThreeBet(int hands, int totalTimes3Bet, int total3BetOpportunities)
            : base(hands)
        {
            this.TotalTimes3Bet = totalTimes3Bet;
            this.Total3BetOpportunities = total3BetOpportunities;
        }

        public bool IsOpportunity { get; private set; }

        public int TotalTimes3Bet { get; private set; }

        public int Total3BetOpportunities { get; private set; }

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

        public override void Update(IGetTurnContext context, string playerName)
        {
            var raises = context.PreviousRoundActions.Count(x => x.Action.Type == PlayerActionType.Raise);

            if (raises == (context.RoundType == GameRoundType.PreFlop ? 1 : 2))
            {
                this.Total3BetOpportunities++;
                this.IsOpportunity = true;
            }
        }

        public override void Update(IGetTurnContext context, PlayerAction madeAction, string playerName)
        {
            if (this.IsOpportunity && madeAction.Type == PlayerActionType.Raise)
            {
                this.TotalTimes3Bet++;
            }

            this.IsOpportunity = false;
        }

        public override string ToString()
        {
            return $"{this.Amount:0.00}%";
        }

        public override ThreeBet DeepClone()
        {
            var copy = new ThreeBet(this.Hands, this.TotalTimes3Bet, this.Total3BetOpportunities);
            copy.IsOpportunity = this.IsOpportunity;
            return copy;
        }

        public override ThreeBet Sum(ThreeBet other)
        {
            return new ThreeBet(
                this.Hands + other.Hands,
                this.TotalTimes3Bet + other.TotalTimes3Bet,
                this.Total3BetOpportunities + other.Total3BetOpportunities);
        }
    }
}
