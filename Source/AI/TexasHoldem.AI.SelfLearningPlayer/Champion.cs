namespace TexasHoldem.AI.SelfLearningPlayer
{
    using System;

    using TexasHoldem.AI.SelfLearningPlayer.PokerMath;
    using TexasHoldem.AI.SelfLearningPlayer.Strategy;
    using TexasHoldem.Logic.Players;

    public class Champion : BasePlayer
    {
        private readonly IPlayingStyle playingStyle;

        private IPocket pocket;

        public Champion(IPlayingStyle playingStyle)
        {
            this.playingStyle = playingStyle;
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
            this.pocket = new Pocket(new[] { this.FirstCard, this.SecondCard });
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
