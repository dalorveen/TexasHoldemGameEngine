namespace TexasHoldem.Statistics
{
    using System.Collections.Generic;
    using System.Linq;

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
                new VPIP(player.Name),
                new PFR(player.Name),
                new ThreeBet(),
                new FourBet(),
                new CBet(player.Name),
                new AFq(),
                new BBper100(),
                new WTSD(player.Name)
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

        public ThreeBet ThreeBet
        {
            get
            {
                return (ThreeBet)this.indicators.First(p => p is ThreeBet).DeepClone();
            }
        }

        public FourBet FourBet
        {
            get
            {
                return (FourBet)this.indicators.First(p => p is FourBet).DeepClone();
            }
        }

        public CBet CBet
        {
            get
            {
                return (CBet)this.indicators.First(p => p is CBet).DeepClone();
            }
        }

        public AFq AFq
        {
            get
            {
                return (AFq)this.indicators.First(p => p is AFq).DeepClone();
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

        public PlayerAction PostingBlind(IPostingBlindContext context)
        {
            return this.player.PostingBlind(context);
        }

        public void StartGame(IStartGameContext context)
        {
            this.player.StartGame(context);

            foreach (var item in this.indicators)
            {
                item.StartGameExtract(context);
            }
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
            this.player.EndGame(context);

            foreach (var item in this.indicators)
            {
                item.EndGameExtract(context);
            }
        }

        public override string ToString()
        {
            return
                $"{this.VPIP.ToString()}\n" +
                $"{this.PFR.ToString()}\n" +
                $"{this.ThreeBet.ToString()}\n" +
                $"{this.FourBet.ToString()}\n" +
                $"{this.CBet.ToString()}\n" +
                $"{this.AFq.ToString()}\n" +
                $"{this.BBper100.ToString()}\n" +
                $"{this.WTSD.ToString()}";
        }
    }
}
