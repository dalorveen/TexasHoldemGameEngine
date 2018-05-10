namespace TexasHoldem.Statistics.Indicators
{
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class FourBet : BaseIndicator<FourBet>
    {
        public FourBet()
            : base(0)
        {
        }

        public FourBet(int hands, int totalTimes4Bet, int total4BetOpportunities)
            : base(hands)
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
        public override double Amount
        {
            get
            {
                return this.Total4BetOpportunities == 0
                    ? 0 : ((double)this.TotalTimes4Bet / (double)this.Total4BetOpportunities) * 100.0;
            }
        }

        public override void Update(IGetTurnContext context, string playerName)
        {
            var raises = context.PreviousRoundActions.Count(x => x.Round == context.RoundType
                && x.Action.Type == PlayerActionType.Raise);

            if (raises == (context.RoundType == GameRoundType.PreFlop ? 2 : 3))
            {
                this.Total4BetOpportunities++;
                this.IsOpportunity = true;
            }
        }

        public override void Update(IGetTurnContext context, PlayerAction madeAction, string playerName)
        {
            if (this.IsOpportunity && madeAction.Type == PlayerActionType.Raise)
            {
                this.TotalTimes4Bet++;
            }

            this.IsOpportunity = false;
        }

        public override string ToString()
        {
            return $"{this.Amount:0.00}%";
        }

        public override FourBet DeepClone()
        {
            var copy = new FourBet(this.Hands, this.TotalTimes4Bet, this.Total4BetOpportunities);
            copy.IsOpportunity = this.IsOpportunity;
            return copy;
        }

        public override FourBet Sum(FourBet other)
        {
            return new FourBet(
                this.Hands + other.Hands,
                this.TotalTimes4Bet + other.TotalTimes4Bet,
                this.Total4BetOpportunities + other.Total4BetOpportunities);
        }
    }
}
