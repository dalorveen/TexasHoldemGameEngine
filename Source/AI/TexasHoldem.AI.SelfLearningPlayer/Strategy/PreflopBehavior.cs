﻿namespace TexasHoldem.AI.Champion.Strategy
{
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.AI.Champion.Helpers;
    using TexasHoldem.AI.Champion.StatsCorrection;
    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Cards;
    using TexasHoldem.Logic.Extensions;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;

    public class PreflopBehavior : BaseBehavior
    {
        private VPIPCorrection vpipCorrection;

        private PFRCorrection pfrCorrection;

        private RFICorrection rfiCorrection;

        private ThreeBetCorrection threeBetCorrection;

        private FourBetCorrection fourBetCorrection;

        public PreflopBehavior(IStats playingStyle)
            : base(playingStyle)
        {
            var numberOfHandsToStartCorrection = 20;
            this.vpipCorrection = new VPIPCorrection(playingStyle, numberOfHandsToStartCorrection);
            this.pfrCorrection = new PFRCorrection(playingStyle, numberOfHandsToStartCorrection);
            this.rfiCorrection = new RFICorrection(playingStyle, numberOfHandsToStartCorrection);
            this.threeBetCorrection = new ThreeBetCorrection(playingStyle, numberOfHandsToStartCorrection);
            this.fourBetCorrection = new FourBetCorrection(playingStyle, numberOfHandsToStartCorrection);
        }

        public override PlayerAction OptimalAction(
            IPocket pocket, IGetTurnExtendedContext context, IReadOnlyCollection<Card> communityCards)
        {
            var startingHand = new StartingHand(pocket);

            if (context.CurrentStats.FourBet.IndicatorByStreets[context.RoundType].IsOpportunity)
            {
                return this.ReactionToFourBetOpportunity(context, communityCards, startingHand);
            }
            else if (context.CurrentStats.ThreeBet.IndicatorByStreets[context.RoundType].IsOpportunity)
            {
                return this.ReactionToThreeBetOpportunity(context, communityCards, startingHand);
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
            if (startingHand.IsPlayablePocket(
                this.PlayingStyle.FourBet.IndicatorByStreets[GameRoundType.PreFlop].Percentage
                    * this.fourBetCorrection.CorrectionFactor(context.CurrentStats, context.RoundType)))
            {
                if (context.AvailablePlayerOptions.Contains(PlayerActionType.Raise))
                {
                    if ((double)context.MoneyToCall / (double)context.CurrentPot >= 0.70)
                    {
                        // opponent bet too much
                        return this.ReactionToAHugeBetFromTheOpponent(context, communityCards, startingHand);
                    }
                    else
                    {
                        var ec = this.ExtraCharge(context);
                        return this.ToRaise((context.MinRaise * 3) + ec, context);
                    }
                }
                else
                {
                    return PlayerAction.CheckOrCall();
                }
            }
            else if (startingHand.IsPlayablePocket(this.PlayingStyle.ThreeBet.IndicatorByStreets[GameRoundType.PreFlop].Percentage))
            {
                if ((double)context.MoneyToCall / (double)context.CurrentPot >= 0.6)
                {
                    var reaction = this.ReactionToAHugeBetFromTheOpponent(context, communityCards, startingHand);
                    if (reaction.Type != PlayerActionType.Fold)
                    {
                        return PlayerAction.CheckOrCall();
                    }
                }
                else
                {
                    return PlayerAction.CheckOrCall();
                }
            }

            return PlayerAction.Fold();
        }

        private PlayerAction ReactionToThreeBetOpportunity(
            IGetTurnExtendedContext context, IReadOnlyCollection<Card> communityCards, StartingHand startingHand)
        {
            if (startingHand.IsPlayablePocket(
                this.PlayingStyle.ThreeBet.IndicatorByStreets[GameRoundType.PreFlop].Percentage
                    * this.threeBetCorrection.CorrectionFactor(context.CurrentStats, context.RoundType)))
            {
                if (context.AvailablePlayerOptions.Contains(PlayerActionType.Raise))
                {
                    if ((double)context.MoneyToCall / (double)context.CurrentPot >= 0.72)
                    {
                        // opponent bet too much
                        return this.ReactionToAHugeBetFromTheOpponent(context, communityCards, startingHand);
                    }
                    else
                    {
                        var ec = this.ExtraCharge(context);
                        return this.ToRaise((context.MinRaise * 3) + ec, context);
                    }
                }
                else
                {
                    return PlayerAction.CheckOrCall();
                }
            }
            else if (startingHand.IsPlayablePocket(
                this.PlayingStyle.VPIP.Percentage * this.vpipCorrection.CorrectionFactor(context.CurrentStats, context.RoundType)/*,
                0.2,
                context*/))
            {
                if ((double)context.MoneyToCall / (double)context.CurrentPot <= 0.67)
                {
                    return PlayerAction.CheckOrCall();
                }
            }

            return PlayerAction.Fold();
        }

        private PlayerAction ReactionToOpenRaiseOpportunity(IGetTurnExtendedContext context, StartingHand startingHand)
        {
            var currentPosition = context.CurrentStats.RFI.CurrentPosition;

            if (context.CurrentStats.RFI.IndicatorByPositions[currentPosition.Value].IsOpportunitiesToOpenThePot)
            {
                if (startingHand.IsPlayablePocket(
                    this.PlayingStyle.RFI.IndicatorByPositions[currentPosition.Value].Percentage
                        * this.rfiCorrection.CorrectionFactor(context.CurrentStats, context.RoundType)))
                {
                    return this.ToRaise(context.MinRaise * 2, context);
                }
            }
            else if (startingHand.IsPlayablePocket(
                this.PlayingStyle.PFR.Percentage * this.pfrCorrection.CorrectionFactor(context.CurrentStats, context.RoundType)/*,
                0.3,
                context*/))
            {
                if (context.AvailablePlayerOptions.Contains(PlayerActionType.Raise))
                {
                    var ec = this.ExtraCharge(context);
                    return this.ToRaise((context.MinRaise * 2) + ec, context);
                }
            }
            else if (startingHand.IsPlayablePocket(
                this.PlayingStyle.VPIP.Percentage * this.vpipCorrection.CorrectionFactor(context.CurrentStats, context.RoundType)/*,
                0.2,
                context*/))
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
                return this.RaiseOrAllIn(moneyToRaise, context);
            }
        }

        private int ExtraCharge(IGetTurnExtendedContext context)
        {
            var beforeTheRaiser = context.PreviousRoundActions.Reverse().TakeWhile(p => p.Action.Type != PlayerActionType.Raise);
            var callers = beforeTheRaiser.Count(p => p.Action.Type == PlayerActionType.CheckCall);
            return callers * context.MoneyToCall;
        }

        private PlayerAction ReactionToAHugeBetFromTheOpponent(
            IGetTurnExtendedContext context, IReadOnlyCollection<Card> communityCards, StartingHand startingHand)
        {
            var playerEconomy = this.PlayerEconomy(startingHand.Pocket, context, communityCards);
            var investment = (int)playerEconomy.NeutralEVInvestment(context.CurrentPot);

            if (playerEconomy.BestHand)
            {
                if (context.Opponents.Where(p => p.InHand).Count() > 1)
                {
                    var ec = this.ExtraCharge(context);
                    return this.ToRaise((context.MinRaise * 3) + ec, context);
                }
                else
                {
                    return PlayerAction.CheckOrCall();
                }
            }
            else if (investment >= context.MoneyToCall)
            {
                if (context.AvailablePlayerOptions.Contains(PlayerActionType.Raise))
                {
                    if (investment >= context.MoneyToCall + context.MinRaise)
                    {
                        return this.ToRaise(investment - context.MoneyToCall, context);
                    }
                    else
                    {
                        return PlayerAction.CheckOrCall();
                    }
                }
            }
            else if (startingHand.IsPremiumHand)
            {
                return PlayerAction.CheckOrCall();
            }

            return PlayerAction.Fold();
        }
    }
}
