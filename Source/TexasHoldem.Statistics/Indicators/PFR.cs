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

        public int TotalHandsRaisedPreflop { get; private set; }

        /// <summary>
        /// Gets the percentage of time a player raised pre-flop
        /// </summary>
        /// <value>
        /// Pre-flop raise
        /// </value>
        public override double Amount
        {
            get
            {
                return this.Hands == 0 ? 0 : ((double)this.TotalHandsRaisedPreflop / (double)this.Hands) * 100.0;
            }
        }

        public override void Update(IGetTurnContext context, PlayerAction madeAction, IStatsContext statsContext)
        {
            if (context.RoundType == Logic.GameRoundType.PreFlop
                && !context.PreviousRoundActions.Any(
                    p => p.PlayerName == statsContext.PlayerName && p.Action.Type != PlayerActionType.Post))
            {
                if (madeAction.Type == PlayerActionType.Raise)
                {
                    this.TotalHandsRaisedPreflop++;
                }
            }
        }

        public override string ToString()
        {
            return $"[{this.Amount:0.0}%]";
        }
    }
}
