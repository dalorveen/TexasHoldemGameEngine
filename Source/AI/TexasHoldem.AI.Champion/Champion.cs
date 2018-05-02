namespace TexasHoldem.AI.Champion
{
    using System;
    using System.Collections.Generic;

    using HandEvaluatorExtension;
    using TexasHoldem.AI.Champion.Strategy;
    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;
    using TexasHoldem.Statistics.Indicators;

    public class Champion : BasePlayer
    {
        private readonly Stats stats;

        private readonly IStats playingStyle;

        private ICardAdapter pocket;

        private BaseBehavior preflopBehavior;

        private BaseBehavior postflopBehavior;

        public Champion(IStats playingStyle, int buyIn)
        {
            this.playingStyle = playingStyle;
            this.BuyIn = buyIn;
            this.preflopBehavior = new PreflopBehavior(playingStyle);
            this.postflopBehavior = new PostflopBehavior(playingStyle);
        }

        //public Champion(PlayerStyles style, int buyIn)
        //{
        //    this.stats = new Stats();
        //
        //    var playingStyle = new PlayingStyle();
        //
        //    switch (style)
        //    {
        //        case PlayerStyles.TIGHT_AGGRESSIVE:
        //            break;
        //        case PlayerStyles.LOOSE_AGGRESSIVE:
        //            playingStyle.VPIP = new VPIP(this.Name, 100, 24);
        //            playingStyle.PFR = new PFR(this.Name, 100, 18);
        //
        //            var rfi = new Dictionary<Positions, RFI>();
        //            rfi.Add(Positions.UTG, new RFI(1000, 15, 100));
        //            rfi.Add(Positions.MP, new RFI(1000, 17, 100));
        //            rfi.Add(Positions.CO, new RFI(1000, 25, 100));
        //            rfi.Add(Positions.BTN, new RFI(1000, 48, 100));
        //            rfi.Add(Positions.SB, new RFI(1000, 49, 100));
        //            rfi.Add(Positions.BB, new RFI(1000, 0, 100));
        //            playingStyle.RFI = new PositionStorage<RFI>(this.Name, rfi);
        //
        //            playingStyle.ThreeBet = new StreetStorage<ThreeBet>(
        //                new Dictionary<GameRoundType, ThreeBet> { { GameRoundType.PreFlop, new ThreeBet(1000, 61, 1000) } });
        //
        //            playingStyle.FourBet = new StreetStorage<FourBet>(
        //                new Dictionary<GameRoundType, FourBet> { { GameRoundType.PreFlop, new FourBet(1000, 23, 1000) } });
        //
        //            var cbet = new Dictionary<GameRoundType, CBet>(3);
        //            cbet.Add(GameRoundType.Flop, new CBet(this.Name, 100, 60, 100));
        //            cbet.Add(GameRoundType.Turn, new CBet(this.Name, 100, 58, 100));
        //            cbet.Add(GameRoundType.River, new CBet(this.Name, 100, 41, 100));
        //            playingStyle.CBet = new StreetStorage<CBet>(cbet);
        //
        //            var afq = new Dictionary<GameRoundType, AFq>(3);
        //            afq.Add(GameRoundType.Flop, new AFq(100, 100, 198, 0));
        //            afq.Add(GameRoundType.Turn, new AFq(100, 100, 158, 0));
        //            afq.Add(GameRoundType.River, new AFq(100, 100, 300, 0));
        //            playingStyle.AFq = new StreetStorage<AFq>(afq);
        //
        //            break;
        //        default:
        //            throw new ArgumentException("Player style not found");
        //    }
        //
        //    this.playingStyle = playingStyle;
        //    this.BuyIn = buyIn;
        //    this.preflopBehavior = new PreflopBehavior(playingStyle);
        //    this.postflopBehavior = new PostflopBehavior(playingStyle);
        //}

        public override string Name { get; } = "Champion_" + Guid.NewGuid();

        public override int BuyIn { get; }

        public override PlayerAction PostingBlind(IPostingBlindContext context)
        {
            return context.BlindAction;
        }

        public override void StartGame(IStartGameContext context)
        {
            this.stats.Update(context);
        }

        public override void StartHand(IStartHandContext context)
        {
            base.StartHand(context);
            this.pocket = new Pocket(new[] { context.FirstCard, context.SecondCard });
            this.stats.Update(context);
        }

        public override void StartRound(IStartRoundContext context)
        {
            base.StartRound(context);
            this.stats.Update(context);
        }

        public override PlayerAction GetTurn(IGetTurnContext context)
        {
            PlayerAction playerAction;
            this.stats.Update(context, this.Name);

            if (context.RoundType == Logic.GameRoundType.PreFlop)
            {
                playerAction = this.preflopBehavior.OptimalAction(
                    this.pocket,
                    context,
                    (IStats)this.stats,
                    this.CommunityCards);
            }
            else
            {
                playerAction = this.postflopBehavior.OptimalAction(
                    this.pocket,
                    context,
                    (IStats)this.stats,
                    this.CommunityCards);
            }

            this.stats.Update(context, playerAction, this.Name);
            return playerAction;
        }

        public override void EndRound(IEndRoundContext context)
        {
            this.stats.Update(context);
        }

        public override void EndHand(IEndHandContext context)
        {
            this.stats.Update(context, this.Name);
        }

        public override void EndGame(IEndGameContext context)
        {
            this.stats.Update(context);
        }
    }
}
