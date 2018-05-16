namespace TexasHoldem.AI.Champion.Strategy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using HandEvaluatorExtension;
    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Cards;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;

    public class PreflopBehavior : BaseBehavior
    {
        private static object locker = new object();

        public PreflopBehavior(PlayingStyle playingStyle)
            : base(playingStyle)
        {
        }

        public override PlayerAction OptimalAction(
            ICardAdapter pocket, IReadOnlyCollection<Card> communityCards, IGetTurnContext context, Stats stats)
        {
            var startingHand = new StartingHand(pocket);
            Func<IReadOnlyCollection<Card>, IGetTurnContext, StartingHand, Stats, PlayerAction> handler;

            if (stats.FourBet().IsOpportunity)
            {
                // faced with three bet
                handler = this.ReactionToFourBetOpportunity;
            }
            else if (stats.ThreeBet().IsOpportunity)
            {
                // faced with raise
                handler = this.ReactionToThreeBetOpportunity;
            }
            else if (context.PreviousRoundActions.Count(x => x.Action.Type == PlayerActionType.Raise) == 0)
            {
                // faced with no action/limp
                handler = this.ReactionToOpenRaiseOpportunity;
            }
            else
            {
                // faced with four bet and more
                handler = this.ReactionToFiveBetAndMoreOpportunity;
            }

            var potentialAmountOfMoneyWon = context.MyMoneyInTheRound;
            foreach (var item in context.Opponents.Select(s => s.CurrentRoundBet))
            {
                if (item > context.MoneyLeft + context.MyMoneyInTheRound)
                {
                    potentialAmountOfMoneyWon += context.MoneyLeft + context.MyMoneyInTheRound;
                }
                else
                {
                    potentialAmountOfMoneyWon += item;
                }
            }

            if ((double)context.MoneyToCall / (double)potentialAmountOfMoneyWon > 0.7)
            {
                // a huge bet from the opponent
                if (this.IsBestOrTieHand(context, startingHand))
                {
                    return handler(communityCards, context, startingHand, stats);
                }
                else
                {
                    return PlayerAction.Fold();
                }
            }

            return handler(communityCards, context, startingHand, stats);
        }

        private PlayerAction ReactionToFiveBetAndMoreOpportunity(
            IReadOnlyCollection<Card> communityCards, IGetTurnContext context, StartingHand startingHand, Stats stats)
        {
            if (startingHand.IsPremiumHand)
            {
                if (context.CanRaise)
                {
                    var overWager = this.AddedBetOnPassivePlayers(context);
                    var moneyToRaise = (context.MinRaise * (2.2 + this.CoefficientOfForceSklansky(startingHand)))
                        + overWager;
                    return this.RaiseOrPush((int)moneyToRaise, context);
                }
                else
                {
                    return PlayerAction.CheckOrCall();
                }
            }
            else if (stats.FoldFourBet().Faced4Bet)
            {
                if (this.PlayingStyle.PreflopFoldFourBetDeviation(stats) > 0
                    && this.PlayingStyle.PFR.PlayableRange.Contains(startingHand.Pocket.Mask))
                {
                    if (this.IsEnoughMoneyLeftAfterInvestment(context.MoneyToCall, context))
                    {
                        return PlayerAction.CheckOrCall();
                    }
                }
            }

            return PlayerAction.Fold();
        }

        private PlayerAction ReactionToFourBetOpportunity(
            IReadOnlyCollection<Card> communityCards, IGetTurnContext context, StartingHand startingHand, Stats stats)
        {
            if ((startingHand.IsPremiumHand
                || this.PlayingStyle.PreflopFourBetDeviation(stats) < 0
                || this.PlayingStyle.PreflopFoldThreeBetDeviation(stats) > 0)
                && this.PlayingStyle.PreflopFourBet.PlayableRange.Contains(startingHand.Pocket.Mask))
            {
                if (context.CanRaise)
                {
                    var overWager = this.AddedBetOnPassivePlayers(context);
                    var moneyToRaise = (context.MinRaise * (2.5 + this.CoefficientOfForceSklansky(startingHand)))
                        + overWager;
                    return this.RaiseOrPush((int)moneyToRaise, context);
                }
                else
                {
                    return PlayerAction.CheckOrCall();
                }
            }
            else if (this.PlayingStyle.PreflopCallThreeBetDeviation(stats) < 0
                && this.PlayingStyle.PreflopCallThreeBet.PlayableRange.Contains(startingHand.Pocket.Mask))
            {
                if (this.IsEnoughMoneyLeftAfterInvestment(context.MoneyToCall, context))
                {
                    return PlayerAction.CheckOrCall();
                }
            }

            return PlayerAction.Fold();
        }

        private PlayerAction ReactionToThreeBetOpportunity(
            IReadOnlyCollection<Card> communityCards, IGetTurnContext context, StartingHand startingHand, Stats stats)
        {
            if ((startingHand.IsPremiumHand || this.PlayingStyle.PreflopThreeBetDeviation(stats) < 0)
                && this.PlayingStyle.PreflopThreeBet.PlayableRange.Contains(startingHand.Pocket.Mask))
            {
                if (context.CanRaise)
                {
                    var overWager = this.AddedBetOnPassivePlayers(context);
                    var moneyToRaise = (context.MinRaise * (3 + this.CoefficientOfForceSklansky(startingHand)))
                        + overWager;
                    return this.RaiseOrPush((int)moneyToRaise, context);
                }
                else
                {
                    return PlayerAction.CheckOrCall();
                }
            }
            else if (this.PlayingStyle.VPIPDeviation(stats) < 0
                && this.PlayingStyle.VPIP.PlayableRange.Contains(startingHand.Pocket.Mask))
            {
                if (this.IsEnoughMoneyLeftAfterInvestment(context.MoneyToCall, context))
                {
                    return PlayerAction.CheckOrCall();
                }
            }

            return PlayerAction.Fold();
        }

        private PlayerAction ReactionToOpenRaiseOpportunity(
            IReadOnlyCollection<Card> communityCards, IGetTurnContext context, StartingHand startingHand, Stats stats)
        {
            if (stats.Position.CurrentPosition != Positions.BB && stats.RFI().IsOpportunitiesToOpenThePot)
            {
                if ((startingHand.IsPremiumHand || this.PlayingStyle.RFIDeviation(stats) < 0)
                    && this.PlayingStyle.RFI[stats.Position.CurrentPosition].PlayableRange
                        .Contains(startingHand.Pocket.Mask))
                {
                    return this.RaiseOrPush(context.MinRaise * 2, context);
                }
            }
            else if ((startingHand.IsPremiumHand || this.PlayingStyle.PFRDeviation(stats) < 0)
                && this.PlayingStyle.PFR.PlayableRange.Contains(startingHand.Pocket.Mask))
            {
                if (context.CanRaise)
                {
                    var overWager = this.AddedBetOnPassivePlayers(context);
                    return this.RaiseOrPush((context.MinRaise * 2) + overWager, context);
                }
                else
                {
                    return PlayerAction.CheckOrCall();
                }
            }

            if (context.CanCheck)
            {
                return PlayerAction.CheckOrCall();
            }

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

        private double CoefficientOfForceSklansky(StartingHand startingHand)
        {
            lock (locker)
            {
                var groupIndex = (int)HoldemHand.PocketHands.GroupType(startingHand.Pocket.Mask);
                var fracture = 3;

                if (groupIndex <= fracture)
                {
                    return 1.0 - (groupIndex / fracture);
                }
                else
                {
                    return -((groupIndex - (fracture + 1)) / (8.0 - (fracture + 1)));
                }
            }
        }
    }
}
