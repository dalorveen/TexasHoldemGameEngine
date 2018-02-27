namespace TexasHoldem.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics.Indicators;

    public class Stats : IPlayer, IStats
    {
        private readonly IPlayer player;

        private readonly IReadOnlyList<BaseIndicator> indicators;

        public Stats(IPlayer player)
        {
            this.player = player;

            this.indicators = new List<BaseIndicator>
            {
                new VPIP(this.player.Name),
                new PFR(this.player.Name),
                new PositionStorage<RFI>(this.player.Name, this.CreateIndicatorByPositions<RFI>(new RFI())),
                new StreetStorage<ThreeBet>(new Dictionary<GameRoundType, ThreeBet>
                {
                    { GameRoundType.PreFlop, new ThreeBet() },
                    { GameRoundType.Flop, new ThreeBet() },
                    { GameRoundType.Turn, new ThreeBet() },
                    { GameRoundType.River, new ThreeBet() }
                }),
                new StreetStorage<FourBet>(new Dictionary<GameRoundType, FourBet>
                {
                    { GameRoundType.PreFlop, new FourBet() },
                    { GameRoundType.Flop, new FourBet() },
                    { GameRoundType.Turn, new FourBet() },
                    { GameRoundType.River, new FourBet() }
                }),
                new StreetStorage<CBet>(new Dictionary<GameRoundType, CBet>
                {
                    { GameRoundType.Flop, new CBet(this.player.Name) },
                    { GameRoundType.Turn, new CBet(this.player.Name) },
                    { GameRoundType.River, new CBet(this.player.Name) }
                }),
                new StreetStorage<AFq>(new Dictionary<GameRoundType, AFq>
                {
                    { GameRoundType.PreFlop, new AFq() },
                    { GameRoundType.Flop, new AFq() },
                    { GameRoundType.Turn, new AFq() },
                    { GameRoundType.River, new AFq() }
                }),
                new BBper100(),
                new WTSD(player.Name),
                new WMSD(player.Name)
            };
        }

        public string Name
        {
            get
            {
                return this.player.Name;
            }
        }

        public int BuyIn
        {
            get
            {
                return this.player.BuyIn;
            }
        }

        public VPIP VPIP
        {
            get
            {
                return (VPIP)this.indicators.First(p => p is VPIP).DeepClone();
            }
        }

        public PFR PFR
        {
            get
            {
                return (PFR)this.indicators.First(p => p is PFR).DeepClone();
            }
        }

        public PositionStorage<RFI> RFI
        {
            get
            {
                return (PositionStorage<RFI>)this.indicators.First(p => p is PositionStorage<RFI>).DeepClone();
            }
        }

        public StreetStorage<ThreeBet> ThreeBet
        {
            get
            {
                return (StreetStorage<ThreeBet>)this.indicators.First(p => p is StreetStorage<ThreeBet>).DeepClone();
            }
        }

        public StreetStorage<FourBet> FourBet
        {
            get
            {
                return (StreetStorage<FourBet>)this.indicators.First(p => p is StreetStorage<FourBet>).DeepClone();
            }
        }

        public StreetStorage<CBet> CBet
        {
            get
            {
                return (StreetStorage<CBet>)this.indicators.First(p => p is StreetStorage<CBet>).DeepClone();
            }
        }

        public StreetStorage<AFq> AFq
        {
            get
            {
                return (StreetStorage<AFq>)this.indicators.First(p => p is StreetStorage<AFq>).DeepClone();
            }
        }

        public BBper100 BBper100
        {
            get
            {
                return (BBper100)this.indicators.First(p => p is BBper100).DeepClone();
            }
        }

        public WTSD WTSD
        {
            get
            {
                return (WTSD)this.indicators.First(p => p is WTSD).DeepClone();
            }
        }

        public WMSD WMSD
        {
            get
            {
                return (WMSD)this.indicators.First(p => p is WMSD).DeepClone();
            }
        }

        public PlayerAction PostingBlind(IPostingBlindContext context)
        {
            return this.player.PostingBlind(context);
        }

        public void StartGame(IStartGameContext context)
        {
        }

        public void StartHand(IStartHandContext context)
        {
            this.player.StartHand(context);

            foreach (var item in this.indicators)
            {
                item.StartHandExtract(context);
            }
        }

        public void StartRound(IStartRoundContext context)
        {
            this.player.StartRound(context);

            foreach (var item in this.indicators)
            {
                item.StartRoundExtract(context);
            }
        }

        public PlayerAction GetTurn(IGetTurnContext context)
        {
            foreach (var item in this.indicators)
            {
                // statistics before the action
                item.GetTurnExtract(context);
            }

            var madeAction = this.player.GetTurn(new GetTurnExtendedContext(context, this));

            foreach (var item in this.indicators)
            {
                // statistics after the action
                item.MadeActionExtract(context, madeAction);
            }

            return madeAction;
        }

        public void EndRound(IEndRoundContext context)
        {
            this.player.EndRound(context);

            foreach (var item in this.indicators)
            {
                item.EndRoundExtract(context);
            }
        }

        public void EndHand(IEndHandContext context)
        {
            this.player.EndHand(context);

            foreach (var item in this.indicators)
            {
                item.EndHandExtract(context);
            }
        }

        public void EndGame(IEndGameContext context)
        {
        }

        public override string ToString()
        {
            return
                $"VPIP:{this.VPIP.ToString()}\n" +
                $"PFR:{this.PFR.ToString()}\n" +
                $"RFI:{this.RFI.ToString()}\n" +
                $"3Bet:{this.ThreeBet.ToString()}\n" +
                $"4Bet:{this.FourBet.ToString()}\n" +
                $"CBet:{this.CBet.ToString()}\n" +
                $"AFq:{this.AFq.ToString()}\n" +
                $"BB/100:{this.BBper100.ToString()}\n" +
                $"WTSD:{this.WTSD.ToString()}\n" +
                $"W$SD:{this.WMSD.ToString()}";
        }

        private Dictionary<SeatNames, T> CreateIndicatorByPositions<T>(T indicator)
            where T : BaseIndicator
        {
            var indicatorByPositions = new Dictionary<SeatNames, T>();

            for (int i = 0; i < Enum.GetNames(typeof(SeatNames)).Length; i++)
            {
                indicatorByPositions.Add((SeatNames)i, (T)indicator.DeepClone());
            }

            return indicatorByPositions;
        }
    }
}
