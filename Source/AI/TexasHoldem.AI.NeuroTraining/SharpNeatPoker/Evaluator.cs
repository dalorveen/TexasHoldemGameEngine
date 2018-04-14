namespace TexasHoldem.AI.NeuroTraining.SharpNeatPoker
{
    using System;
    using System.Collections.Generic;

    using AI.DummyPlayer;
    using AI.SmartPlayer;
    using SharpNeat.Core;
    using SharpNeat.Phenomes;
    using TexasHoldem.Logic.Extensions;
    using TexasHoldem.Logic.GameMechanics;
    using TexasHoldem.Logic.Players;

    public class Evaluator : IPhenomeEvaluator<IBlackBox>
    {
        public ulong EvaluationCount { get; private set; }

        public bool StopConditionSatisfied { get; private set; }

        public FitnessInfo Evaluate(IBlackBox phenome)
        {
            var players = new List<IPlayer>();
            var learner = new Learner(RandomProvider.Next(80, 201), phenome);

            players.Add(learner);
            players.Add(new DummyPlayer(RandomProvider.Next(80, 201)));
            players.Add(new SmartPlayer(RandomProvider.Next(80, 201)));
            players.Add(new DummyPlayer(RandomProvider.Next(80, 201)));

            var game = new TexasHoldemGame(players, 300);
            game.Start();

            this.EvaluationCount++;

            var fitness = learner.Fitness();

            return new FitnessInfo(fitness, fitness);
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
