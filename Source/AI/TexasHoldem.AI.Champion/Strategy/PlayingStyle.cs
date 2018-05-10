namespace TexasHoldem.AI.Champion.Strategy
{
    using System;
    using System.Collections.Generic;

    using HandEvaluatorExtension;
    using TexasHoldem.Logic;
    using TexasHoldem.Statistics;

    public class PlayingStyle
    {
        private static object locker = new object();

        public PlayingStyle()
        {
            lock (locker)
            {
                this.VPIP = new StatsSetting(
                    24.0,
                    Range.Parse("22+, A2s+, K2s+, Q8s+, J7s+, T8s+, 96s+, 85s+, 75s+, 64s+, 53s+, 42s+, 32s, A9o+," +
                        "K9o+, Q9o+, J8o+, T8o+, 98o, 87o, 76o, 65o, 54o"));

                this.PFR = new StatsSetting(
                    18.0,
                    Range.Parse("22+, A2s+, K2s+, Q8s+, J8s+, T8s+, 97s+, 86s+, 75s+, 64s+, 54s, 43s, ATo+, KTo+, QTo+," +
                        "J9o+, T9o, 98o"));

                this.RFI = new Dictionary<Positions, StatsSetting>
                {
                    {
                        Positions.SB,
                        new StatsSetting(
                            49.0,
                            Range.Parse("22+, A2s+, K2s+, Q2s+, J7s+, T7s+, 96s+, 85s+, 74s+, 64s+, 53s+, 43s, 32s," +
                                "A2o+, K2o+, Q8o+, J8o+, T8o+, 97o+, 87o, 76o, 65o, 54o"))
                    },
                    {
                        Positions.EP,
                        new StatsSetting(
                            15.0,
                            Range.Parse("22+, A7s+, KTs+, QTs+, J9s+, T8s+, 98s, 87s, 76s, 65s, ATo+, KJo+"))
                    },
                    {
                        Positions.MP,
                        new StatsSetting(
                            17.0,
                            Range.Parse("22+, A7s+, A5s, KTs+, QTs+, J9s+, T8s+, 97s+, 87s, 76s, 65s, A9o+, KJo+, QJo"))
                    },
                    {
                        Positions.CO,
                        new StatsSetting(
                            25.0,
                            Range.Parse("22+, A2s+, K5s+, Q8s+, J8s+, T8s+, 97s+, 86s+, 76s, A5o+, K9o+, QJo"))
                    },
                    {
                        Positions.BTN,
                        new StatsSetting(
                            48.0,
                            Range.Parse("22+, A2s+, K2s+, Q2s+, J7s+, T7s+, 96s+, 85s+, 75s+, 64s+, 54s, 43s, A2o+," +
                                "K2o+, Q8o+, J8o+, T8o+, 97o+, 87o, 76o, 65o, 54o"))
                    }
                };

                this.PreflopThreeBet = new StatsSetting(
                    5.2,
                    Range.Parse("99+, 44-22, AJs+, A4s-A2s, KQs, 87s, 76s, AQo+"));

                this.PreflopCallThreeBet = new StatsSetting(
                    29.0,
                    Range.Parse("22+, A2s+, K9s+, Q8s+, J8s+, T8s+, 97s+, 86s+, 75s+, 64s+, 54s, 43s, 32s, ATo+," +
                        "KTo+, QTo+, JTo"));

                this.PreflopFourBet = new StatsSetting(
                    2.3,
                    Range.Parse("TT+, AJs+, KQs, AKo"));

                this.PreflopFoldThreeBet = 62.0;

                this.PreflopFoldFourBet = 41.0;

                this.CBet = new Dictionary<GameRoundType, double>
                {
                    { GameRoundType.Flop, 60.0 },
                    { GameRoundType.Turn, 58.0 },
                    { GameRoundType.River, 41.0 }
                };

                this.FoldToCBet = new Dictionary<GameRoundType, double>
                {
                    { GameRoundType.Flop, 50.0 },
                    { GameRoundType.Turn, 41.0 },
                    { GameRoundType.River, 50.0 }
                };

                this.AFq = new Dictionary<GameRoundType, double>
                {
                    { GameRoundType.Flop, 36.0 },
                    { GameRoundType.Turn, 42.0 },
                    { GameRoundType.River, 25.0 }
                };

                this.CheckRaise = new Dictionary<GameRoundType, double>
                {
                    { GameRoundType.Flop, 8.6 },
                    { GameRoundType.Turn, 9.2 }
                };
            }
        }

        public StatsSetting VPIP { get; }

        public StatsSetting PFR { get; }

        public Dictionary<Positions, StatsSetting> RFI { get; }

        public StatsSetting PreflopThreeBet { get; }

        public StatsSetting PreflopCallThreeBet { get; }

        public StatsSetting PreflopFourBet { get; }

        public double PreflopFoldThreeBet { get; }

        public double PreflopFoldFourBet { get; }

        public Dictionary<GameRoundType, double> CBet { get; }

        public Dictionary<GameRoundType, double> FoldToCBet { get; }

        public Dictionary<GameRoundType, double> AFq { get; }

        public Dictionary<GameRoundType, double> CheckRaise { get; }

        public double VPIPDeviation(Stats stats)
        {
            return stats.VPIP().StatsForAllPositions().Amount - this.VPIP.Amount;
        }

        public double PFRDeviation(Stats stats)
        {
            return stats.PFR().StatsForAllPositions().Amount - this.PFR.Amount;
        }

        public double RFIDeviation(Stats stats)
        {
            var rfi = stats.RFI();
            return rfi.StatsOfCurrentPosition().Amount - this.RFI[rfi.CurrentPosition].Amount;
        }

        public double PreflopThreeBetDeviation(Stats stats)
        {
            return stats.ThreeBet().GetStatsBy(GameRoundType.PreFlop).StatsForAllPositions().Amount
                - this.PreflopThreeBet.Amount;
        }

        public double PreflopCallThreeBetDeviation(Stats stats)
        {
            return stats.CallThreeBet().GetStatsBy(GameRoundType.PreFlop).StatsForAllPositions().Amount
                - this.PreflopCallThreeBet.Amount;
        }

        public double PreflopFourBetDeviation(Stats stats)
        {
            return stats.FourBet().GetStatsBy(GameRoundType.PreFlop).StatsForAllPositions().Amount
                - this.PreflopFourBet.Amount;
        }

        public double PreflopFoldThreeBetDeviation(Stats stats)
        {
            return stats.FoldThreeBet().GetStatsBy(GameRoundType.PreFlop).StatsForAllPositions().Amount
                - this.PreflopFoldThreeBet;
        }

        public double PreflopFoldFourBetDeviation(Stats stats)
        {
            return stats.FoldFourBet().GetStatsBy(GameRoundType.PreFlop).StatsForAllPositions().Amount
                - this.PreflopFoldFourBet;
        }

        public double CBetDeviation(Stats stats)
        {
            var cbet = stats.CBet();
            return cbet.StatsOfCurrentStreet().StatsForAllPositions().Amount - this.CBet[cbet.CurrentStreet];
        }

        public double FoldToCBetDeviation(Stats stats)
        {
            var foldToCBet = stats.FoldToCBet();
            return foldToCBet.StatsOfCurrentStreet().StatsForAllPositions().Amount
                - this.FoldToCBet[foldToCBet.CurrentStreet];
        }

        public double PostflopAFqDeviation(Stats stats)
        {
            var afq = stats.AFq();
            return afq.StatsOfCurrentStreet().StatsForAllPositions().Amount - this.AFq[afq.CurrentStreet];
        }

        public double CheckRaiseDeviation(Stats stats)
        {
            var checkRaise = stats.CheckRaise();
            return checkRaise.StatsOfCurrentStreet().StatsForAllPositions().Amount
                - this.CheckRaise[checkRaise.CurrentStreet];
        }
    }
}
