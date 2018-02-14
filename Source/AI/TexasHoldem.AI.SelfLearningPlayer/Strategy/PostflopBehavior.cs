namespace TexasHoldem.AI.SelfLearningPlayer.Strategy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.AI.SelfLearningPlayer.Helpers;
    using TexasHoldem.Logic.Cards;
    using TexasHoldem.Logic.Extensions;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;

    public class PostflopBehavior : PreflopBehavior
    {
        public PostflopBehavior(IStats playingStyle)
            : base(playingStyle)
        {
        }

        public override PlayerAction OptimalAction(
            IPocket pocket, IGetTurnExtendedContext context, IReadOnlyCollection<Card> communityCards)
        {
            var playerEconomy = this.PlayerEconomy(pocket, context, communityCards);

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

        private PlayerAction ReactionCausedByNutHand(IGetTurnExtendedContext context, PlayerEconomy playerEconomy)
        {
            if (context.AvailablePlayerOptions.Contains(PlayerActionType.Raise))
            {
                if (!(context.Position > context.Opponents.Where(x => x.InHand).Max(s => s.Position))
                    || context.RoundType == Logic.GameRoundType.River)
                {
                    return this.ToValueBet(context, playerEconomy);
                }
            }

            return PlayerAction.CheckOrCall();
        }

        private PlayerAction ReactionCausedByBestHand(IGetTurnExtendedContext context, PlayerEconomy playerEconomy)
        {
            if (playerEconomy.TiedHandsWithHero > 0)
            {
                if (playerEconomy.HandsThatLoseToTheHero.Count > 0
                    && context.AvailablePlayerOptions.Contains(PlayerActionType.Raise))
                {
                    var investment = (int)playerEconomy.OptimalInvestment(context.CurrentPot);
                    if (this.IsPush(investment - context.MoneyToCall, context))
                    {
                        return this.RaiseOrAllIn(int.MaxValue, context);
                    }
                    else
                    {
                        return this.RaiseOrAllIn(investment - context.MoneyToCall, context);
                    }
                }
            }
            else
            {
                if (context.AvailablePlayerOptions.Contains(PlayerActionType.Raise))
                {
                    return this.ToValueBet(context, playerEconomy);
                }
            }

            return PlayerAction.CheckOrCall();
        }

        private PlayerAction ReactionCausedByWeakHand(IGetTurnExtendedContext context, PlayerEconomy playerEconomy)
        {
            var investment = (int)playerEconomy.OptimalInvestment(context.CurrentPot);

            if (investment < context.MoneyToCall)
            {
                return PlayerAction.Fold();
            }

            if (context.AvailablePlayerOptions.Contains(PlayerActionType.Raise))
            {
                if (investment >= context.MoneyToCall + context.MinRaise)
                {
                    if (!this.IsPush(investment - context.MoneyToCall, context))
                    {
                        // TODO: make use of Aggression Factor = (Raise% + Bet%) / Call%
                        if (context.Position > context.Opponents.Where(x => x.InHand).Max(s => s.Position))
                        {
                            if (context.MoneyToCall == 0 || RandomProvider.Next(0, 3) == 0)
                            {
                                return this.RaiseOrAllIn(investment - context.MoneyToCall, context);
                            }
                        }
                        else
                        {
                            var quotient = (double)investment / (double)context.CurrentPot;
                            if (quotient > 0.416 && RandomProvider.Next(0, 3) == 0)
                            {
                                return this.RaiseOrAllIn(investment - context.MoneyToCall, context);
                            }
                        }
                    }
                }
            }

            return PlayerAction.CheckOrCall();
        }

        private PlayerAction ToValueBet(IGetTurnExtendedContext context, PlayerEconomy playerEconomy)
        {
            var moneyToRaise =
                (int)playerEconomy.OptimalInvestment(context.CurrentPot) - context.MoneyToCall;
            moneyToRaise = moneyToRaise < context.MinRaise ? context.MinRaise : moneyToRaise;
            if (moneyToRaise >= context.CurrentPot + 1)
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
            else
            {
                var n = 1.0 + (RandomProvider.Next(0, 1001) / 1000.0);
                var k = Math.Pow(2.0 * (n - 1.0), n) / 4.0;
                var valueBet = moneyToRaise + ((context.CurrentPot + 1 - moneyToRaise) * k);
                if (this.IsPush((int)valueBet, context))
                {
                    return this.RaiseOrAllIn(int.MaxValue, context);
                }
                else
                {
                    return this.RaiseOrAllIn((int)valueBet, context);
                }
            }
        }
    }
}
