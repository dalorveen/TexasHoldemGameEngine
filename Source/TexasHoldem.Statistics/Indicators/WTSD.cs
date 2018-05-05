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

        public WTSD(int hands, int totalTimesSawTheFlop, int totalTimesWentToShowdown)
            : base(hands)
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
        public override double Amount
        {
            get
            {
                return this.TotalTimesSawTheFlop == 0
                    ? 0 : ((double)this.TotalTimesWentToShowdown / (double)this.TotalTimesSawTheFlop) * 100.0;
            }
        }

        public override void Update(IGetTurnContext context, string playerName)
        {
            if (context.RoundType == GameRoundType.Flop)
            {
                this.TotalTimesSawTheFlop++;
                this.didThePlayerSeeTheFlop = true;
            }
        }

        public override void Update(IEndHandContext context, string playerName)
        {
            if (this.didThePlayerSeeTheFlop && context.ShowdownCards.Count > 0)
            {
                this.TotalTimesWentToShowdown += context.ShowdownCards.ContainsKey(playerName) ? 1 : 0;
            }

            this.didThePlayerSeeTheFlop = false;
        }

        public override string ToString()
        {
            return $"{this.Amount:0.00}%";
        }

        public override WTSD DeepClone()
        {
            return new WTSD(this.Hands, this.TotalTimesSawTheFlop, this.TotalTimesWentToShowdown);
        }

        public override WTSD Sum(WTSD other)
        {
            return new WTSD(
                this.Hands + other.Hands,
                this.TotalTimesSawTheFlop + other.TotalTimesSawTheFlop,
                this.TotalTimesWentToShowdown + other.TotalTimesWentToShowdown);
        }
    }
}
