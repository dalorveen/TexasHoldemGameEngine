namespace TexasHoldem.AI.Champion.Strategy
{
    using TexasHoldem.Statistics;
    using TexasHoldem.Statistics.Indicators;

    public class PlayingStyle : IStats
    {
        public VPIP VPIP { get; set; }

        public PFR PFR { get; set; }

        public ThreeBet ThreeBet { get; set; }

        public FourBet FourBet { get; set; }

        public CBet CBet { get; set; }

        public AFq AFq { get; set; }

        public BBper100 BBper100 { get; set; }

        public WTSD WTSD { get; set; }
    }
}
