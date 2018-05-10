namespace TexasHoldem.AI.NeuroPlayer.Helpers
{
    using System.Collections.Generic;

    using SharpNeat.Genomes.Neat;
    using SharpNeat.Phenomes;
    using TexasHoldem.AI.Champion;
    using TexasHoldem.AI.Champion.Strategy;
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
            var learner = new Learner(200, (IBlackBox)genome.CachedPhenome);

            this.Stats = learner.Stats;

            players.Add(learner);
            players.Add(new Champion(new PlayingStyle(), 200));
            players.Add(new Champion(new PlayingStyle(), 200));
            players.Add(new Champion(new PlayingStyle(), 200));
            players.Add(new Champion(new PlayingStyle(), 200));
            players.Add(new Champion(new PlayingStyle(), 200));

            var game = new TexasHoldemGame(players, 10000);

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
