namespace TexasHoldem.AI.NeuroTraining.Helpers
{
    using System;
    using System.Collections.Generic;

    using SharpNeat.Genomes.Neat;
    using SharpNeat.Phenomes;
    using TexasHoldem.AI.DummyPlayer;
    using TexasHoldem.AI.SmartPlayer;
    using TexasHoldem.Logic.GameMechanics;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;

    public class Agent
    {
        private readonly NeatGenome genome;

        public Agent(NeatGenome genome)
        {
            this.genome = genome;

            var players = new List<IPlayer>();
            var learner = new Learner((IBlackBox)genome.CachedPhenome);

            this.Stats = new Stats(learner);

            players.Add(this.Stats);
            players.Add(new DummyPlayer());
            players.Add(new SmartPlayer());
            players.Add(new DummyPlayer());

            var game = new TexasHoldemGame(players, 100000);

            game.Start();

            this.TotalProfit = learner.Profit;
            this.HandsPlayed = learner.HandsPlayed;
        }

        public NeatGenome Genome
        {
            get
            {
                return this.genome;
            }
        }

        public Stats Stats { get; }

        public int TotalProfit { get; }

        public int HandsPlayed { get; }

        public double MoneyWonPerHand()
        {
            return this.HandsPlayed == 0 ? 0 : (double)this.TotalProfit / (double)this.HandsPlayed;
        }
    }
}
