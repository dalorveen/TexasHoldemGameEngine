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

        public Champion(PlayerStyles style, int buyIn)
        {
            var playingStyle = new PlayingStyle();

            switch (style)
            {
                case PlayerStyles.TIGHT_AGGRESSIVE:
                    break;
                case PlayerStyles.LOOSE_AGGRESSIVE:
                    playingStyle.VPIP = new VPIP(this.Name, 100, 24);
                    playingStyle.PFR = new PFR(this.Name, 100, 18);

                    var rfi = new Dictionary<SeatNames, RFI>();
                    rfi.Add(SeatNames.UTG, new RFI(1000, 15, 100));
                    rfi.Add(SeatNames.MP, new RFI(1000, 17, 100));
                    rfi.Add(SeatNames.CO, new RFI(1000, 25, 100));
                    rfi.Add(SeatNames.BTN, new RFI(1000, 48, 100));
                    rfi.Add(SeatNames.SB, new RFI(1000, 49, 100));
                    rfi.Add(SeatNames.BB, new RFI(1000, 0, 100));
                    playingStyle.RFI = new PositionStorage<RFI>(this.Name, rfi);

                    playingStyle.ThreeBet = new StreetStorage<ThreeBet>(
                        new Dictionary<GameRoundType, ThreeBet> { { GameRoundType.PreFlop, new ThreeBet(1000, 61, 1000) } });

                    playingStyle.FourBet = new StreetStorage<FourBet>(
                        new Dictionary<GameRoundType, FourBet> { { GameRoundType.PreFlop, new FourBet(1000, 23, 1000) } });

                    var cbet = new Dictionary<GameRoundType, CBet>(3);
                    cbet.Add(GameRoundType.Flop, new CBet(this.Name, 100, 60, 100));
                    cbet.Add(GameRoundType.Turn, new CBet(this.Name, 100, 58, 100));
                    cbet.Add(GameRoundType.River, new CBet(this.Name, 100, 41, 100));
                    playingStyle.CBet = new StreetStorage<CBet>(cbet);

                    var afq = new Dictionary<GameRoundType, AFq>(3);
                    afq.Add(GameRoundType.Flop, new AFq(100, 100, 198, 0));
                    afq.Add(GameRoundType.Turn, new AFq(100, 100, 158, 0));
                    afq.Add(GameRoundType.River, new AFq(100, 100, 300, 0));
                    playingStyle.AFq = new StreetStorage<AFq>(afq);

                    break;
                default:
                    throw new ArgumentException("Player style not found");
            }

            this.playingStyle = playingStyle;
            this.BuyIn = buyIn;
            this.preflopBehavior = new PreflopBehavior(playingStyle);
            this.postflopBehavior = new PostflopBehavior(playingStyle);
        }

        public override string Name { get; } = "Champion_" + Guid.NewGuid();

        public override int BuyIn { get; }

        public override PlayerAction PostingBlind(IPostingBlindContext context)
        {
            return context.BlindAction;
        }

        public override void StartHand(IStartHandContext context)
        {
            base.StartHand(context);
            this.pocket = new Pocket(new[] { context.FirstCard, context.SecondCard });
        }

        public override PlayerAction GetTurn(IGetTurnContext context)
        {
            if (context is IGetTurnExtendedContext)
            {
                if (context.RoundType == Logic.GameRoundType.PreFlop)
                {
                    return this.preflopBehavior.OptimalAction(this.pocket, (IGetTurnExtendedContext)context, this.CommunityCards);
                }
                else
                {
                    return this.postflopBehavior.OptimalAction(this.pocket, (IGetTurnExtendedContext)context, this.CommunityCards);
                }
            }
            else
            {
                throw new ArgumentException("Context should have an extended interface", nameof(context));
            }
        }
    }
}
