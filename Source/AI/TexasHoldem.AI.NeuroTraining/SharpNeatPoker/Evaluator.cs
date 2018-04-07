namespace TexasHoldem.AI.NeuroTraining.SharpNeatPoker
{
    using System;
    using System.Threading;

    using SharpNeat.Core;
    using SharpNeat.Phenomes;
    using TexasHoldem.Logic.Players;

    public class Evaluator : IPhenomeEvaluator<IBlackBox>
    {
        private object sync;

        private ManualResetEvent startHandSignal;

        private ManualResetEvent resumeHandSignal;

        private ManualResetEvent syncSignal;

        private ulong populationSize;

        private IStartHandContext startHandContext;

        private IEndHandContext endHandContext;

        private IBlackBox phenome;

        private int millisecondsTimeout;

        private ulong numberOfVerificationHands;

        public Evaluator()
        {
            this.sync = new object();
            this.startHandSignal = new ManualResetEvent(false);
            this.resumeHandSignal = new ManualResetEvent(false);
            this.syncSignal = new ManualResetEvent(false);
            this.millisecondsTimeout = -1;
            this.numberOfVerificationHands = 300UL;
        }

        public ulong EvaluationCount { get; private set; }

        public bool StopConditionSatisfied { get; private set; }

        public void StartGame(ulong populationSize)
        {
            this.populationSize = populationSize;
        }

        public void StartHand(IStartHandContext context)
        {
            lock (this.sync)
            {
                this.startHandContext = context;

                this.startHandSignal.Set();

                this.syncSignal.WaitOne(this.millisecondsTimeout);
                this.syncSignal.Reset();
            }
        }

        public void Turn()
        {
            lock (this.sync)
            {
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
            var numberOfHandsPlayedByTheOnePhenome = 0UL;
            var profit = 0;
            var wonMoney = 0;
            var lostMoney = 0;
            var neutralAction = 0UL;

            do
            {
                // New hand
                this.startHandSignal.WaitOne();
                this.startHandSignal.Reset();

                var actionsCounter = 0;

                numberOfHandsPlayedByTheOnePhenome++;
                this.phenome = phenome;

                this.syncSignal.Set();

                do
                {
                    // Actions inside the hand
                    this.resumeHandSignal.WaitOne();
                    this.resumeHandSignal.Reset();

                    if (this.endHandContext != null)
                    {
                        if (actionsCounter == 0)
                        {
                            // 1. This is the beginning of the hand. All but this player folded their cards.
                            // 2. It's a showdown. This player was preflop in the auto-all-in.
                            // Skip this hand.
                            numberOfHandsPlayedByTheOnePhenome--;
                            break;
                        }

                        profit += this.endHandContext.MoneyLeft - this.startHandContext.MoneyLeft;

                        if (this.endHandContext.MoneyLeft > this.startHandContext.MoneyLeft)
                        {
                            // won hand
                            wonMoney += this.endHandContext.MoneyLeft - this.startHandContext.MoneyLeft;
                        }
                        else if (this.endHandContext.MoneyLeft == this.startHandContext.MoneyLeft)
                        {
                            neutralAction++;
                        }
                        else
                        {
                            // lost hand
                            lostMoney += this.startHandContext.MoneyLeft - this.endHandContext.MoneyLeft;
                        }

                        break;
                    }
                    else
                    {
                        actionsCounter += 1;
                        this.syncSignal.Set();
                    }
                }
                while (true);

                this.startHandContext = null;
                this.endHandContext = null;
                this.syncSignal.Set();
            }
            while (numberOfHandsPlayedByTheOnePhenome < this.numberOfVerificationHands);

            this.EvaluationCount++;

            // TODO: a correct condition for stopping the training is necessary
            if (this.EvaluationCount >= (uint)this.populationSize * 1000)
            {
                this.StopConditionSatisfied = true;
                this.millisecondsTimeout = 2500;
            }

            var fitnessRatio = 1.0;
            var sum = wonMoney + lostMoney;

            if (sum > 0)
            {
                var profitRatio = (double)profit / (double)sum;
                profitRatio *= (double)(this.numberOfVerificationHands - neutralAction) / (double)this.numberOfVerificationHands;

                if (profitRatio >= 0)
                {
                    fitnessRatio += profitRatio;
                }
                else
                {
                    fitnessRatio += profitRatio;
                }
            }

            var fitness = Math.Pow(fitnessRatio, 2);

            return new FitnessInfo(fitness, fitness);
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
