namespace TexasHoldem.AI.NeuroPlayer
{
    using System;

    using HandEvaluatorExtension;
    using HoldemHand;
    using SharpNeat.Phenomes;
    using TexasHoldem.Logic.Players;

    internal class Learner : NeuroPlayer
    {
        private int startHandMoney;
        private int wonMoney;
        private int lostMoney;
        private int rfiScore;
        private int rfiOpportunity;
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
                    this.preflopFoldScore += 169
                        - StartingHandStrength.ForceIndexOfStartingHand[this.InputData.PocketCards().Mask];
                    this.preflopFoldOpportunity += 168;
                }
                else
                {
                    this.postflopFoldScore += 8 - (int)Hand.EvaluateType(this.InputData.PocketCards().Mask
                        | this.InputData.CommunityCards().Mask);
                    this.postflopFoldOpportunity += 8;
                }
            }

            this.ScoreRFI(playerAction);

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
                var rfiRatio = this.rfiOpportunity == 0
                    ? 0.0 : (double)this.rfiScore / (double)this.rfiOpportunity;

                if (profitRatio >= 0)
                {
                    score = 4.0 + profitRatio + preflopFoldRatio + postflopFoldRatio + rfiRatio;
                }
                else
                {
                    score = (1.0 + profitRatio) + preflopFoldRatio + postflopFoldRatio + rfiRatio;
                }

                return Math.Pow(score, 2);
            }

            return 0;
        }

        private void ScoreRFI(PlayerAction playerAction)
        {
            if (this.Stats.Position.CurrentPosition != Statistics.Positions.BB
                        && this.Stats.RFI().IsOpportunitiesToOpenThePot)
            {
                this.rfiOpportunity++;

                if (playerAction.Type == PlayerActionType.Raise)
                {
                    switch (this.Stats.Position.CurrentPosition)
                    {
                        case Statistics.Positions.SB:
                            if (Range.Parse("22+, A6s+, K9s+, Q9s+, J8s+, T9s, 98s, 87s, 76s, ATo+, K9o+, Q9o+, J9o+")
                                .Contains(this.InputData.PocketCards().Mask))
                            {
                                this.rfiScore++;
                            }

                            break;
                        case Statistics.Positions.BB:
                            break;
                        case Statistics.Positions.EP:
                            if (Range.Parse("22+, ATs+, KQs, AJo+, KQo")
                                .Contains(this.InputData.PocketCards().Mask))
                            {
                                this.rfiScore++;
                            }

                            break;
                        case Statistics.Positions.MP:
                            if (Range.Parse("22+, ATs+, KJs+, ATo+, KJo+")
                                .Contains(this.InputData.PocketCards().Mask))
                            {
                                this.rfiScore++;
                            }

                            break;
                        case Statistics.Positions.CO:
                            if (Range.Parse("22+, A6s+, K9s+, Q9s+, J9s+, T8s+, 98s, 87s, 76s, 65s, A9o+, K9o+," +
                                    "Q9o+, J9o+, T9o, 98o, 87o")
                                .Contains(this.InputData.PocketCards().Mask))
                            {
                                this.rfiScore++;
                            }

                            break;
                        case Statistics.Positions.BTN:
                            if (Range.Parse("22+, A2s+, K7s+, Q7s+, J7s+, T8s+, 97s+, 87s, 76s, 65s, 54s, A2o+," +
                                    "K8o+, Q8o+, J8o+, T8o+, 97o+, 87o, 76o, 65o, 54o")
                                .Contains(this.InputData.PocketCards().Mask))
                            {
                                this.rfiScore++;
                            }

                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }
    }
}
