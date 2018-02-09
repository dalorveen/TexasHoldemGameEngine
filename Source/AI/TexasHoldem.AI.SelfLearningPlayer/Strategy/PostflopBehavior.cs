namespace TexasHoldem.AI.SelfLearningPlayer.Strategy
{
    using System;
    using System.Collections.Generic;

    using TexasHoldem.AI.SelfLearningPlayer.PokerMath;
    using TexasHoldem.Logic.Cards;
    using TexasHoldem.Logic.Extensions;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;

    public class PostflopBehavior : BaseBehavior
    {
        private readonly PlayerEconomy playerEconomy;

        public PostflopBehavior(
            IPocket pocket, IStats playingStyle, IGetTurnContext context, IReadOnlyCollection<Card> communityCards)
            : base(pocket, playingStyle, context, communityCards)
        {
            this.playerEconomy = this.PlayerEconomy();
        }

        public override PlayerAction OptimalAction()
        {
            if (this.playerEconomy.NutHand)
            {
                return this.ReactionCausedByNutHand();
            }
            else if (this.playerEconomy.BestHand)
            {
                return this.ReactionCausedByBestHand();
            }
            else
            {
                return this.ReactionCausedByWeakHand();
            }
        }

        private PlayerAction ReactionCausedByNutHand()
        {
            if (this.Context.AvailablePlayerOptions.Contains(PlayerActionType.Raise))
            {
                if (this.Tracker.OutOfPosition || this.Context.RoundType == Logic.GameRoundType.River)
                {
                    return this.ToValueBet();
                }
            }

            return PlayerAction.CheckOrCall();
        }

        private PlayerAction ReactionCausedByBestHand()
        {
            if (this.playerEconomy.TiedHandsWithHero > 0)
            {
                if (this.playerEconomy.HandsThatLoseToTheHero.Count > 0
                    && this.Context.AvailablePlayerOptions.Contains(PlayerActionType.Raise))
                {
                    var investment = (int)this.playerEconomy.OptimalInvestment(this.Context.CurrentPot);
                    if (this.IsPush(investment - this.Context.MoneyToCall))
                    {
                        return this.RaiseOrAllIn(int.MaxValue);
                    }
                    else
                    {
                        return this.RaiseOrAllIn(investment - this.Context.MoneyToCall);
                    }
                }
            }
            else
            {
                if (this.Context.AvailablePlayerOptions.Contains(PlayerActionType.Raise))
                {
                    return this.ToValueBet();
                }
            }

            return PlayerAction.CheckOrCall();
        }

        private PlayerAction ReactionCausedByWeakHand()
        {
            var investment = (int)this.playerEconomy.OptimalInvestment(this.Context.CurrentPot);

            if (investment < this.Context.MoneyToCall)
            {
                return PlayerAction.Fold();
            }

            if (this.Context.AvailablePlayerOptions.Contains(PlayerActionType.Raise))
            {
                if (investment >= this.Context.MoneyToCall + this.Context.MinRaise)
                {
                    if (!this.IsPush(investment - this.Context.MoneyToCall))
                    {
                        // TODO: make use of Aggression Factor = (Raise% + Bet%) / Call%
                        if (this.Tracker.InPosition)
                        {
                            if (this.Context.MoneyToCall == 0 || RandomProvider.Next(0, 3) == 0)
                            {
                                return this.RaiseOrAllIn(investment - this.Context.MoneyToCall);
                            }
                        }
                        else
                        {
                            var quotient = (double)investment / (double)this.Context.CurrentPot;
                            if (quotient > 0.416 && RandomProvider.Next(0, 3) == 0)
                            {
                                return this.RaiseOrAllIn(investment - this.Context.MoneyToCall);
                            }
                        }
                    }
                }
            }

            return PlayerAction.CheckOrCall();
        }

        private PlayerAction ToValueBet()
        {
            var moneyToRaise =
                (int)this.playerEconomy.OptimalInvestment(this.Context.CurrentPot) - this.Context.MoneyToCall;
            moneyToRaise = moneyToRaise < this.Context.MinRaise ? this.Context.MinRaise : moneyToRaise;
            if (moneyToRaise >= this.Context.CurrentPot + 1)
            {
                if (this.IsPush(moneyToRaise))
                {
                    return this.RaiseOrAllIn(int.MaxValue);
                }
                else
                {
                    return this.RaiseOrAllIn(moneyToRaise);
                }
            }
            else
            {
                var n = 1.0 + (RandomProvider.Next(0, 1001) / 1000.0);
                var k = Math.Pow(2.0 * (n - 1.0), n) / 4.0;
                var valueBet = moneyToRaise + ((this.Context.CurrentPot + 1 - moneyToRaise) * k);
                if (this.IsPush((int)valueBet))
                {
                    return this.RaiseOrAllIn(int.MaxValue);
                }
                else
                {
                    return this.RaiseOrAllIn((int)valueBet);
                }
            }
        }
    }
}
