namespace TexasHoldem.AI.NeuroTraining.SharpNeatPoker
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using HandEvaluatorExtension;
    using SharpNeat.Core;
    using SharpNeat.Phenomes;
    using TexasHoldem.Logic.Players;

    public class Evaluator : IPhenomeEvaluator<IBlackBox>
    {
        private object sync;

        private ManualResetEvent startHandSignal;

        private ManualResetEvent resumeHandSignal;

        private ManualResetEvent syncSignal;

        private int populationSize;

        private IStartHandContext startHandContext;

        private IEndHandContext endHandContext;

        private IBlackBox phenome;

        private int millisecondsTimeout;

        private double preflopScore;

        private double postflopScore;

        private int postflopActionsCounter;

        private double maxFitness;

        private int maxFitnessDuration;

        private bool isStopping;

        public Evaluator()
        {
            this.sync = new object();
            this.startHandSignal = new ManualResetEvent(false);
            this.resumeHandSignal = new ManualResetEvent(false);
            this.syncSignal = new ManualResetEvent(false);
            this.millisecondsTimeout = -1;
        }

        public ulong EvaluationCount { get; private set; }

        public bool StopConditionSatisfied { get; private set; }

        public void StartGame(int populationSize)
        {
            this.populationSize = populationSize;
        }

        public void StartHand(IStartHandContext context, int strengthIndex)
        {
            lock (this.sync)
            {
                this.startHandContext = context;
                this.preflopScore = (169 - strengthIndex) / 169.0;
                this.postflopScore = 0;
                this.postflopActionsCounter = 0;

                this.startHandSignal.Set();

                this.syncSignal.WaitOne(this.millisecondsTimeout);
                this.syncSignal.Reset();
            }
        }

        public void Turn(IGetTurnContext context, PlayerAction reaction, ulong pocket, ulong board)
        {
            lock (this.sync)
            {
                if (context.RoundType != Logic.GameRoundType.PreFlop)
                {
                    // Range of EVRatio [-1..1]. If less than zero, then an unprofitable action
                    this.postflopScore += 1.0 + this.EVRatio(context, reaction, pocket, board);
                    this.postflopActionsCounter += 2;
                }

                this.resumeHandSignal.Set();

                this.syncSignal.WaitOne(this.millisecondsTimeout);
                this.syncSignal.Reset();
            }
        }

        public IBlackBox GetCurrentPhenome()
        {
            lock (this.sync)
            {
                return this.phenome;
            }
        }

        public void EndHand(IEndHandContext context)
        {
            lock (this.sync)
            {
                this.endHandContext = context;

                this.resumeHandSignal.Set();

                this.syncSignal.WaitOne(this.millisecondsTimeout);
                this.syncSignal.Reset();
            }
        }

        public FitnessInfo Evaluate(IBlackBox phenome)
        {
            var fitness = 0.0;
            var numberOfHandsPlayedByTheOneGenome = 0;

            do
            {
                this.startHandSignal.WaitOne();
                this.startHandSignal.Reset();

                var actionsCounter = 0.0;

                numberOfHandsPlayedByTheOneGenome++;
                this.phenome = phenome;

                this.syncSignal.Set();

                do
                {
                    this.resumeHandSignal.WaitOne(); // get turn or end hand
                    this.resumeHandSignal.Reset();

                    if (this.endHandContext != null)
                    {
                        if (actionsCounter == 0)
                        {
                            // 1. This is the beginning of the hand. All but this player folded their cards.
                            // 2. It's a showdown. This player was preflop in the auto-all-in.
                            // Skip this hand.
                            numberOfHandsPlayedByTheOneGenome--;
                            break;
                        }

                        var postflopScoreRatio = this.postflopActionsCounter == 0
                            ? 1 : this.postflopScore / this.postflopActionsCounter;

                        if (this.endHandContext.MoneyLeft < this.startHandContext.MoneyLeft)
                        {
                            // lost
                            var wonMoneyRatio = 2.0 - ((this.startHandContext.MoneyLeft - this.endHandContext.MoneyLeft)
                                / (double)this.startHandContext.MoneyLeft); // [1..2)

                            fitness += wonMoneyRatio * (this.preflopScore + (2.0 * postflopScoreRatio)); // [0..6)
                        }
                        else if (this.endHandContext.MoneyLeft >= this.startHandContext.MoneyLeft)
                        {
                            // won
                            var wonMoneyRatio = 1.0 + ((this.endHandContext.MoneyLeft - this.startHandContext.MoneyLeft)
                                / (double)this.endHandContext.MoneyLeft); // [1..2)

                            fitness += (2.0 * wonMoneyRatio) * (this.preflopScore + (2.0 * postflopScoreRatio));
                        }

                        break;
                    }
                    else
                    {
                        actionsCounter += 1.0;
                        this.syncSignal.Set();
                    }
                }
                while (true);

                this.startHandContext = null;
                this.endHandContext = null;
                this.syncSignal.Set();
            }
            while (numberOfHandsPlayedByTheOneGenome < 6);

            this.EvaluationCount++;

            if (fitness > this.maxFitness)
            {
                if (this.maxFitnessDuration > this.populationSize * 20)
                {
                    this.isStopping = true;
                }

                this.maxFitness = fitness;
                this.maxFitnessDuration = 0;
            }
            else
            {
                this.maxFitnessDuration++;
            }

            if (this.isStopping)
            {
                this.StopConditionSatisfied = true;
                this.millisecondsTimeout = 2500;
            }

            return new FitnessInfo(fitness, fitness);
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        private double WinOdds(IGetTurnContext context, ulong pocket, ulong board)
        {
            var opponentsCards = new List<ulong>();
            var dead = 0UL;

            foreach (var item in context.Opponents)
            {
                if (item.InHand)
                {
                    opponentsCards.Add(new CardAdapter(item.HoleCards).Mask);
                }
                else
                {
                    dead |= new CardAdapter(item.HoleCards).Mask;
                }
            }

            return HandEx.HandWinOdds(pocket, opponentsCards.ToArray(), dead, board);
        }

        private double EVRatio(IGetTurnContext context, PlayerAction reaction, ulong pocket, ulong board)
        {
            var winOdds = this.WinOdds(context, pocket, board);
            var wager = 0;

            if (reaction.Type == PlayerActionType.Fold)
            {
                return 0;
            }
            else if (reaction.Type == PlayerActionType.Raise)
            {
                wager = reaction.Money;
            }
            else
            {
                wager = context.MoneyToCall;
            }

            var ev = (context.CurrentPot * winOdds) - (wager * (1.0 - winOdds));

            if (ev >= 0)
            {
                return ev / context.CurrentPot;
            }
            else
            {
                return ev / wager;
            }
        }
    }
}
