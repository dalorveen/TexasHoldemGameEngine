namespace TexasHoldem.AI.Champion.Strategy
{
    using TexasHoldem.Statistics;
    using TexasHoldem.Statistics.Indicators;

    public class PlayingStyle : IStats
    {
        public VPIP VPIP { get; set; }

        public PFR PFR { get; set; }

        public PositionStorage<RFI> RFI { get; set; }

        public StreetStorage<ThreeBet> ThreeBet { get; set; }

        public StreetStorage<FourBet> FourBet { get; set; }

        public StreetStorage<CBet> CBet { get; set; }

        public StreetStorage<AFq> AFq { get; set; }

        public BBper100 BBper100 { get; set; }

        public WTSD WTSD { get; set; }

        public WMSD WMSD { get; set; }
    }
}
