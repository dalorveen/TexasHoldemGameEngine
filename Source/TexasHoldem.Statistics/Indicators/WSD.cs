namespace TexasHoldem.Statistics.Indicators
{
    using TexasHoldem.Logic.Players;

    public class WSD : BaseIndicator<WSD>
    {
        private int moneyInTheBeginningOfTheHand;

        public WSD()
            : base(0)
        {
        }

        public int TotalTimesWonMoneyAtShowdown { get; private set; }

        public int TotalTimesWentToShowdown { get; private set; }

        /// <summary>
        /// Gets the percentage of times the player won money when going to showdown,
        /// including split pots where the player won less than they bet
        /// </summary>
        /// <value>
        /// Won Money at Showdown
        /// </value>
        public override double Amount
        {
            get
            {
                return this.TotalTimesWentToShowdown == 0
                    ? 0 : ((double)this.TotalTimesWonMoneyAtShowdown / (double)this.TotalTimesWentToShowdown) * 100.0;
            }
        }

        public override void Update(IStartHandContext context)
        {
            base.Update(context);
            this.moneyInTheBeginningOfTheHand = context.MoneyLeft;
        }

        public override void Update(IEndHandContext context, IStatsContext statsContext)
        {
            if (context.ShowdownCards.Count > 0)
            {
                var balance = context.MoneyLeft - this.moneyInTheBeginningOfTheHand;

                this.TotalTimesWentToShowdown += context.ShowdownCards.ContainsKey(statsContext.PlayerName) ? 1 : 0;
                this.TotalTimesWonMoneyAtShowdown += balance > 0 ? 1 : 0;
            }
        }

        public override string ToString()
        {
            return $"[{this.Amount:0.0}%]";
        }
    }
}