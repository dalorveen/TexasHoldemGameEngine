namespace TexasHoldem.Statistics.Indicators
{
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class FoldFourBet : BaseIndicator<FoldFourBet>
    {
        public FoldFourBet()
            : base(0)
        {
        }

        public FoldFourBet(int hands, int totalTimesFoldedTo4Bet, int totalTimesFaced4Bet)
            : base(hands)
        {
            this.TotalTimesFoldedTo4Bet = totalTimesFoldedTo4Bet;
            this.TotalTimesFaced4Bet = totalTimesFaced4Bet;
        }

        public bool Faced4Bet { get; private set; }

        public int TotalTimesFoldedTo4Bet { get; private set; }

        public int TotalTimesFaced4Bet { get; private set; }

        /// <summary>
        /// Gets the percentage of times the player folded when facing a 4-bet
        /// </summary>
        /// <value>Percentages of fold to 4Bet</value>
        public override double Amount
        {
            get
            {
                return this.TotalTimesFaced4Bet == 0
                    ? 0 : ((double)this.TotalTimesFoldedTo4Bet / (double)this.TotalTimesFaced4Bet) * 100.0;
            }
        }

        public override void Update(IGetTurnContext context, string playerName)
        {
            var raises = context.PreviousRoundActions.Count(x => x.Round == context.RoundType
                && x.Action.Type == PlayerActionType.Raise);

            if (raises == (context.RoundType == GameRoundType.PreFlop ? 3 : 4))
            {
                this.TotalTimesFaced4Bet++;
                this.Faced4Bet = true;
            }
        }

        public override void Update(IGetTurnContext context, PlayerAction madeAction, string playerName)
        {
            if (this.Faced4Bet && madeAction.Type == PlayerActionType.Fold)
            {
                this.TotalTimesFoldedTo4Bet++;
            }

            this.Faced4Bet = false;
        }

        public override string ToString()
        {
            return $"{this.Amount:0.00}%";
        }

        public override FoldFourBet DeepClone()
        {
            var copy = new FoldFourBet(this.Hands, this.TotalTimesFoldedTo4Bet, this.TotalTimesFaced4Bet);
            copy.Faced4Bet = this.Faced4Bet;
            return copy;
        }

        public override FoldFourBet Sum(FoldFourBet other)
        {
            return new FoldFourBet(
                this.Hands + other.Hands,
                this.TotalTimesFoldedTo4Bet + other.TotalTimesFoldedTo4Bet,
                this.TotalTimesFaced4Bet + other.TotalTimesFaced4Bet);
        }
    }
}