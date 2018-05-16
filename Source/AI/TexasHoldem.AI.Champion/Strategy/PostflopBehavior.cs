namespace TexasHoldem.AI.Champion.Strategy
{
    using System.Collections.Generic;
    using System.Linq;

    using HandEvaluatorExtension;
    using TexasHoldem.Logic;
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
            var calculator = this.Calculator(pocket, communityCards, context);
            var list = calculator.OnlyCurrentRound();
            var playerEconomy = new PlayerEconomy(
                list.First(p => p.Pocket.Mask == pocket.Mask),
                list.Where(p => p.Pocket.Mask != pocket.Mask).ToList());

            if (playerEconomy.NutHand || playerEconomy.BestHand)
            {
                return this.ReactionCausedByBestHand(context, playerEconomy, stats);
            }
            else
            {
                return this.ReactionCausedByWeakHand(context, playerEconomy, stats);
            }
        }

        private PlayerAction ReactionCausedByBestHand(
            IGetTurnContext context, PlayerEconomy playerEconomy, Stats stats)
        {
            if (playerEconomy.TiedHandsWithHero > 0)
            {
                if (context.CanRaise
                    && playerEconomy.HandsThatLoseToTheHero.Count > 0
                    && stats.CBet().IsOpportunity
                    && this.PlayingStyle.CBetDeviation(stats) < 0)
                {
                    // Correct the CBet
                    return this.RaiseOrPush(this.RandomBet(context, 0.4, 0.6), context);
                }
            }
            else
            {
                if (context.RoundType != GameRoundType.River
                    && !stats.IsInPosition
                    && this.PlayingStyle.CheckRaiseDeviation(stats) < 0)
                {
                    if (context.CanCheck)
                    {
                        // Correct the CheckRaise
                        return PlayerAction.CheckOrCall();
                    }
                    else if (context.CanRaise
                        && stats.CheckRaise().IsOpportunity)
                    {
                        // Correct the CheckRaise
                        return this.RaiseOrPush(this.RandomBet(context, 0.9, 1.3), context);
                    }
                }

                if (context.CanRaise
                    && stats.CBet().IsOpportunity
                    && this.PlayingStyle.CBetDeviation(stats) < 0)
                {
                    // Correct the CBet
                    return this.RaiseOrPush(this.RandomBet(context, 0.5, 0.7), context);
                }
            }

            if (context.CanRaise
                && this.PlayingStyle.PostflopAFqDeviation(stats) < 0)
            {
                if (context.RoundType == GameRoundType.River)
                {
                    return this.RaiseOrPush(
                        context.CanCheck ? this.RandomBet(context, 0.5, 0.7) : this.RandomBet(context, 0.9, 1.3),
                        context);
                }
                else if (!stats.CheckRaise().IsOpportunity)
                {
                    return this.RaiseOrPush(
                        context.CanCheck ? this.RandomBet(context, 0.6, 0.7) : this.RandomBet(context, 0.7, 0.9),
                        context);
                }
            }

            return PlayerAction.CheckOrCall();
        }

        private PlayerAction ReactionCausedByWeakHand(
            IGetTurnContext context, PlayerEconomy playerEconomy, Stats stats)
        {
            //var investment = (int)playerEconomy.OptimalInvestment(context.CurrentPot);

            if (!context.CanCheck &&
                (stats.FoldToCBet().IsOpportunity
                    && this.PlayingStyle.FoldToCBetDeviation(stats) < 0))
            {
                // Correct the FoldToCBet
                return PlayerAction.Fold();
            }

            if (context.CanCheck
                && context.CanRaise
                && this.PlayingStyle.PostflopAFqDeviation(stats) < 0
                && ((context.Opponents.Count > 2 && stats.IsInPosition) || (context.Opponents.Count == 2)))
            {
                // Correct the AFq
                var moneyToRaise = 0;
                if (context.RoundType != GameRoundType.River)
                {
                    moneyToRaise = context.CanCheck
                        ? this.RandomBet(context, 0.4, 0.7)
                        : this.RandomBet(context, 0.7, 0.9);
                }
                else
                {
                    moneyToRaise = context.CanCheck
                        ? this.RandomBet(context, 0.4, 0.7)
                        : this.RandomBet(context, 0.9, 1.3);
                }

                if (this.IsEnoughMoneyLeftAfterInvestment(moneyToRaise, context))
                {
                    // bluff
                    return this.RaiseOrPush(moneyToRaise, context);
                }
            }

            if (!context.CanCheck && context.RoundType == GameRoundType.River)
            {
                return PlayerAction.Fold();
            }

            if (!this.IsEnoughMoneyLeftAfterInvestment(context.MoneyToCall, context))
            {
                return PlayerAction.Fold();
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
    }
}
