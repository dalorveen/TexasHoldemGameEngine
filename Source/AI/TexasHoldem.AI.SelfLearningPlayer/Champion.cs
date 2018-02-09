namespace TexasHoldem.AI.SelfLearningPlayer
{
    using System;

    using TexasHoldem.AI.SelfLearningPlayer.PokerMath;
    using TexasHoldem.AI.SelfLearningPlayer.Strategy;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;

    public class Champion : BasePlayer
    {
        private readonly IStats playingStyle;

        private IPocket pocket;

        public Champion(IStats playingStyle, int buyIn)
        {
            this.playingStyle = playingStyle;
            this.BuyIn = buyIn;
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
            if (context.RoundType == Logic.GameRoundType.PreFlop)
            {
                var preflop = new PreflopBehavior(this.pocket, this.playingStyle, context, this.CommunityCards);
                return preflop.OptimalAction();
            }
            else
            {
                var postflop = new PostflopBehavior(this.pocket, this.playingStyle, context, this.CommunityCards);
                return postflop.OptimalAction();
            }
        }
    }
}
