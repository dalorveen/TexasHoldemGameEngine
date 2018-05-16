namespace TexasHoldem.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics.Indicators;

    public class Stats : IStats
    {
        private readonly string playerName;
        private readonly IDictionary<Type, IUpdate> indicators;

        private int actionPriority;

        public Stats(string playerName)
        {
            this.playerName = playerName;

            this.indicators = new Dictionary<Type, IUpdate>
            {
                { typeof(VPIP), new VPIP() },
                { typeof(PFR), new PFR() },
                { typeof(RFI), new RFI() },
                { typeof(BBper100), new BBper100() },
                { typeof(WTSD), new WTSD() },
                { typeof(WSD), new WSD() },
                { typeof(WWSF), new WWSF() },
                { typeof(ThreeBet), new ThreeBet() },
                { typeof(FoldThreeBet), new FoldThreeBet() },
                { typeof(CallThreeBet), new CallThreeBet() },
                { typeof(FourBet), new FourBet() },
                { typeof(FoldFourBet), new FoldFourBet() },
                { typeof(CBet), new CBet() },
                { typeof(FoldToCBet), new FoldToCBet() },
                { typeof(AFq), new AFq() },
                { typeof(CheckRaise), new CheckRaise() }
            };
        }

        public TablePosition Position { get; private set; }

        public bool IsInPosition { get; private set; }

        public GameRoundType Street { get; private set; }

        public VPIP VPIP()
        {
            return (VPIP)this.indicators[typeof(VPIP)];
        }

        public PFR PFR()
        {
            return (PFR)this.indicators[typeof(PFR)];
        }

        public RFI RFI()
        {
            return (RFI)this.indicators[typeof(RFI)];
        }

        public BBper100 BBper100()
        {
            return (BBper100)this.indicators[typeof(BBper100)];
        }

        public WTSD WTSD()
        {
            return (WTSD)this.indicators[typeof(WTSD)];
        }

        public WSD WSD()
        {
            return (WSD)this.indicators[typeof(WSD)];
        }

        public WWSF WWSF()
        {
            return (WWSF)this.indicators[typeof(WWSF)];
        }

        public ThreeBet ThreeBet()
        {
            return (ThreeBet)this.indicators[typeof(ThreeBet)];
        }

        public FoldThreeBet FoldThreeBet()
        {
            return (FoldThreeBet)this.indicators[typeof(FoldThreeBet)];
        }

        public CallThreeBet CallThreeBet()
        {
            return (CallThreeBet)this.indicators[typeof(CallThreeBet)];
        }

        public FourBet FourBet()
        {
            return (FourBet)this.indicators[typeof(FourBet)];
        }

        public FoldFourBet FoldFourBet()
        {
            return (FoldFourBet)this.indicators[typeof(FoldFourBet)];
        }

        public CBet CBet()
        {
            return (CBet)this.indicators[typeof(CBet)];
        }

        public FoldToCBet FoldToCBet()
        {
            return (FoldToCBet)this.indicators[typeof(FoldToCBet)];
        }

        public AFq AFq()
        {
            return (AFq)this.indicators[typeof(AFq)];
        }

        public CheckRaise CheckRaise()
        {
            return (CheckRaise)this.indicators[typeof(CheckRaise)];
        }

        public void Update(IStartGameContext context)
        {
            this.Position = new TablePosition(context);

            foreach (var item in this.indicators)
            {
                item.Value.Update(context);
            }
        }

        public void Update(IStartHandContext context)
        {
            this.Position.SetCurrentPosition(context);
            this.actionPriority = context.ActionPriority;

            foreach (var item in this.indicators)
            {
                item.Value.Update(context);
            }
        }

        public void Update(IStartRoundContext context)
        {
            this.Street = context.RoundType;

            foreach (var item in this.indicators)
            {
                item.Value.Update(context);
            }
        }

        public void Update(IGetTurnContext context)
        {
            this.IsInPosition = context.Opponents
                .Where(p => p.InHand)
                .All(p => p.ActionPriority < this.actionPriority);

            foreach (var item in this.indicators)
            {
                item.Value.Update(context, new StatsContext(this.playerName, this.Position));
            }
        }

        public void Update(IGetTurnContext context, PlayerAction playerAction)
        {
            foreach (var item in this.indicators)
            {
                item.Value.Update(context, playerAction, new StatsContext(this.playerName, this.Position));
            }
        }

        public void Update(IEndRoundContext context)
        {
            foreach (var item in this.indicators)
            {
                item.Value.Update(context);
            }
        }

        public void Update(IEndHandContext context)
        {
            foreach (var item in this.indicators)
            {
                item.Value.Update(context, new StatsContext(this.playerName, this.Position));
            }
        }

        public void Update(IEndGameContext context)
        {
            foreach (var item in this.indicators)
            {
                item.Value.Update(context);
            }
        }

        public override string ToString()
        {
            return
                $"VPIP: {this.VPIP().ToString()}\n" +
                $"PFR: {this.PFR().ToString()}\n" +
                $"RFI: {this.RFI().ToString()}\n" +
                $"3Bet: {this.ThreeBet().ToString()}\n" +
                $"Fold3Bet: {this.FoldThreeBet().ToString()}\n" +
                $"Call3Bet: {this.CallThreeBet().ToString()}\n" +
                $"4Bet: {this.FourBet().ToString()}\n" +
                $"Fold4Bet: {this.FoldFourBet().ToString()}\n" +
                $"CBet: {this.CBet().ToString()}\n" +
                $"FoldToCBet: {this.FoldToCBet().ToString()}\n" +
                $"AFq: {this.AFq().ToString()}\n" +
                $"CheckRaise: {this.CheckRaise().ToString()}\n" +
                $"BB/100: {this.BBper100().ToString()}\n" +
                $"WTSD: {this.WTSD().ToString()}\n" +
                $"W$SD: {this.WSD().ToString()}\n" +
                $"W$WSF: {this.WWSF().ToString()}";
        }
    }
}