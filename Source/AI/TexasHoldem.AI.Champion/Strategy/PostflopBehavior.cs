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
                return this.ReactionCausedByNutHand(context, playerEconomy, stats);
            }
            else if (playerEconomy.BestHand)
            {
                return this.ReactionCausedByBestHand(context, playerEconomy, stats);
            }
            else
            {
                return this.ReactionCausedByWeakHand(context, playerEconomy, stats);
            }
        }

        private PlayerAction ReactionCausedByNutHand(IGetTurnContext context, PlayerEconomy playerEconomy, Stats stats)
        {
            if (context.CanRaise && this.NeedRaiseToMatchThePlaystyle(context, stats))
            {
                return this.ToRaise(this.RandomBet(context, 0.4, 1.25), context);
            }

            return PlayerAction.CheckOrCall();
        }

        private PlayerAction ReactionCausedByBestHand(
            IGetTurnContext context, PlayerEconomy playerEconomy, Stats stats)
        {
            if (playerEconomy.TiedHandsWithHero > 0)
            {
                if (context.CanRaise
                    && playerEconomy.HandsThatLoseToTheHero.Count > 0
                    && this.NeedRaiseToMatchThePlaystyle(context, stats))
                {
                    return this.ToRaise(this.RandomBet(context, 0.4, 0.6), context);
                }
            }
            else
            {
                if (context.CanRaise && this.NeedRaiseToMatchThePlaystyle(context, stats))
                {
                    return this.ToRaise(this.RandomBet(context, 0.6, 1.0), context);
                }
            }

            return PlayerAction.CheckOrCall();
        }

        private PlayerAction ReactionCausedByWeakHand(
            IGetTurnContext context, PlayerEconomy playerEconomy, Stats stats)
        {
            var investment = (int)playerEconomy.OptimalInvestment(context.CurrentPot);

            if (investment < context.MoneyToCall)
            {
                return PlayerAction.Fold();
            }

            if (context.CanRaise)
            {
                if (investment >= context.MoneyToCall + context.MinRaise)
                {
                    if (this.NeedRaiseToMatchThePlaystyle(context, stats))
                    {
                        // bluff
                        return this.ToRaise(this.RandomBet(context, 0.4, 0.6), context);
                    }
                }
            }

            return PlayerAction.CheckOrCall();
        }

        private int RandomBet(IGetTurnContext context, double lowerLimit, double upperLimit)
        {
            var min = (int)(context.CurrentPot * lowerLimit) - context.MoneyToCall;
            var max = (int)((context.CurrentPot + 1) * upperLimit) - context.MoneyToCall;
            var difference = max - min;

            min = min >= context.MinRaise ? min : context.MinRaise;
            max = max > min ? max : min + difference;

            return RandomProvider.Next(min, max);
        }

        private bool NeedRaiseToMatchThePlaystyle(IGetTurnContext context, Stats stats)
        {
            if (stats.CBet().StatsOfCurrentStreet().StatsOfCurrentPosition().IsOpportunity)
            {
                if (this.PlayingStyle.CBetDeviation(stats).Amount <=
                    this.PlayingStyle.CBet[context.RoundType].Amount)
                {
                    return true;
                }
            }

            if (this.PlayingStyle.PostflopAFqDeviation(stats).Amount <=
                    this.PlayingStyle.AFq[context.RoundType].Amount)
            {
                return true;
            }

            return false;
        }
    }
}
