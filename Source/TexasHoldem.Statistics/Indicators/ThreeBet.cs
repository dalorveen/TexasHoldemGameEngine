namespace TexasHoldem.Statistics.Indicators
{
    using System;
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class ThreeBet : BaseIndicator, IAdd<ThreeBet>
    {
        public ThreeBet(int hands = 0)
            : base(hands)
        {
        }

        public ThreeBet(int hands, int totalTimes3Bet, int total3BetOpportunities)
            : this(hands)
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
        public double Percentage
        {
            get
            {
                return this.Total3BetOpportunities == 0
                    ? 0 : ((double)this.TotalTimes3Bet / (double)this.Total3BetOpportunities) * 100.0;
            }
        }

        public override void GetTurnExtract(IGetTurnContext context)
        {
            var raises = context.PreviousRoundActions.Count(x => x.Action.Type == PlayerActionType.Raise);

            if (raises == (context.RoundType == GameRoundType.PreFlop ? 1 : 2))
            {
                this.Total3BetOpportunities++;
                this.IsOpportunity = true;
            }
        }

        public override void MadeActionExtract(IGetTurnContext context, PlayerAction madeAction)
        {
            if (this.IsOpportunity && madeAction.Type == PlayerActionType.Raise)
            {
                this.TotalTimes3Bet++;
            }

            this.IsOpportunity = false;
        }

        public override string ToString()
        {
            return $"{this.Percentage:0.00}%";
        }

        public override BaseIndicator DeepClone()
        {
            var copy = new ThreeBet(this.Hands, this.TotalTimes3Bet, this.Total3BetOpportunities);
            copy.IsOpportunity = this.IsOpportunity;

            return copy;
        }

        public ThreeBet Add(ThreeBet otherIndicator)
        {
            return new ThreeBet(
                this.Hands + otherIndicator.Hands,
                this.TotalTimes3Bet + otherIndicator.TotalTimes3Bet,
                this.Total3BetOpportunities + otherIndicator.Total3BetOpportunities);
        }
    }
}
