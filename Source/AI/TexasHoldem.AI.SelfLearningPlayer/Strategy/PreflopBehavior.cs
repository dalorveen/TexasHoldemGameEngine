namespace TexasHoldem.AI.SelfLearningPlayer.Strategy
{
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.AI.SelfLearningPlayer.Helpers;
    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Cards;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;

    public class PreflopBehavior : BaseBehavior
    {
        private int lastFourBetTotalOpportunitiesPF;

        private int lastThreeBetTotalOpportunitiesPF;

        public PreflopBehavior(IStats playingStyle)
            : base(playingStyle)
        {
        }

        public override PlayerAction OptimalAction(
            IPocket pocket, IGetTurnExtendedContext context, IReadOnlyCollection<Card> communityCards)
        {
            var startingHand = new StartingHand(pocket);

            if (this.lastFourBetTotalOpportunitiesPF != context.CurrentStats.FourBet.TotalOpportunities.PF)
            {
                this.lastFourBetTotalOpportunitiesPF = context.CurrentStats.FourBet.TotalOpportunities.PF;
                return this.ReactionToFourBetOpportunity(context, communityCards, startingHand);
            }
            else if (this.lastThreeBetTotalOpportunitiesPF != context.CurrentStats.ThreeBet.TotalOpportunities.PF)
            {
                this.lastThreeBetTotalOpportunitiesPF = context.CurrentStats.ThreeBet.TotalOpportunities.PF;
                return this.ReactionToThreeBetOpportunity(context, startingHand);
            }
            else if (context.PreviousRoundActions.Count(x => x.Action.Type == PlayerActionType.Raise) == 0)
            {
                return this.ReactionToOpenRaiseOpportunity(context, startingHand);
            }
            else
            {
                // faced with four bet and more
                return this.ReactionToFourBetOpportunity(context, communityCards, startingHand);
            }
        }

        private PlayerAction ReactionToFourBetOpportunity(
            IGetTurnExtendedContext context, IReadOnlyCollection<Card> communityCards, StartingHand startingHand)
        {
            if (startingHand.IsPlayablePocket(this.PlayingStyle.FourBet.Percentage(GameRoundType.PreFlop)))
            {
                if (context.AvailablePlayerOptions.Contains(PlayerActionType.Raise))
                {
                    return this.ToRaise(context.MinRaise * 3, context);
                }
            }
            else if (startingHand.IsPlayablePocket(this.PlayingStyle.VPIP.Percentage, 0.2, context))
            {
                var playerEconomy = this.PlayerEconomy(startingHand.Pocket, context, communityCards);
                var investment = (int)playerEconomy.OptimalInvestment(context.CurrentPot);
                if (investment >= context.MoneyToCall)
                {
                    return PlayerAction.CheckOrCall();
                }
            }

            return PlayerAction.Fold();
        }

        private PlayerAction ReactionToThreeBetOpportunity(IGetTurnExtendedContext context, StartingHand startingHand)
        {
            if (startingHand.IsPlayablePocket(this.PlayingStyle.ThreeBet.Percentage(GameRoundType.PreFlop)))
            {
                if (context.AvailablePlayerOptions.Contains(PlayerActionType.Raise))
                {
                    return this.ToRaise(context.MinRaise * 3, context);
                }
            }
            else if (startingHand.IsPlayablePocket(this.PlayingStyle.VPIP.Percentage, 0.2, context))
            {
                return PlayerAction.CheckOrCall();
            }

            return PlayerAction.Fold();
        }

        private PlayerAction ReactionToOpenRaiseOpportunity(IGetTurnExtendedContext context, StartingHand startingHand)
        {
            if (startingHand.IsPlayablePocket(this.PlayingStyle.PFR.Percentage, 0.3, context))
            {
                if (context.AvailablePlayerOptions.Contains(PlayerActionType.Raise))
                {
                    return this.ToRaise(context.MinRaise * 2, context);
                }
            }
            else if (startingHand.IsPlayablePocket(this.PlayingStyle.VPIP.Percentage, 0.2, context))
            {
                return PlayerAction.CheckOrCall();
            }

            return PlayerAction.Fold();
        }

        private PlayerAction ToRaise(int moneyToRaise, IGetTurnExtendedContext context)
        {
            if (this.IsPush(moneyToRaise, context))
            {
                return this.RaiseOrAllIn(int.MaxValue, context);
            }
            else
            {
                var reverse = context.PreviousRoundActions.Reverse();
                var callers = reverse
                    .Count(x => x.Action.Type == PlayerActionType.CheckCall && x.Action.Type != PlayerActionType.Raise);
                return this.RaiseOrAllIn(moneyToRaise + (callers * context.MoneyToCall), context);
            }
        }
    }
}
