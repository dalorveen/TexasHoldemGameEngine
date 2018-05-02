namespace TexasHoldem.Statistics.Indicators
{
    using System.Linq;

    using TexasHoldem.Logic.Players;

    public class PFR : BaseIndicator<PFR>
    {
        public PFR()
            : base(0)
        {
        }

        public PFR(int hands, int totalHandsRaisedPreflop)
            : base(hands)
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

        public override void Update(IGetTurnContext context, PlayerAction madeAction, string playerName)
        {
            if (context.RoundType == Logic.GameRoundType.PreFlop
                && !context.PreviousRoundActions.Any(
                    p => p.PlayerName == playerName && p.Action.Type != PlayerActionType.Post))
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

        public override PFR DeepClone()
        {
            return new PFR(this.Hands, this.TotalHandsRaisedPreflop);
        }

        public override PFR Sum(PFR other)
        {
            return new PFR(
                this.Hands + other.Hands,
                this.TotalHandsRaisedPreflop + other.TotalHandsRaisedPreflop);
        }
    }
}
