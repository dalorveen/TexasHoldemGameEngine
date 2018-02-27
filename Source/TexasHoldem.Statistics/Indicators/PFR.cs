namespace TexasHoldem.Statistics.Indicators
{
    using System.Linq;

    using TexasHoldem.Logic.Players;

    public class PFR : BaseIndicator
    {
        private readonly string playerName;

        public PFR(string playerName, int hands = 0)
            : base(hands)
        {
            this.playerName = playerName;
        }

        public PFR(string playerName, int hands, int totalHandsRaisedPreflop)
            : this(playerName, hands)
        {
            this.TotalHandsRaisedPreflop = totalHandsRaisedPreflop;
        }

        public int TotalHandsRaisedPreflop { get; private set; }

        /// <summary>
        /// Gets the percentage of time a player raised pre-flop
        /// </summary>
        /// <value>
        /// Pre-flop raise
        /// </value>
        public double Percentage
        {
            get
            {
                return this.Hands == 0 ? 0 : ((double)this.TotalHandsRaisedPreflop / (double)this.Hands) * 100.0;
            }
        }

        public override void MadeActionExtract(IGetTurnContext context, PlayerAction madeAction)
        {
            if (context.RoundType == Logic.GameRoundType.PreFlop
                && !context.PreviousRoundActions.Any(
                    p => p.PlayerName == this.playerName && p.Action.Type != PlayerActionType.Post))
            {
                if (madeAction.Type == PlayerActionType.Raise)
                {
                    this.TotalHandsRaisedPreflop++;
                }
            }
        }

        public override string ToString()
        {
            return $"{this.Percentage:0.00}%";
        }

        public override BaseIndicator DeepClone()
        {
            return new PFR(this.playerName, this.Hands, this.TotalHandsRaisedPreflop);
        }
    }
}
