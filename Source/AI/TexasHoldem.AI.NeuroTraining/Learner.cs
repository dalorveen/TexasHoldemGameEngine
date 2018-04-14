namespace TexasHoldem.AI.NeuroTraining
{
    using System;

    using HandEvaluatorExtension;
    using HoldemHand;
    using SharpNeat.Phenomes;
    using TexasHoldem.AI.NeuroPlayer;
    using TexasHoldem.Logic.Players;

    public class Learner : NeuroPlayer
    {
        private int startHandMoney;

        private int wonMoney;

        private int lostMoney;

        private int preflopFoldScore;

        private int preflopFoldOpportunity;

        private int postflopFoldScore;

        private int postflopFoldOpportunity;

        private bool takesPartInTheCurrentHand;

        public Learner(IBlackBox phenome)
            : base(phenome)
        {
            this.BuyIn = -1;
        }

        public Learner(int buyIn, IBlackBox phenome)
            : base(phenome)
        {
            this.BuyIn = buyIn;
        }

        public override string Name { get; } = "Learner_" + Guid.NewGuid();

        public override int BuyIn { get; }

        public int HandsPlayed { get; private set; }

        public int Profit { get; private set; }

        public override void StartHand(IStartHandContext context)
        {
            base.StartHand(context);

            this.startHandMoney = context.MoneyLeft;
        }

        public override PlayerAction GetTurn(IGetTurnContext context)
        {
            this.takesPartInTheCurrentHand = true;

            var playerAction = base.GetTurn(context);

            if (playerAction.Type == PlayerActionType.Fold)
            {
                if (context.RoundType == Logic.GameRoundType.PreFlop)
                {
                    this.preflopFoldScore += 169 - StartingHandStrength.ForceIndexOfStartingHand[this.PocketMask];
                    this.preflopFoldOpportunity += 168;
                }
                else
                {
                    this.postflopFoldScore += 8 - (int)Hand.EvaluateType(this.PocketMask | this.CommunityCardsMask);
                    this.postflopFoldOpportunity += 8;
                }
            }

            return playerAction;
        }

        public override void EndHand(IEndHandContext context)
        {
            base.EndHand(context);

            if (this.takesPartInTheCurrentHand)
            {
                this.HandsPlayed++;
                this.Profit += context.MoneyLeft - this.startHandMoney;

                if (context.MoneyLeft > this.startHandMoney)
                {
                    // won hand
                    this.wonMoney += context.MoneyLeft - this.startHandMoney;
                }
                else if (context.MoneyLeft == this.startHandMoney)
                {
                    // 1. The player does not sit in the blinds and folds his cards pre-flop
                    // 2. The player shared the pot at the showdown and he wins exactly as much as he invested
                }
                else
                {
                    // lost hand
                    this.lostMoney += this.startHandMoney - context.MoneyLeft;
                }
            }

            this.takesPartInTheCurrentHand = false;
        }

        public double Fitness()
        {
            var sum = this.wonMoney + this.lostMoney;

            if (sum > 0)
            {
                var score = 0.0;
                var profitRatio = (double)this.Profit / (double)sum;
                var preflopFoldRatio = this.preflopFoldOpportunity == 0
                    ? 0.0 : (double)this.preflopFoldScore / (double)this.preflopFoldOpportunity;
                var postflopFoldRatio = this.postflopFoldOpportunity == 0
                    ? 0.0 : (double)this.postflopFoldScore / (double)this.postflopFoldOpportunity;

                if (profitRatio >= 0)
                {
                    score = 3.0 + profitRatio + preflopFoldRatio + postflopFoldRatio;
                }
                else
                {
                    score = (1.0 + profitRatio) + preflopFoldRatio + postflopFoldRatio;
                }

                return Math.Pow(score, 2);
            }

            return 0;
        }
    }
}
