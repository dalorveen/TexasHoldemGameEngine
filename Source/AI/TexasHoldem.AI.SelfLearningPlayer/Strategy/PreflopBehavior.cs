namespace TexasHoldem.AI.SelfLearningPlayer.Strategy
{
    using System;
    using System.Collections.Generic;

    using TexasHoldem.AI.SelfLearningPlayer.PokerMath;
    using TexasHoldem.Logic.Cards;
    using TexasHoldem.Logic.Players;

    public class PreflopBehavior : BaseBehavior
    {
        private readonly StartingHand startingHand;

        public PreflopBehavior(
            IPocket pocket, IPlayingStyle playingStyle, IGetTurnContext context, IReadOnlyCollection<Card> communityCards)
            : base(pocket, playingStyle, context, communityCards)
        {
            this.startingHand = new StartingHand(pocket, context);
        }

        public override PlayerAction OptimalAction()
        {
            if (this.Stats.PreflopFourBetAndMoreOpportunity)
            {
                return this.ReactionToFourBetAndMoreOpportunity();
            }
            else if (this.Stats.PreflopThreeBetOpportunity)
            {
                return this.ReactionToThreeBetOpportunity();
            }
            else if (this.Stats.OpenRaiseOpportunity)
            {
                return this.ReactionToOpenRaiseOpportunity();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private PlayerAction ReactionToFourBetAndMoreOpportunity()
        {
            if (this.startingHand.IsPlayablePocket(this.PlayingStyle.PreflopThreeBet))
            {
                if (this.Context.AvailablePlayerOptions.Contains(PlayerActionType.Raise))
                {
                    return this.ToRaise(this.Context.MinRaise * 3);
                }
            }
            else if (this.startingHand.IsPlayablePocket(this.PlayingStyle.VPIP, 0.2))
            {
                var playerEconomy = this.PlayerEconomy();
                var investment = (int)playerEconomy.OptimalInvestment(this.Context.CurrentPot);
                if (investment >= this.Context.MoneyToCall)
                {
                    return PlayerAction.CheckOrCall();
                }
            }

            return PlayerAction.Fold();
        }

        private PlayerAction ReactionToThreeBetOpportunity()
        {
            if (this.startingHand.IsPlayablePocket(this.PlayingStyle.PreflopThreeBet))
            {
                if (this.Context.AvailablePlayerOptions.Contains(PlayerActionType.Raise))
                {
                    return this.ToRaise(this.Context.MinRaise * 3);
                }
            }
            else if (this.startingHand.IsPlayablePocket(this.PlayingStyle.VPIP, 0.2))
            {
                return PlayerAction.CheckOrCall();
            }

            return PlayerAction.Fold();
        }

        private PlayerAction ReactionToOpenRaiseOpportunity()
        {
            if (this.startingHand.IsPlayablePocket(this.PlayingStyle.PFR, 0.3))
            {
                if (this.Context.AvailablePlayerOptions.Contains(PlayerActionType.Raise))
                {
                    return this.ToRaise(this.Context.MinRaise * 2);
                }
            }
            else if (this.startingHand.IsPlayablePocket(this.PlayingStyle.VPIP, 0.2))
            {
                return PlayerAction.CheckOrCall();
            }

            return PlayerAction.Fold();
        }

        private PlayerAction ToRaise(int moneyToRaise)
        {
            if (this.IsPush(moneyToRaise))
            {
                return this.RaiseOrAllIn(int.MaxValue);
            }
            else
            {
                return this.RaiseOrAllIn(moneyToRaise + (this.Stats.Callers * this.Context.MoneyToCall));
            }
        }
    }
}
