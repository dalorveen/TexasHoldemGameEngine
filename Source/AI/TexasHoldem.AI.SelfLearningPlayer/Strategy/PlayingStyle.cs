namespace TexasHoldem.AI.SelfLearningPlayer.Strategy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.AI.SelfLearningPlayer.PokerMath;

    public class PlayingStyle : IPlayingStyle
    {
        public PlayingStyle(double vpip, double pfr, double preflopThreeBet, double preflopFourBetAndMore)
        {
            this.VPIP = vpip;
            this.PFR = pfr;
            this.PreflopThreeBet = preflopThreeBet;
            this.PreflopFourBetAndMore = preflopFourBetAndMore;
        }

        public double VPIP { get; }

        public double PFR { get; }

        public double PreflopThreeBet { get; }

        public double PreflopFourBetAndMore { get; }
    }
}
