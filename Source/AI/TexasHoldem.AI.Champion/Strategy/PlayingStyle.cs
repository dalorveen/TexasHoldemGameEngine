namespace TexasHoldem.AI.Champion.Strategy
{
    using System.Collections.Generic;

    using HandEvaluatorExtension;
    using TexasHoldem.Logic;
    using TexasHoldem.Statistics;
    using TexasHoldem.Statistics.Indicators;

    public class PlayingStyle
    {
        public PlayingStyle()
        {
            var hands = 10000;

            this.VPIP = new StatsSetting<VPIP>(
                new VPIP(hands, 2400),
                Range.Parse("22+, A2s+, K2s+, Q8s+, J7s+, T8s+, 96s+, 85s+, 75s+, 64s+, 53s+, 42s+, 32s, A2o+," +
                    "K9o+, Q9o+, J8o+, T8o+, 98o, 87o, 76o, 65o, 54o"));

            this.PFR = new StatsSetting<PFR>(
                new PFR(hands, 1800),
                Range.Parse("22+, A2s+, K2s+, Q8s+, J8s+, T8s+, 97s+, 86s+, 75s+, 64s+, 54s, 43s, ATo+, KTo+," +
                    "QTo+, J9o+, T9o, 98o"));

            this.RFI = new Dictionary<Positions, StatsSetting<RFI>>
            {
                {
                    Positions.SB,
                    new StatsSetting<RFI>(
                        new RFI(hands, 490, 1000),
                        Range.Parse("22+, A2s+, K2s+, Q2s+, J7s+, T7s+, 96s+, 85s+, 74s+, 64s+, 53s+, 43s, 32s," +
                            "A2o+, K2o+, Q8o+, J8o+, T8o+, 97o+, 87o, 76o, 65o, 54o"))
                },
                {
                    Positions.EP,
                    new StatsSetting<RFI>(
                        new RFI(hands, 150, 1000),
                        Range.Parse("22+, A7s+, KTs+, QTs+, J9s+, T8s+, 98s, 87s, 76s, 65s, ATo+, KJo+"))
                },
                {
                    Positions.MP,
                    new StatsSetting<RFI>(
                        new RFI(hands, 170, 1000),
                         Range.Parse("22+, A7s+, A5s, KTs+, QTs+, J9s+, T8s+, 97s+, 87s, 76s, 65s, A9o+, KJo+, QJo"))
                },
                {
                    Positions.CO,
                    new StatsSetting<RFI>(
                        new RFI(hands, 250, 1000),
                         Range.Parse("22+, A2s+, K5s+, Q8s+, J8s+, T8s+, 97s+, 86s+, 76s, A5o+, K9o+, QJo"))
                },
                {
                    Positions.BTN,
                    new StatsSetting<RFI>(
                        new RFI(hands, 480, 1000),
                         Range.Parse("22+, A2s+, K2s+, Q2s+, J7s+, T7s+, 96s+, 85s+, 75s+, 64s+, 54s, 43s, A2o+," +
                            "K2o+, Q8o+, J8o+, T8o+, 97o+, 87o, 76o, 65o, 54o"))
                }
            };

            this.PreflopThreeBet = new StatsSetting<ThreeBet>(
                new ThreeBet(hands, 61, 1000),
                Range.Parse("99+, 44-22, AJs+, A4s-A2s, KQs, 87s, 76s, AQo+"));

            this.PreflopFourBet = new StatsSetting<FourBet>(
                new FourBet(hands, 23, 1000),
                Range.Parse("TT+, AJs+, A4s-A2s, KQs, AKo"));

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
        }

        public StatsSetting<VPIP> VPIP { get; }

        public StatsSetting<PFR> PFR { get; }

        public Dictionary<Positions, StatsSetting<RFI>> RFI { get; }

        public StatsSetting<ThreeBet> PreflopThreeBet { get; }

        public StatsSetting<FourBet> PreflopFourBet { get; }

        public Dictionary<GameRoundType, CBet> CBet { get; }

        public Dictionary<GameRoundType, AFq> AFq { get; }

        public VPIP VPIPDeviation(Stats stats)
        {
            return stats.VPIP().StatsForAllPositions().Sum(this.VPIP.Indicator);
        }

        public PFR PFRDeviation(Stats stats)
        {
            return stats.PFR().StatsForAllPositions().Sum(this.PFR.Indicator);
        }

        public RFI RFIDeviation(Stats stats)
        {
            var rfi = stats.RFI();
            return rfi.StatsOfCurrentPosition().Sum(this.RFI[rfi.CurrentPosition].Indicator);
        }

        public ThreeBet PreflopThreeBetDeviation(Stats stats)
        {
            return stats.ThreeBet().GetStatsBy(GameRoundType.PreFlop).StatsForAllPositions()
                .Sum(this.PreflopThreeBet.Indicator);
        }

        public FourBet PreflopFourBetDeviation(Stats stats)
        {
            return stats.FourBet().GetStatsBy(GameRoundType.PreFlop).StatsForAllPositions()
                .Sum(this.PreflopFourBet.Indicator);
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
