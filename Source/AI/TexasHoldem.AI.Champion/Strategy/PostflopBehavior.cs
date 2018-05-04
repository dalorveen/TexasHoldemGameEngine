namespace TexasHoldem.AI.Champion.Strategy
{
    using System.Collections.Generic;

    using HandEvaluatorExtension;
    using TexasHoldem.Logic.Cards;
    using TexasHoldem.Logic.Extensions;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;

    public class PostflopBehavior : BaseBehavior
    {
        private const double LowerWagerLimit = 0.42;

        public PostflopBehavior(PlayingStyle playingStyle)
            : base(playingStyle)
        {
        }

        public override PlayerAction OptimalAction(
            ICardAdapter pocket, IReadOnlyCollection<Card> communityCards, IGetTurnContext context, Stats stats)
        {
            var playerEconomy = this.PlayerEconomy(pocket, communityCards, context);

            if (playerEconomy.NutHand)
            {
                return this.ReactionCausedByNutHand(context, playerEconomy);
            }
            else if (playerEconomy.BestHand)
            {
                return this.ReactionCausedByBestHand(context, playerEconomy);
            }
            else
            {
                return this.ReactionCausedByWeakHand(context, playerEconomy);
            }
        }

        private PlayerAction ReactionCausedByNutHand(IGetTurnContext context, PlayerEconomy playerEconomy)
        {
            //if (context.CanRaise)
            //{
            //    if (this.NeedAnRaiseToAdjustTheStats(context))
            //    {
            //        return this.ToValueBet(context, playerEconomy);
            //    }
            //}

            return PlayerAction.CheckOrCall();
        }

        private PlayerAction ReactionCausedByBestHand(IGetTurnContext context, PlayerEconomy playerEconomy)
        {
            //if (playerEconomy.TiedHandsWithHero > 0)
            //{
            //    if (context.CanRaise && playerEconomy.HandsThatLoseToTheHero.Count > 0)
            //    {
            //        if (this.NeedAnRaiseToAdjustTheStats(context))
            //        {
            //            return this.ToValueBet(context, playerEconomy);
            //        }
            //    }
            //}
            //else
            //{
            //    if (context.CanRaise)
            //    {
            //        if (this.NeedAnRaiseToAdjustTheStats(context))
            //        {
            //            return this.ToValueBet(context, playerEconomy);
            //        }
            //    }
            //}

            return PlayerAction.CheckOrCall();
        }

        private PlayerAction ReactionCausedByWeakHand(IGetTurnContext context, PlayerEconomy playerEconomy)
        {
            //var investment = (int)playerEconomy.OptimalInvestment(context.CurrentPot);
            //
            //if (investment < context.MoneyToCall)
            //{
            //    return PlayerAction.Fold();
            //}
            //
            //if (context.CanRaise)
            //{
            //    if (investment >= context.MoneyToCall + context.MinRaise)
            //    {
            //        if (context.CurrentPot * LowerWagerLimit <= investment && this.NeedAnRaiseToAdjustTheStats(context))
            //        {
            //            if (this.IsPush(investment - context.MoneyToCall, context))
            //            {
            //                // it's a very losing action
            //                // return this.RaiseOrAllIn(int.MaxValue, context);
            //            }
            //            else
            //            {
            //                return this.RaiseOrAllIn(investment - context.MoneyToCall, context);
            //            }
            //        }
            //    }
            //}

            return PlayerAction.CheckOrCall();
        }

        private PlayerAction ToValueBet(IGetTurnContext context, PlayerEconomy playerEconomy)
        {
            double lowerLimit, upperLimit;

            if (playerEconomy.NutHand)
            {
                lowerLimit = LowerWagerLimit;
                upperLimit = 0.75;
            }
            else if (playerEconomy.BestHand)
            {
                lowerLimit = 0.5;
                upperLimit = 1.25;
            }
            else
            {
                lowerLimit = LowerWagerLimit;
                upperLimit = 1.25;
            }

            var min = (int)(context.CurrentPot * lowerLimit) - context.MoneyToCall;
            var max = (int)((context.CurrentPot + 1) * upperLimit) - context.MoneyToCall;
            var difference = max - min;

            min = min >= context.MinRaise ? min : context.MinRaise;
            max = max > min ? max : min + difference;

            var moneyToRaise = RandomProvider.Next(min, max);

            if (this.IsPush(moneyToRaise, context))
            {
                return this.RaiseOrAllIn(int.MaxValue, context);
            }
            else
            {
                return this.RaiseOrAllIn(moneyToRaise, context);
            }
        }

        private bool NeedAnRaiseToAdjustTheStats(IGetTurnContext context)
        {
            //if (context.CurrentStats.CBet.IndicatorByStreets[context.RoundType].IsOpportunity)
            //{
            //    if (context.CurrentStats.CBet.IndicatorByStreets[context.RoundType].Percentage
            //        < this.PlayingStyle.CBet.IndicatorByStreets[context.RoundType].Percentage)
            //    {
            //        return true;
            //    }
            //}
            //
            //if (context.CurrentStats.AFq.IndicatorByStreets[context.RoundType].Percentage
            //    < this.PlayingStyle.AFq.IndicatorByStreets[context.RoundType].Percentage)
            //{
            //    // adjust the current stats of AFq to match the style of the player's game
            //    return true;
            //}

            return false;
        }
    }
}
