namespace TexasHoldem.AI.SelfLearningPlayer
{
    using System;

    using TexasHoldem.AI.SelfLearningPlayer.Helpers;
    using TexasHoldem.AI.SelfLearningPlayer.Strategy;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;

    public class Champion : BasePlayer
    {
        private readonly IStats playingStyle;

        private IPocket pocket;

        private BaseBehavior preflopBehavior;

        private BaseBehavior postflopBehavior;

        public Champion(IStats playingStyle, int buyIn)
        {
            this.playingStyle = playingStyle;
            this.BuyIn = buyIn;
            this.preflopBehavior = new PreflopBehavior(playingStyle);
            this.postflopBehavior = new PostflopBehavior(playingStyle);
        }

        public override string Name { get; } = "Champion_" + Guid.NewGuid();

        public override int BuyIn { get; } = -1;

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
