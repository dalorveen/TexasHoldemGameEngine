namespace TexasHoldem.Statistics.Indicators
{
    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class WTSD : BaseIndicator<WTSD>
    {
        private bool didThePlayerSeeTheFlop;

        public WTSD()
            : base(0)
        {
        }

        public int TotalTimesSawTheFlop { get; private set; }

        public int TotalTimesWentToShowdown { get; private set; }

        /// <summary>
        /// Gets the percentage of times the player went to showdown after seeing the flop
        /// </summary>
        /// <value>
        /// Went to Showdown
        /// </value>
        public override double Amount
        {
            get
            {
                return this.TotalTimesSawTheFlop == 0
                    ? 0 : ((double)this.TotalTimesWentToShowdown / (double)this.TotalTimesSawTheFlop) * 100.0;
            }
        }

        public override void Update(IGetTurnContext context, IStatsContext statsContext)
        {
            if (context.RoundType == GameRoundType.Flop)
            {
                this.TotalTimesSawTheFlop++;
                this.didThePlayerSeeTheFlop = true;
            }
        }

        public override void Update(IEndHandContext context, IStatsContext statsContext)
        {
            if (this.didThePlayerSeeTheFlop && context.ShowdownCards.Count > 0)
            {
                this.TotalTimesWentToShowdown += context.ShowdownCards.ContainsKey(statsContext.PlayerName) ? 1 : 0;
            }

            this.didThePlayerSeeTheFlop = false;
        }

        public override string ToString()
        {
            return $"[{this.Amount:0.0}%]";
        }
    }
}
