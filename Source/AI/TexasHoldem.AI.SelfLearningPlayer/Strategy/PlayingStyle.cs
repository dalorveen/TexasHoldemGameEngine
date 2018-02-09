namespace TexasHoldem.AI.SelfLearningPlayer.Strategy
{
    using System;
    using Logic;
    using TexasHoldem.Statistics;

    public class PlayingStyle : IStats
    {
        private readonly double threeBetPF;

        private readonly double fourBetAndMorePF;

        public PlayingStyle(double vpip, double pfr, Proportion threeBet, Proportion fourBetAndMore)
        {
            this.VPIP = vpip;
            this.PFR = pfr;
            this.ThreeBet = threeBet;
            this.FourBetAndMore = fourBetAndMore;
        }

        public double VPIP { get; }

        public double PFR { get; }

        public double AF { get; }

        public double BBPer100 { get; }

        public Proportion ThreeBet { get; }

        public Proportion FourBetAndMore { get; }
    }
}
