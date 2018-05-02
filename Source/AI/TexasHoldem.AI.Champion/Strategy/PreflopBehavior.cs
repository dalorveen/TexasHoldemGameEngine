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
        private VPIPCorrection vpipCorrection;

        private PFRCorrection pfrCorrection;

        private RFICorrection rfiCorrection;

        private ThreeBetCorrection threeBetCorrection;

        private FourBetCorrection fourBetCorrection;

        public PreflopBehavior(IStats playingStyle)
            : base(playingStyle)
        {
            var numberOfHandsToStartCorrection = 30;
            this.vpipCorrection = new VPIPCorrection(playingStyle, numberOfHandsToStartCorrection);
            this.pfrCorrection = new PFRCorrection(playingStyle, numberOfHandsToStartCorrection);
            this.rfiCorrection = new RFICorrection(playingStyle, numberOfHandsToStartCorrection);
            this.threeBetCorrection = new ThreeBetCorrection(playingStyle, numberOfHandsToStartCorrection);
            this.fourBetCorrection = new FourBetCorrection(playingStyle, numberOfHandsToStartCorrection);
        }

        public override PlayerAction OptimalAction(
            ICardAdapter pocket, IGetTurnContext context, IStats stats, IReadOnlyCollection<Card> communityCards)
        {
            //var startingHand = new StartingHand(pocket);
            //
            //if (stats.FourBet().CurrentPositionIndicatorBy(GameRoundType.PreFlop).IsOpportunity)
            //{
            //    return this.ReactionToFourBetOpportunity(context, communityCards, startingHand);
            //}
            //else if (stats.ThreeBet().CurrentPositionIndicatorBy(GameRoundType.PreFlop).IsOpportunity)
            //{
            //    return this.ReactionToThreeBetOpportunity(context, communityCards, startingHand);
            //}
            //else if (context.PreviousRoundActions.Count(x => x.Action.Type == PlayerActionType.Raise) == 0)
            //{
            //    return this.ReactionToOpenRaiseOpportunity(context, startingHand);
            //}
            //else
            //{
            //    // faced with four bet and more
            //    return this.ReactionToFourBetOpportunity(context, communityCards, startingHand);
            //}
            return null; // remove it
        }

        private PlayerAction ReactionToFourBetOpportunity(
            IGetTurnContext context, IReadOnlyCollection<Card> communityCards, StartingHand startingHand)
        {
            //if (startingHand.IsPlayablePocket(
            //    this.PlayingStyle.FourBet.IndicatorByStreets[GameRoundType.PreFlop].Percentage
            //        * this.fourBetCorrection.CorrectionFactor(context.CurrentStats, context.RoundType)))
            //{
            //    if (context.CanRaise)
            //    {
            //        if ((double)context.MoneyToCall / (double)context.CurrentPot >= 0.6)
            //        {
            //            // opponent bet too much
            //            // return this.ReactionToAHugeBetFromTheOpponent(context, communityCards, startingHand); Too expensive method.
            //
            //            var overWager = this.OverWager(context);
            //            return this.ToRaise((context.MinRaise * 3) + overWager, context);
            //        }
            //        else
            //        {
            //            var overWager = this.OverWager(context);
            //            return this.ToRaise((context.MinRaise * 3) + overWager, context);
            //        }
            //    }
            //    else
            //    {
            //        return PlayerAction.CheckOrCall();
            //    }
            //}
            //else if (startingHand.IsPlayablePocket(this.PlayingStyle.ThreeBet.IndicatorByStreets[GameRoundType.PreFlop].Percentage))
            //{
            //    if ((double)context.MoneyToCall / (double)context.CurrentPot >= 0.6)
            //    {
            //        //var reaction = this.ReactionToAHugeBetFromTheOpponent(context, communityCards, startingHand);
            //        //if (reaction.Type != PlayerActionType.Fold)
            //        //{
            //        //    return PlayerAction.CheckOrCall();
            //        //}
            //
            //        return PlayerAction.CheckOrCall();
            //    }
            //    else
            //    {
            //        return PlayerAction.CheckOrCall();
            //    }
            //}

            return PlayerAction.Fold();
        }

        private PlayerAction ReactionToThreeBetOpportunity(
            IGetTurnContext context, IReadOnlyCollection<Card> communityCards, StartingHand startingHand)
        {
            //if (startingHand.IsPlayablePocket(
            //    this.PlayingStyle.ThreeBet.IndicatorByStreets[GameRoundType.PreFlop].Percentage
            //        * this.threeBetCorrection.CorrectionFactor(context.CurrentStats, context.RoundType)))
            //{
            //    if (context.CanRaise)
            //    {
            //        if ((double)context.MoneyToCall / (double)context.CurrentPot >= 0.7)
            //        {
            //            // opponent bet too much
            //            // return this.ReactionToAHugeBetFromTheOpponent(context, communityCards, startingHand); Too expensive method.
            //
            //            var overWager = this.OverWager(context);
            //            return this.ToRaise((context.MinRaise * 3) + overWager, context);
            //        }
            //        else
            //        {
            //            var overWager = this.OverWager(context);
            //            return this.ToRaise((context.MinRaise * 3) + overWager, context);
            //        }
            //    }
            //    else
            //    {
            //        return PlayerAction.CheckOrCall();
            //    }
            //}
            //else if (startingHand.IsPlayablePocket(
            //    this.PlayingStyle.VPIP.Percentage * this.vpipCorrection.CorrectionFactor(context.CurrentStats, context.RoundType)))
            //{
            //    if ((double)context.MoneyToCall / (double)context.CurrentPot <= 0.625)
            //    {
            //        return PlayerAction.CheckOrCall();
            //    }
            //}

            return PlayerAction.Fold();
        }

        private PlayerAction ReactionToOpenRaiseOpportunity(
            IGetTurnContext context, IStats stats, StartingHand startingHand)
        {
            //if (stats.RFI().CurrentPosition().IsOpportunitiesToOpenThePot)
            //{
            //    if (startingHand.IsPlayablePocket(
            //        this.PlayingStyle.RFI.IndicatorByPositions[currentPosition.Value].Percentage
            //            * this.rfiCorrection.CorrectionFactor(context.CurrentStats, context.RoundType)))
            //    {
            //        return this.ToRaise(context.MinRaise * 2, context);
            //    }
            //}
            //else if (startingHand.IsPlayablePocket(
            //    this.PlayingStyle.PFR.Percentage * this.pfrCorrection.CorrectionFactor(context.CurrentStats, context.RoundType)))
            //{
            //    if (context.CanRaise)
            //    {
            //        var overWager = this.OverWager(context);
            //        return this.ToRaise((context.MinRaise * 2) + overWager, context);
            //    }
            //}
            //else if (startingHand.IsPlayablePocket(
            //    this.PlayingStyle.VPIP.Percentage * this.vpipCorrection.CorrectionFactor(context.CurrentStats, context.RoundType)))
            //{
            //    return PlayerAction.CheckOrCall();
            //}

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
