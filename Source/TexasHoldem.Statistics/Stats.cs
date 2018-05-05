namespace TexasHoldem.Statistics
{
    using System;
    using System.Collections.Generic;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics.Indicators;

    public class Stats : IStats, IUpdate
    {
        private readonly IDictionary<Type, IUpdate> indicatorsWithSingleStreet;

        private readonly IDictionary<Type, IUpdate> indicatorsWithSeveralStreets;

        public Stats()
        {
            this.indicatorsWithSingleStreet = new Dictionary<Type, IUpdate>
            {
                { typeof(VPIP), new PositionalCollection<VPIP>() },
                { typeof(PFR), new PositionalCollection<PFR>() },
                { typeof(RFI), new PositionalCollection<RFI>() },
                { typeof(BBper100), new PositionalCollection<BBper100>() },
                { typeof(WTSD), new PositionalCollection<WTSD>() },
                { typeof(WSD), new PositionalCollection<WSD>() },
                { typeof(WWSF), new PositionalCollection<WWSF>() }
            };

            this.indicatorsWithSeveralStreets = new Dictionary<Type, IUpdate>
            {
                { typeof(ThreeBet), new StreetCollection<ThreeBet>() },
                { typeof(FourBet), new StreetCollection<FourBet>() },
                { typeof(CBet), new StreetCollection<CBet>() },
                { typeof(AFq), new StreetCollection<AFq>() },
            };
        }

        public PositionalCollection<VPIP> VPIP()
        {
            return (PositionalCollection<VPIP>)this.indicatorsWithSingleStreet[typeof(VPIP)];
        }

        public PositionalCollection<PFR> PFR()
        {
            return (PositionalCollection<PFR>)this.indicatorsWithSingleStreet[typeof(PFR)];
        }

        public PositionalCollection<RFI> RFI()
        {
            return (PositionalCollection<RFI>)this.indicatorsWithSingleStreet[typeof(RFI)];
        }

        public PositionalCollection<BBper100> BBper100()
        {
            return (PositionalCollection<BBper100>)this.indicatorsWithSingleStreet[typeof(BBper100)];
        }

        public PositionalCollection<WTSD> WTSD()
        {
            return (PositionalCollection<WTSD>)this.indicatorsWithSingleStreet[typeof(WTSD)];
        }

        public PositionalCollection<WSD> WSD()
        {
            return (PositionalCollection<WSD>)this.indicatorsWithSingleStreet[typeof(WSD)];
        }

        public PositionalCollection<WWSF> WWSF()
        {
            return (PositionalCollection<WWSF>)this.indicatorsWithSingleStreet[typeof(WWSF)];
        }

        public StreetCollection<ThreeBet> ThreeBet()
        {
            return (StreetCollection<ThreeBet>)this.indicatorsWithSeveralStreets[typeof(ThreeBet)];
        }

        public StreetCollection<FourBet> FourBet()
        {
            return (StreetCollection<FourBet>)this.indicatorsWithSeveralStreets[typeof(FourBet)];
        }

        public StreetCollection<CBet> CBet()
        {
            return (StreetCollection<CBet>)this.indicatorsWithSeveralStreets[typeof(CBet)];
        }

        public StreetCollection<AFq> AFq()
        {
            return (StreetCollection<AFq>)this.indicatorsWithSeveralStreets[typeof(AFq)];
        }

        public void Update(IStartGameContext context)
        {
            // TODO: remove duplicate code!
            foreach (var item in this.indicatorsWithSingleStreet)
            {
                item.Value.Update(context);
            }

            foreach (var item in this.indicatorsWithSeveralStreets)
            {
                item.Value.Update(context);
            }
        }

        public void Update(IStartHandContext context)
        {
            // TODO: remove duplicate code!
            foreach (var item in this.indicatorsWithSingleStreet)
            {
                item.Value.Update(context);
            }

            foreach (var item in this.indicatorsWithSeveralStreets)
            {
                item.Value.Update(context);
            }
        }

        public void Update(IStartRoundContext context)
        {
            // TODO: remove duplicate code!
            foreach (var item in this.indicatorsWithSingleStreet)
            {
                item.Value.Update(context);
            }

            foreach (var item in this.indicatorsWithSeveralStreets)
            {
                item.Value.Update(context);
            }
        }

        public void Update(IGetTurnContext context, string playerName)
        {
            foreach (var item in this.indicatorsWithSingleStreet)
            {
                item.Value.Update(context, playerName);
            }

            foreach (var item in this.indicatorsWithSeveralStreets)
            {
                item.Value.Update(context, playerName);
            }
        }

        public void Update(IGetTurnContext context, PlayerAction playerAction, string playerName)
        {
            foreach (var item in this.indicatorsWithSingleStreet)
            {
                item.Value.Update(context, playerAction, playerName);
            }

            foreach (var item in this.indicatorsWithSeveralStreets)
            {
                item.Value.Update(context, playerAction, playerName);
            }
        }

        public void Update(IEndRoundContext context)
        {
            // TODO: remove duplicate code!
            foreach (var item in this.indicatorsWithSingleStreet)
            {
                item.Value.Update(context);
            }

            foreach (var item in this.indicatorsWithSeveralStreets)
            {
                item.Value.Update(context);
            }
        }

        public void Update(IEndHandContext context, string playerName)
        {
            foreach (var item in this.indicatorsWithSingleStreet)
            {
                item.Value.Update(context, playerName);
            }

            foreach (var item in this.indicatorsWithSeveralStreets)
            {
                item.Value.Update(context, playerName);
            }
        }

        public void Update(IEndGameContext context)
        {
            // TODO: remove duplicate code!
            foreach (var item in this.indicatorsWithSingleStreet)
            {
                item.Value.Update(context);
            }

            foreach (var item in this.indicatorsWithSeveralStreets)
            {
                item.Value.Update(context);
            }
        }

        public override string ToString()
        {
            return
                $"VPIP:{this.VPIP().StatsForAllPositions().ToString()}\n" +
                $"PFR:{this.PFR().StatsForAllPositions().ToString()}\n" +
                $"RFI:{this.RFI().ToString()}\n" +
                $"3Bet:{this.ThreeBet().ToSimplifiedString()}\n" +
                $"4Bet:{this.FourBet().ToSimplifiedString()}\n" +
                $"CBet:{this.CBet().ToSimplifiedString()}\n" +
                $"AFq:{this.AFq().ToSimplifiedString()}\n" +
                $"BB/100:{this.BBper100().StatsForAllPositions().ToString()}\n" +
                $"WTSD:{this.WTSD().StatsForAllPositions().ToString()}\n" +
                $"W$SD:{this.WSD().StatsForAllPositions().ToString()}\n" +
                $"W$WSF:{this.WWSF().StatsForAllPositions().ToString()}";
        }
    }
}