namespace TexasHoldem.AI.Champion.Strategy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Statistics;
    using TexasHoldem.Statistics.Indicators;

    public class PlayingStyle
    {
        public PlayingStyle(PlayerStyles style)
        {
            int hands = 10000;

            switch (style)
            {
                case PlayerStyles.TIGHT_AGGRESSIVE:
                    break;
                case PlayerStyles.LOOSE_AGGRESSIVE:
                    this.VPIP = new VPIP(hands, 2400);
                    this.PFR = new PFR(hands, 1800);
                    this.RFI = new Dictionary<Positions, RFI>
                    {
                        { Positions.SB, new RFI(hands, 490, 1000) },
                        { Positions.EP, new RFI(hands, 150, 1000) },
                        { Positions.MP, new RFI(hands, 170, 1000) },
                        { Positions.CO, new RFI(hands, 250, 1000) },
                        { Positions.BTN, new RFI(hands, 480, 1000) }
                    };
                    this.PreflopThreeBet = new ThreeBet(hands, 61, 1000);
                    this.PreflopFourBet = new FourBet(hands, 23, 1000);
                    this.CBet = new Dictionary<GameRoundType, CBet>
                    {
                        { GameRoundType.Flop, new CBet(hands, 600, 1000) },
                        { GameRoundType.Turn, new CBet(hands, 580, 1000) },
                        { GameRoundType.River, new CBet(hands, 410, 1000) }
                    };
                    this.AFq = new Dictionary<GameRoundType, AFq>
                    {
                        { GameRoundType.Flop, new AFq(hands, 1000, 1980, 0) },
                        { GameRoundType.Turn, new AFq(hands, 1000, 1580, 0) },
                        { GameRoundType.River, new AFq(hands, 1000, 3000, 0) }
                    };
                    break;
                default:
                    break;
            }
        }

        public VPIP VPIP { get; }

        public PFR PFR { get; }

        public Dictionary<Positions, RFI> RFI { get; }

        public ThreeBet PreflopThreeBet { get; }

        public FourBet PreflopFourBet { get; }

        public Dictionary<GameRoundType, CBet> CBet { get; }

        public Dictionary<GameRoundType, AFq> AFq { get; }

        public VPIP VPIPDeviation(Stats stats)
        {
            return stats.VPIP().StatsForAllPositions().Sum(this.VPIP);
        }

        public PFR PFRDeviation(Stats stats)
        {
            return stats.PFR().StatsForAllPositions().Sum(this.PFR);
        }

        public RFI RFIDeviation(Stats stats)
        {
            var rfi = stats.RFI();
            return rfi.StatsOfCurrentPosition().Sum(this.RFI[rfi.CurrentPosition]);
        }

        public ThreeBet PreflopThreeBetDeviation(Stats stats)
        {
            return stats.ThreeBet().GetStatsBy(GameRoundType.PreFlop).StatsForAllPositions().Sum(this.PreflopThreeBet);
        }

        public FourBet PreflopFourBetDeviation(Stats stats)
        {
            return stats.FourBet().GetStatsBy(GameRoundType.PreFlop).StatsForAllPositions().Sum(this.PreflopFourBet);
        }

        public CBet CBetDeviation(Stats stats)
        {
            var cbet = stats.CBet();
            return cbet.StatsOfCurrentStreet().StatsForAllPositions().Sum(this.CBet[cbet.CurrentStreet]);
        }

        public AFq PostflopAFqDeviation(Stats stats)
        {
            var afq = stats.AFq();
            return afq.StatsOfCurrentStreet().StatsForAllPositions().Sum(this.AFq[afq.CurrentStreet]);
        }
    }
}
