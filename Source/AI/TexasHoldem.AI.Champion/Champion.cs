namespace TexasHoldem.AI.Champion
{
    using System;

    using HandEvaluatorExtension;
    using TexasHoldem.AI.Champion.Strategy;
    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;

    public class Champion : BasePlayer
    {
        private readonly Stats stats;

        private ICardAdapter pocket;

        private BaseBehavior preflopBehavior;

        private BaseBehavior postflopBehavior;

        public Champion(PlayingStyle playingStyle, int buyIn)
        {
            this.stats = new Stats(this.Name);
            this.preflopBehavior = new PreflopBehavior(playingStyle);
            this.postflopBehavior = new PostflopBehavior(playingStyle);
            this.BuyIn = buyIn;
        }

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
            this.stats.Update(context);

            if (context.RoundType == GameRoundType.PreFlop)
            {
                playerAction = this.preflopBehavior.OptimalAction(
                    this.pocket,
                    this.CommunityCards,
                    context,
                    this.stats);
            }
            else
            {
                playerAction = this.postflopBehavior.OptimalAction(
                    this.pocket,
                    this.CommunityCards,
                    context,
                    this.stats);
            }

            this.stats.Update(context, playerAction);
            return playerAction;
        }

        public override void EndRound(IEndRoundContext context)
        {
            this.stats.Update(context);
        }

        public override void EndHand(IEndHandContext context)
        {
            this.stats.Update(context);
        }

        public override void EndGame(IEndGameContext context)
        {
            this.stats.Update(context);
        }
    }
}
