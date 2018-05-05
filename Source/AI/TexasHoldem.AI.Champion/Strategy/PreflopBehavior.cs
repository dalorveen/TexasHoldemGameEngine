namespace TexasHoldem.AI.Champion.Strategy
{
    using System.Collections.Generic;
    using System.Linq;

    using HandEvaluatorExtension;
    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Cards;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;

    public class PreflopBehavior : BaseBehavior
    {
        public PreflopBehavior(PlayingStyle playingStyle)
            : base(playingStyle)
        {
        }

        public override PlayerAction OptimalAction(
            ICardAdapter pocket, IReadOnlyCollection<Card> communityCards, IGetTurnContext context, Stats stats)
        {
            var startingHand = new StartingHand(pocket);

            if (stats.FourBet().GetStatsBy(GameRoundType.PreFlop).StatsOfCurrentPosition().IsOpportunity)
            {
                // faced with three bet
                return this.ReactionToFourBetOpportunity(communityCards, context, startingHand, stats);
            }
            else if (stats.ThreeBet().GetStatsBy(GameRoundType.PreFlop).StatsOfCurrentPosition().IsOpportunity)
            {
                // faced with raise
                return this.ReactionToThreeBetOpportunity(communityCards, context, startingHand, stats);
            }
            else if (context.PreviousRoundActions.Count(x => x.Action.Type == PlayerActionType.Raise) == 0)
            {
                // faced with no action/limp
                return this.ReactionToOpenRaiseOpportunity(context, startingHand, stats);
            }
            else
            {
                // faced with four bet and more
                return this.ReactionToFourBetOpportunity(communityCards, context, startingHand, stats);
            }
        }

        private PlayerAction ReactionToFourBetOpportunity(
            IReadOnlyCollection<Card> communityCards, IGetTurnContext context, StartingHand startingHand, Stats stats)
        {
            if (this.PlayingStyle.PreflopFourBetDeviation(stats).Amount <=
                this.PlayingStyle.PreflopFourBet.Indicator.Amount
                && this.PlayingStyle.PreflopFourBet.PossibleRange.Contains(startingHand.Pocket.Mask))
            {
                if (context.CanRaise && this.IsBestOrTieHand(context, startingHand))
                {
                    var overWager = this.AddedBetOnPassivePlayers(context);
                    return this.ToRaise((context.MinRaise * 3) + overWager, context);
                }
                else
                {
                    return PlayerAction.CheckOrCall();
                }
            }
            else if (this.PlayingStyle.PreflopThreeBet.PossibleRange.Contains(startingHand.Pocket.Mask))
            {
                if (this.IsBestOrTieHand(context, startingHand))
                {
                    return PlayerAction.CheckOrCall();
                }
            }

            return PlayerAction.Fold();
        }

        private PlayerAction ReactionToThreeBetOpportunity(
            IReadOnlyCollection<Card> communityCards, IGetTurnContext context, StartingHand startingHand, Stats stats)
        {
            if (this.PlayingStyle.PreflopThreeBetDeviation(stats).Amount <=
                this.PlayingStyle.PreflopThreeBet.Indicator.Amount
                && this.PlayingStyle.PreflopThreeBet.PossibleRange.Contains(startingHand.Pocket.Mask))
            {
                if (context.CanRaise)
                {
                    var overWager = this.AddedBetOnPassivePlayers(context);
                    return this.ToRaise((context.MinRaise * 3) + overWager, context);
                }
                else
                {
                    return PlayerAction.CheckOrCall();
                }
            }
            else if (this.PlayingStyle.VPIPDeviation(stats).Amount <= this.PlayingStyle.VPIP.Indicator.Amount
                && this.PlayingStyle.VPIP.PossibleRange.Contains(startingHand.Pocket.Mask))
            {
                return PlayerAction.CheckOrCall();
            }

            return PlayerAction.Fold();
        }

        private PlayerAction ReactionToOpenRaiseOpportunity(
            IGetTurnContext context, StartingHand startingHand, Stats stats)
        {
            var rfi = stats.RFI();
            if (rfi.CurrentPosition != Positions.BB && rfi.StatsOfCurrentPosition().IsOpportunitiesToOpenThePot)
            {
                if (this.PlayingStyle.RFIDeviation(stats).Amount <=
                    this.PlayingStyle.RFI[rfi.CurrentPosition].Indicator.Amount
                    && this.PlayingStyle.RFI[rfi.CurrentPosition].PossibleRange.Contains(startingHand.Pocket.Mask))
                {
                    return this.ToRaise(context.MinRaise * 2, context);
                }
            }
            else if (this.PlayingStyle.PFRDeviation(stats).Amount <= this.PlayingStyle.PFR.Indicator.Amount
                && this.PlayingStyle.PFR.PossibleRange.Contains(startingHand.Pocket.Mask))
            {
                if (context.CanRaise)
                {
                    var overWager = this.AddedBetOnPassivePlayers(context);
                    return this.ToRaise((context.MinRaise * 2) + overWager, context);
                }
            }
            //else if (this.PlayingStyle.VPIPDeviation(stats).Percentage <= this.PlayingStyle.VPIP.Indicator.Percentage
            //    && this.PlayingStyle.VPIP.PossibleRange.Contains(startingHand.Pocket.Mask))
            //{
            //    // the player does not limp
            //}

            return PlayerAction.Fold();
        }

        private int AddedBetOnPassivePlayers(IGetTurnContext context)
        {
            var beforeTheRaiser = context.PreviousRoundActions
                .Reverse()
                .TakeWhile(p => p.Action.Type != PlayerActionType.Raise);
            var callers = beforeTheRaiser.Count(p => p.Action.Type == PlayerActionType.CheckCall);
            return callers * context.MoneyToCall;
        }

        private bool IsBestOrTieHand(IGetTurnContext context, StartingHand startingHand)
        {
            var heroForceIndex = StartingHandStrength.ForceIndexOfStartingHand[startingHand.Pocket.Mask];

            foreach (var item in context.Opponents.Where(p => p.InHand))
            {
                if (heroForceIndex <
                    StartingHandStrength.ForceIndexOfStartingHand[new CardAdapter(item.HoleCards).Mask])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
