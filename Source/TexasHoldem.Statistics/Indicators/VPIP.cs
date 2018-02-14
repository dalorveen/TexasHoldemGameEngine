namespace TexasHoldem.Statistics.Indicators
{
    using System.Linq;

    using TexasHoldem.Logic.Players;

    public class VPIP : BaseIndicator
    {
        private readonly string playerName;

        public VPIP(string playerName, int hands = 0)
            : base(hands)
        {
            this.playerName = playerName;
        }

        public VPIP(string playerName, int hands, int totalTimesVoluntarilyPutMoneyInThePot)
            : this(playerName, hands)
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
        public double Percentage
        {
            get
            {
                return this.Hands == 0 ? 0 : ((double)this.TotalTimesVoluntarilyPutMoneyInThePot / (double)this.Hands) * 100.0;
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
            return $"VPIP:{this.Percentage:0.00}%";
        }

        public override BaseIndicator DeepClone()
        {
            return new VPIP(this.playerName, this.Hands, this.TotalTimesVoluntarilyPutMoneyInThePot);
        }
    }
}
