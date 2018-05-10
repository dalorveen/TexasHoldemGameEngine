namespace TexasHoldem.Statistics.Indicators
{
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class FoldThreeBet : BaseIndicator<FoldThreeBet>
    {
        public FoldThreeBet()
            : base(0)
        {
        }

        public FoldThreeBet(int hands, int totalTimesFoldedTo3Bet, int totalTimesFaced3Bet)
            : base(hands)
        {
            this.TotalTimesFoldedTo3Bet = totalTimesFoldedTo3Bet;
            this.TotalTimesFaced3Bet = totalTimesFaced3Bet;
        }

        public bool Faced3Bet { get; private set; }

        public int TotalTimesFoldedTo3Bet { get; private set; }

        public int TotalTimesFaced3Bet { get; private set; }

        /// <summary>
        /// Gets the percentage of times the player folded when facing a 3-bet
        /// </summary>
        /// <value>Percentages of fold to 3Bet</value>
        public override double Amount
        {
            get
            {
                return this.TotalTimesFaced3Bet == 0
                    ? 0 : ((double)this.TotalTimesFoldedTo3Bet / (double)this.TotalTimesFaced3Bet) * 100.0;
            }
        }

        public override void Update(IGetTurnContext context, string playerName)
        {
            var raises = context.PreviousRoundActions.Count(x => x.Round == context.RoundType
                && x.Action.Type == PlayerActionType.Raise);

            if (raises == (context.RoundType == GameRoundType.PreFlop ? 2 : 3))
            {
                this.TotalTimesFaced3Bet++;
                this.Faced3Bet = true;
            }
        }

        public override void Update(IGetTurnContext context, PlayerAction madeAction, string playerName)
        {
            if (this.Faced3Bet && madeAction.Type == PlayerActionType.Fold)
            {
                this.TotalTimesFoldedTo3Bet++;
            }

            this.Faced3Bet = false;
        }

        public override string ToString()
        {
            return $"{this.Amount:0.00}%";
        }

        public override FoldThreeBet DeepClone()
        {
            var copy = new FoldThreeBet(this.Hands, this.TotalTimesFoldedTo3Bet, this.TotalTimesFaced3Bet);
            copy.Faced3Bet = this.Faced3Bet;
            return copy;
        }

        public override FoldThreeBet Sum(FoldThreeBet other)
        {
            return new FoldThreeBet(
                this.Hands + other.Hands,
                this.TotalTimesFoldedTo3Bet + other.TotalTimesFoldedTo3Bet,
                this.TotalTimesFaced3Bet + other.TotalTimesFaced3Bet);
        }
    }
}
