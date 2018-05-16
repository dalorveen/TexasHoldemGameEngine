namespace TexasHoldem.Statistics.Indicators
{
    using System.Linq;

    using TexasHoldem.Logic.Players;

    public class VPIP : BaseIndicator<VPIP>
    {
        public VPIP()
            : base(0)
        {
        }

        public int TotalTimesVoluntarilyPutMoneyInThePot { get; private set; }

        /// <summary>
        /// Gets the percentage of time a player voluntarily put money into the pot
        /// </summary>
        /// <value>
        /// Voluntarily put money into the pot
        /// </value>
        public override double Amount
        {
            get
            {
                return this.Hands == 0
                    ? 0 : ((double)this.TotalTimesVoluntarilyPutMoneyInThePot / (double)this.Hands) * 100.0;
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
                    this.TotalTimesVoluntarilyPutMoneyInThePot++;
                }
                else if (madeAction.Type == PlayerActionType.CheckCall && context.MoneyToCall > 0)
                {
                    this.TotalTimesVoluntarilyPutMoneyInThePot++;
                }
            }
        }

        public override string ToString()
        {
            return $"[{this.Amount:0.0}%]";
        }
    }
}
