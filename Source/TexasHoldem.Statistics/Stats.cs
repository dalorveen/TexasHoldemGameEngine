namespace TexasHoldem.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics.Indicators;

    public class Stats : IStats, IUpdate
    {
        private readonly IDictionary<Type, IUpdate> indicatorsByPositions;

        private readonly IDictionary<Type, IUpdate> indicatorsByStreets;

        private int actionPriority;

        public Stats()
        {
            this.indicatorsByPositions = new Dictionary<Type, IUpdate>
            {
                { typeof(VPIP), new PositionalCollection<VPIP>() },
                { typeof(PFR), new PositionalCollection<PFR>() },
                { typeof(RFI), new PositionalCollection<RFI>() },
                { typeof(BBper100), new PositionalCollection<BBper100>() },
                { typeof(WTSD), new PositionalCollection<WTSD>() },
                { typeof(WSD), new PositionalCollection<WSD>() },
                { typeof(WWSF), new PositionalCollection<WWSF>() }
            };

            this.indicatorsByStreets = new Dictionary<Type, IUpdate>
            {
                { typeof(ThreeBet), new StreetCollection<ThreeBet>() },
                { typeof(FoldThreeBet), new StreetCollection<FoldThreeBet>() },
                { typeof(CallThreeBet), new StreetCollection<CallThreeBet>() },
                { typeof(FourBet), new StreetCollection<FourBet>() },
                { typeof(FoldFourBet), new StreetCollection<FoldFourBet>() },
                { typeof(CBet), new StreetCollection<CBet>() },
                { typeof(FoldToCBet), new StreetCollection<FoldToCBet>() },
                { typeof(AFq), new StreetCollection<AFq>() },
                { typeof(CheckRaise), new StreetCollection<CheckRaise>() }
            };
        }

        public bool IsInPosition { get; private set; }

        public PositionalCollection<VPIP> VPIP()
        {
            return (PositionalCollection<VPIP>)this.indicatorsByPositions[typeof(VPIP)];
        }

        public PositionalCollection<PFR> PFR()
        {
            return (PositionalCollection<PFR>)this.indicatorsByPositions[typeof(PFR)];
        }

        public PositionalCollection<RFI> RFI()
        {
            return (PositionalCollection<RFI>)this.indicatorsByPositions[typeof(RFI)];
        }

        public PositionalCollection<BBper100> BBper100()
        {
            return (PositionalCollection<BBper100>)this.indicatorsByPositions[typeof(BBper100)];
        }

        public PositionalCollection<WTSD> WTSD()
        {
            return (PositionalCollection<WTSD>)this.indicatorsByPositions[typeof(WTSD)];
        }

        public PositionalCollection<WSD> WSD()
        {
            return (PositionalCollection<WSD>)this.indicatorsByPositions[typeof(WSD)];
        }

        public PositionalCollection<WWSF> WWSF()
        {
            return (PositionalCollection<WWSF>)this.indicatorsByPositions[typeof(WWSF)];
        }

        public StreetCollection<ThreeBet> ThreeBet()
        {
            return (StreetCollection<ThreeBet>)this.indicatorsByStreets[typeof(ThreeBet)];
        }

        public StreetCollection<FoldThreeBet> FoldThreeBet()
        {
            return (StreetCollection<FoldThreeBet>)this.indicatorsByStreets[typeof(FoldThreeBet)];
        }

        public StreetCollection<CallThreeBet> CallThreeBet()
        {
            return (StreetCollection<CallThreeBet>)this.indicatorsByStreets[typeof(CallThreeBet)];
        }

        public StreetCollection<FourBet> FourBet()
        {
            return (StreetCollection<FourBet>)this.indicatorsByStreets[typeof(FourBet)];
        }

        public StreetCollection<FoldFourBet> FoldFourBet()
        {
            return (StreetCollection<FoldFourBet>)this.indicatorsByStreets[typeof(FoldFourBet)];
        }

        public StreetCollection<CBet> CBet()
        {
            return (StreetCollection<CBet>)this.indicatorsByStreets[typeof(CBet)];
        }

        public StreetCollection<FoldToCBet> FoldToCBet()
        {
            return (StreetCollection<FoldToCBet>)this.indicatorsByStreets[typeof(FoldToCBet)];
        }

        public StreetCollection<AFq> AFq()
        {
            return (StreetCollection<AFq>)this.indicatorsByStreets[typeof(AFq)];
        }

        public StreetCollection<CheckRaise> CheckRaise()
        {
            return (StreetCollection<CheckRaise>)this.indicatorsByStreets[typeof(CheckRaise)];
        }

        public void Update(IStartGameContext context)
        {
            // TODO: remove duplicate code!
            foreach (var item in this.indicatorsByPositions)
            {
                item.Value.Update(context);
            }

            foreach (var item in this.indicatorsByStreets)
            {
                item.Value.Update(context);
            }
        }

        public void Update(IStartHandContext context)
        {
            this.actionPriority = context.ActionPriority;

            // TODO: remove duplicate code!
            foreach (var item in this.indicatorsByPositions)
            {
                item.Value.Update(context);
            }

            foreach (var item in this.indicatorsByStreets)
            {
                item.Value.Update(context);
            }
        }

        public void Update(IStartRoundContext context)
        {
            // TODO: remove duplicate code!
            foreach (var item in this.indicatorsByPositions)
            {
                item.Value.Update(context);
            }

            foreach (var item in this.indicatorsByStreets)
            {
                item.Value.Update(context);
            }
        }

        public void Update(IGetTurnContext context, string playerName)
        {
            this.IsInPosition = context.Opponents
                .Where(p => p.InHand)
                .All(p => p.ActionPriority < this.actionPriority);

            foreach (var item in this.indicatorsByPositions)
            {
                item.Value.Update(context, playerName);
            }

            foreach (var item in this.indicatorsByStreets)
            {
                item.Value.Update(context, playerName);
            }
        }

        public void Update(IGetTurnContext context, PlayerAction playerAction, string playerName)
        {
            foreach (var item in this.indicatorsByPositions)
            {
                item.Value.Update(context, playerAction, playerName);
            }

            foreach (var item in this.indicatorsByStreets)
            {
                item.Value.Update(context, playerAction, playerName);
            }
        }

        public void Update(IEndRoundContext context)
        {
            // TODO: remove duplicate code!
            foreach (var item in this.indicatorsByPositions)
            {
                item.Value.Update(context);
            }

            foreach (var item in this.indicatorsByStreets)
            {
                item.Value.Update(context);
            }
        }

        public void Update(IEndHandContext context, string playerName)
        {
            foreach (var item in this.indicatorsByPositions)
            {
                item.Value.Update(context, playerName);
            }

            foreach (var item in this.indicatorsByStreets)
            {
                item.Value.Update(context, playerName);
            }
        }

        public void Update(IEndGameContext context)
        {
            // TODO: remove duplicate code!
            foreach (var item in this.indicatorsByPositions)
            {
                item.Value.Update(context);
            }

            foreach (var item in this.indicatorsByStreets)
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
                $"Fold3Bet:{this.FoldThreeBet().ToSimplifiedString()}\n" +
                $"Call3Bet:{this.CallThreeBet().ToSimplifiedString()}\n" +
                $"4Bet:{this.FourBet().ToSimplifiedString()}\n" +
                $"Fold4Bet:{this.FoldFourBet().ToSimplifiedString()}\n" +
                $"CBet:{this.CBet().ToSimplifiedString()}\n" +
                $"FoldToCBet:{this.FoldToCBet().ToSimplifiedString()}\n" +
                $"AFq:{this.AFq().ToSimplifiedString()}\n" +
                $"CheckRaise:{this.CheckRaise().ToSimplifiedString()}\n" +
                $"BB/100:{this.BBper100().StatsForAllPositions().ToString()}\n" +
                $"WTSD:{this.WTSD().StatsForAllPositions().ToString()}\n" +
                $"W$SD:{this.WSD().StatsForAllPositions().ToString()}\n" +
                $"W$WSF:{this.WWSF().StatsForAllPositions().ToString()}";
        }
    }
}