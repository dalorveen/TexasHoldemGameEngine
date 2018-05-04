namespace TexasHoldem.AI.Champion.Strategy
{
    using System.Collections.Generic;
    using System.Linq;

    using HandEvaluatorExtension;
    using TexasHoldem.AI.Champion.StatsCorrection;
    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Cards;
    using TexasHoldem.Logic.Extensions;
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
            if (this.PlayingStyle.PreflopFourBetDeviation(stats).Percentage <= this.PlayingStyle.PreflopFourBet.Percentage
                && startingHand.IsPlayablePocket(this.PlayingStyle.PreflopFourBet.Percentage))
            {
                if (context.CanRaise)
                {
                    if ((double)context.MoneyToCall / (double)context.CurrentPot >= 0.6)
                    {
                        // opponent bet too much
                        // return this.ReactionToAHugeBetFromTheOpponent(context, communityCards, startingHand); Too expensive method.

                        var overWager = this.OverWager(context);
                        return this.ToRaise((context.MinRaise * 3) + overWager, context);
                    }
                    else
                    {
                        var overWager = this.OverWager(context);
                        return this.ToRaise((context.MinRaise * 3) + overWager, context);
                    }
                }
                else
                {
                    return PlayerAction.CheckOrCall();
                }
            }
            else if (startingHand.IsPlayablePocket(this.PlayingStyle.PreflopThreeBet.Percentage))
            {
                if ((double)context.MoneyToCall / (double)context.CurrentPot >= 0.6)
                {
                    //var reaction = this.ReactionToAHugeBetFromTheOpponent(context, communityCards, startingHand);
                    //if (reaction.Type != PlayerActionType.Fold)
                    //{
                    //    return PlayerAction.CheckOrCall();
                    //}

                    return PlayerAction.CheckOrCall();
                }
                else
                {
                    return PlayerAction.CheckOrCall();
                }
            }

            return PlayerAction.Fold();
        }

        private PlayerAction ReactionToThreeBetOpportunity(
            IReadOnlyCollection<Card> communityCards, IGetTurnContext context, StartingHand startingHand, Stats stats)
        {
            if (this.PlayingStyle.PreflopThreeBetDeviation(stats).Percentage <= this.PlayingStyle.PreflopThreeBet.Percentage
                && startingHand.IsPlayablePocket(this.PlayingStyle.PreflopThreeBet.Percentage))
            {
                if (context.CanRaise)
                {
                    if ((double)context.MoneyToCall / (double)context.CurrentPot >= 0.7)
                    {
                        // opponent bet too much
                        // return this.ReactionToAHugeBetFromTheOpponent(context, communityCards, startingHand); Too expensive method.

                        var overWager = this.OverWager(context);
                        return this.ToRaise((context.MinRaise * 3) + overWager, context);
                    }
                    else
                    {
                        var overWager = this.OverWager(context);
                        return this.ToRaise((context.MinRaise * 3) + overWager, context);
                    }
                }
                else
                {
                    return PlayerAction.CheckOrCall();
                }
            }
            else if (this.PlayingStyle.VPIPDeviation(stats).Percentage <= this.PlayingStyle.VPIP.Percentage)
            {
                if ((double)context.MoneyToCall / (double)context.CurrentPot <= 0.625)
                {
                    return PlayerAction.CheckOrCall();
                }
            }

            return PlayerAction.Fold();
        }

        private PlayerAction ReactionToOpenRaiseOpportunity(
            IGetTurnContext context, StartingHand startingHand, Stats stats)
        {
            var rfi = stats.RFI();
            if (rfi.StatsOfCurrentPosition() != null && rfi.StatsOfCurrentPosition().IsOpportunitiesToOpenThePot)
            {
                if (this.PlayingStyle.RFIDeviation(stats).Percentage <= this.PlayingStyle.RFI[rfi.CurrentPosition].Percentage
                    && startingHand.IsPlayablePocket(this.PlayingStyle.RFI[rfi.CurrentPosition].Percentage))
                {
                    return this.ToRaise(context.MinRaise * 2, context);
                }
            }
            else if (this.PlayingStyle.PFRDeviation(stats).Percentage <= this.PlayingStyle.PFR.Percentage
                && startingHand.IsPlayablePocket(this.PlayingStyle.PFR.Percentage))
            {
                if (context.CanRaise)
                {
                    var overWager = this.OverWager(context);
                    return this.ToRaise((context.MinRaise * 2) + overWager, context);
                }
            }
            else if (this.PlayingStyle.VPIPDeviation(stats).Percentage <= this.PlayingStyle.VPIP.Percentage
                && startingHand.IsPlayablePocket(this.PlayingStyle.VPIP.Percentage))
            {
                return PlayerAction.CheckOrCall();
            }

            return PlayerAction.Fold();
        }

        private PlayerAction ToRaise(int moneyToRaise, IGetTurnContext context)
        {
            if (this.IsPush(moneyToRaise, context))
            {
                return this.RaiseOrAllIn(int.MaxValue, context);
            }
            else
            {
                return this.RaiseOrAllIn(moneyToRaise, context);
            }
        }

        private int OverWager(IGetTurnContext context)
        {
            var beforeTheRaiser = context.PreviousRoundActions
                .Reverse()
                .TakeWhile(p => p.Action.Type != PlayerActionType.Raise);
            var callers = beforeTheRaiser.Count(p => p.Action.Type == PlayerActionType.CheckCall);
            return callers * context.MoneyToCall;
        }

        private PlayerAction ReactionToAHugeBetFromTheOpponent(
            IGetTurnContext context, IReadOnlyCollection<Card> communityCards, StartingHand startingHand)
        {
            //var playerEconomy = this.PlayerEconomy(startingHand.Pocket, context, communityCards);
            //var investment = (int)playerEconomy.NeutralEVInvestment(context.CurrentPot);
            //
            //if (playerEconomy.BestHand)
            //{
            //    if (!this.IsInPosition(context) || context.Opponents.Where(p => p.InHand).Count() > 1)
            //    {
            //        var overWager = this.OverWager(context);
            //        return this.ToRaise((context.MinRaise * 3) + overWager, context);
            //    }
            //    else
            //    {
            //        return PlayerAction.CheckOrCall();
            //    }
            //}
            //else if (investment >= context.MoneyToCall)
            //{
            //    if (context.CanRaise)
            //    {
            //        if (investment >= context.MoneyToCall + context.MinRaise)
            //        {
            //            return this.ToRaise(investment - context.MoneyToCall, context);
            //        }
            //    }
            //
            //    return PlayerAction.CheckOrCall();
            //}
            //else if (startingHand.IsPremiumHand)
            //{
            //    return PlayerAction.CheckOrCall();
            //}

            return PlayerAction.Fold();
        }
    }
}
