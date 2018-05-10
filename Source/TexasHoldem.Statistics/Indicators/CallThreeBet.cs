namespace TexasHoldem.Statistics.Indicators
{
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class CallThreeBet : BaseIndicator<CallThreeBet>
    {
        public CallThreeBet()
            : base(0)
        {
        }

        public CallThreeBet(int hands, int totalTimesCalled3Bet, int totalTimesFaced3Bet)
            : base(hands)
        {
            this.TotalTimesCalled3Bet = totalTimesCalled3Bet;
            this.TotalTimesFaced3Bet = totalTimesFaced3Bet;
        }

        public bool Faced3Bet { get; private set; }

        public int TotalTimesCalled3Bet { get; private set; }

        public int TotalTimesFaced3Bet { get; private set; }

        /// <summary>
        /// Gets the percentage of hands where the player called a 3-bet
        /// </summary>
        /// <value>Percentages called 3-bet</value>
        public override double Amount
        {
            get
            {
                return this.TotalTimesFaced3Bet == 0
                    ? 0 : ((double)this.TotalTimesCalled3Bet / (double)this.TotalTimesFaced3Bet) * 100.0;
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
            if (this.Faced3Bet && madeAction.Type == PlayerActionType.CheckCall)
            {
                this.TotalTimesCalled3Bet++;
            }

            this.Faced3Bet = false;
        }

        public override string ToString()
        {
            return $"{this.Amount:0.00}%";
        }

        public override CallThreeBet DeepClone()
        {
            var copy = new CallThreeBet(this.Hands, this.TotalTimesCalled3Bet, this.TotalTimesFaced3Bet);
            copy.Faced3Bet = this.Faced3Bet;
            return copy;
        }

        public override CallThreeBet Sum(CallThreeBet other)
        {
            return new CallThreeBet(
                this.Hands + other.Hands,
                this.TotalTimesCalled3Bet + other.TotalTimesCalled3Bet,
                this.TotalTimesFaced3Bet + other.TotalTimesFaced3Bet);
        }
    }
}