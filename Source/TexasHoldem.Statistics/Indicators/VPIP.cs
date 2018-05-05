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

        public VPIP(int hands, int totalTimesVoluntarilyPutMoneyInThePot)
            : base(hands)
        {
            this.TotalTimesVoluntarilyPutMoneyInThePot = totalTimesVoluntarilyPutMoneyInThePot;
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

        public override void Update(IGetTurnContext context, PlayerAction madeAction, string playerName)
        {
            if (context.RoundType == Logic.GameRoundType.PreFlop
                && !context.PreviousRoundActions.Any(
                    p => p.PlayerName == playerName && p.Action.Type != PlayerActionType.Post))
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
            return $"{this.Amount:0.00}%";
        }

        public override VPIP DeepClone()
        {
            return new VPIP(this.Hands, this.TotalTimesVoluntarilyPutMoneyInThePot);
        }

        public override VPIP Sum(VPIP other)
        {
            return new VPIP(
                this.Hands + other.Hands,
                this.TotalTimesVoluntarilyPutMoneyInThePot + other.TotalTimesVoluntarilyPutMoneyInThePot);
        }
    }
}
