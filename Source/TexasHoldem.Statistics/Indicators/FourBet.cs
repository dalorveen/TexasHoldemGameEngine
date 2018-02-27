namespace TexasHoldem.Statistics.Indicators
{
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class FourBet : BaseIndicator, IAdd<FourBet>
    {
        public FourBet(int hands = 0)
            : base(hands)
        {
        }

        public FourBet(int hands, int totalTimes4Bet, int total4BetOpportunities)
            : this(hands)
        {
            this.TotalTimes4Bet = totalTimes4Bet;
            this.Total4BetOpportunities = total4BetOpportunities;
        }

        public bool IsOpportunity { get; private set; }

        public int TotalTimes4Bet { get; private set; }

        public int Total4BetOpportunities { get; private set; }

        /// <summary>
        /// Gets the percentage of time a player re-raised a 3Bet
        /// </summary>
        /// <value>Percentages of 4Bet</value>
        public double Percentage
        {
            get
            {
                return this.Total4BetOpportunities == 0
                    ? 0 : ((double)this.TotalTimes4Bet / (double)this.Total4BetOpportunities) * 100.0;
            }
        }

        public override void GetTurnExtract(IGetTurnContext context)
        {
            var raises = context.PreviousRoundActions.Count(x => x.Action.Type == PlayerActionType.Raise);

            if (raises == (context.RoundType == GameRoundType.PreFlop ? 2 : 3))
            {
                this.Total4BetOpportunities++;
                this.IsOpportunity = true;
            }
        }

        public override void MadeActionExtract(IGetTurnContext context, PlayerAction madeAction)
        {
            if (this.IsOpportunity && madeAction.Type == PlayerActionType.Raise)
            {
                this.TotalTimes4Bet++;
            }

            this.IsOpportunity = false;
        }

        public override string ToString()
        {
            return $"{this.Percentage:0.00}%";
        }

        public override BaseIndicator DeepClone()
        {
            var copy = new FourBet(this.Hands, this.TotalTimes4Bet, this.Total4BetOpportunities);
            copy.IsOpportunity = this.IsOpportunity;
            return copy;
        }

        public FourBet Add(FourBet otherIndicator)
        {
            return new FourBet(
                this.Hands + otherIndicator.Hands,
                this.TotalTimes4Bet + otherIndicator.TotalTimes4Bet,
                this.Total4BetOpportunities + otherIndicator.Total4BetOpportunities);
        }
    }
}
