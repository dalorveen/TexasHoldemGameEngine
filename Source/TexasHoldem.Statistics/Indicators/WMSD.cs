namespace TexasHoldem.Statistics.Indicators
{
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class WMSD : BaseIndicator
    {
        private readonly string playerName;

        public WMSD(string playerName, int hands = 0)
            : base(hands)
        {
            this.playerName = playerName;
        }

        public WMSD(string playerName, int hands, int totalTimesWonMoneyAtShowdown, int totalTimesWentToShowdown)
            : this(playerName, hands)
        {
            this.TotalTimesWonMoneyAtShowdown = totalTimesWonMoneyAtShowdown;
            this.TotalTimesWentToShowdown = totalTimesWentToShowdown;
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
        public double Percentage
        {
            get
            {
                return this.TotalTimesWentToShowdown == 0
                    ? 0 : ((double)this.TotalTimesWonMoneyAtShowdown / (double)this.TotalTimesWentToShowdown) * 100.0;
            }
        }

        public override void EndHandExtract(IEndHandContext context)
        {
            if (context.ShowdownCards.Count > 0)
            {
                this.TotalTimesWentToShowdown += context.ShowdownCards.ContainsKey(this.playerName) ? 1 : 0;
                this.TotalTimesWonMoneyAtShowdown += context.PlayersWhoWonMoney.Contains(this.playerName) ? 1 : 0;
            }
        }

        public override string ToString()
        {
            return $"{this.Percentage:0.00}%";
        }

        public override BaseIndicator DeepClone()
        {
            return new WMSD(this.playerName, this.Hands, this.TotalTimesWonMoneyAtShowdown, this.TotalTimesWentToShowdown);
        }
    }
}