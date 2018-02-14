namespace TexasHoldem.Statistics.Indicators
{
    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class WTSD : BaseIndicator
    {
        private readonly string playerName;

        public WTSD(string playerName, int hands = 0)
            : base(hands)
        {
            this.playerName = playerName;
        }

        public WTSD(string playerName, int hands, int totalTimesSawTheFlop, int totalTimesWentToShowdown)
            : this(playerName, hands)
        {
            this.TotalTimesSawTheFlop = totalTimesSawTheFlop;
            this.TotalTimesWentToShowdown = totalTimesWentToShowdown;
        }

        public int TotalTimesSawTheFlop { get; private set; }

        public int TotalTimesWentToShowdown { get; private set; }

        /// <summary>
        /// Gets the percentage of times the player went to showdown after seeing the flop
        /// </summary>
        /// <value>
        /// Went to Showdown
        /// </value>
        public double Percentage
        {
            get
            {
                return this.TotalTimesSawTheFlop == 0
                    ? 0 : ((double)this.TotalTimesWentToShowdown / (double)this.TotalTimesSawTheFlop) * 100.0;
            }
        }

        public override void GetTurnExtract(IGetTurnContext context)
        {
            if (context.RoundType == GameRoundType.Flop)
            {
                this.TotalTimesSawTheFlop++;
            }
        }

        public override void EndHandExtract(IEndHandContext context)
        {
            this.TotalTimesWentToShowdown += context.ShowdownCards.ContainsKey(this.playerName) ? 1 : 0;
        }

        public override string ToString()
        {
            return $"WTSD:{this.Percentage:0.00}%";
        }

        public override BaseIndicator DeepClone()
        {
            return new WTSD(this.playerName, this.Hands, this.TotalTimesSawTheFlop, this.TotalTimesWentToShowdown);
        }
    }
}
