namespace TexasHoldem.Statistics
{
    using System;
    using System.Collections.Generic;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics.Indicators;

    public class Stats : /*IStats,*/ IUpdate
    {
        private readonly IDictionary<Type, IUpdate> indicatorsWithSingleStreet;

        private readonly IDictionary<Type, IUpdate> indicatorsWithSeveralStreets;

        public Stats()
        {
            this.indicatorsWithSingleStreet = new Dictionary<Type, IUpdate>
            {
                { typeof(VPIP), new SingleStreet<VPIP>(new Positions[0]) },
                { typeof(PFR), new SingleStreet<PFR>(new Positions[0]) },
                { typeof(RFI), new SingleStreet<RFI>(new Positions[] { Positions.BB }) },
                { typeof(BBper100), new SingleStreet<BBper100>(new Positions[0]) },
                { typeof(WTSD), new SingleStreet<WTSD>(new Positions[0]) },
                { typeof(WSD), new SingleStreet<WSD>(new Positions[0]) },
                { typeof(WWSF), new SingleStreet<WWSF>(new Positions[0]) }
            };

            this.indicatorsWithSeveralStreets = new Dictionary<Type, IUpdate>
            {
                { typeof(ThreeBet), new SeveralStreets<ThreeBet>(new GameRoundType[0], new Positions[0]) },
                { typeof(FourBet), new SeveralStreets<FourBet>(new GameRoundType[0], new Positions[0]) },
                {
                    typeof(CBet),
                    new SeveralStreets<CBet>(new GameRoundType[] { GameRoundType.PreFlop }, new Positions[0])
                },
                { typeof(AFq), new SeveralStreets<AFq>(new GameRoundType[0], new Positions[0]) },
            };
        }

        public SingleStreet<VPIP> VPIP()
        {
            return (SingleStreet<VPIP>)this.indicatorsWithSingleStreet[typeof(VPIP)];
        }

        public SingleStreet<PFR> PFR()
        {
            return (SingleStreet<PFR>)this.indicatorsWithSingleStreet[typeof(PFR)];
        }

        public SingleStreet<RFI> RFI()
        {
            return (SingleStreet<RFI>)this.indicatorsWithSingleStreet[typeof(RFI)];
        }

        public SingleStreet<BBper100> BBper100()
        {
            return (SingleStreet<BBper100>)this.indicatorsWithSingleStreet[typeof(BBper100)];
        }

        public SingleStreet<WTSD> WTSD()
        {
            return (SingleStreet<WTSD>)this.indicatorsWithSingleStreet[typeof(WTSD)];
        }

        public SingleStreet<WSD> WSD()
        {
            return (SingleStreet<WSD>)this.indicatorsWithSingleStreet[typeof(WSD)];
        }

        public SingleStreet<WWSF> WWSF()
        {
            return (SingleStreet<WWSF>)this.indicatorsWithSingleStreet[typeof(WWSF)];
        }

        public SeveralStreets<ThreeBet> ThreeBet()
        {
            return (SeveralStreets<ThreeBet>)this.indicatorsWithSeveralStreets[typeof(ThreeBet)];
        }

        public SeveralStreets<FourBet> FourBet()
        {
            return (SeveralStreets<FourBet>)this.indicatorsWithSeveralStreets[typeof(FourBet)];
        }

        public SeveralStreets<CBet> CBet()
        {
            return (SeveralStreets<CBet>)this.indicatorsWithSeveralStreets[typeof(CBet)];
        }

        public SeveralStreets<AFq> AFq()
        {
            return (SeveralStreets<AFq>)this.indicatorsWithSeveralStreets[typeof(AFq)];
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
                $"VPIP:{this.VPIP().ToString()}\n" +
                $"PFR:{this.PFR().ToString()}\n" +
                $"RFI:{this.RFI().ToString()}\n" +
                $"3Bet:{this.ThreeBet().ToString()}\n" +
                $"4Bet:{this.FourBet().ToString()}\n" +
                $"CBet:{this.CBet().ToString()}\n" +
                $"AFq:{this.AFq().ToString()}\n" +
                $"BB/100:{this.BBper100().ToString()}\n" +
                $"WTSD:{this.WTSD().ToString()}\n" +
                $"W$SD:{this.WSD().ToString()}\n" +
                $"W$WSF:{this.WWSF().ToString()}";
        }
    }
}