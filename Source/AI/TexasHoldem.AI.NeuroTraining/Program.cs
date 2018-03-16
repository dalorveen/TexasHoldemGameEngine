namespace TexasHoldem.AI.NeuroTraining
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Xml;

    using SharpNeat.Core;
    using SharpNeat.EvolutionAlgorithms;
    using SharpNeat.Genomes.Neat;
    using TexasHoldem.AI.Champion;
    using TexasHoldem.AI.NeuroPlayer;
    using TexasHoldem.Logic.GameMechanics;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Write("Please wait.\nThe learning process will take a long time.\nThe learning process will " +
                "be interrupted automatically.\n");

            var players = new List<IPlayer>();

            players.Add(new Stats(new Learner()));
            players.Add(new Stats(new Champion(PlayerStyles.LOOSE_AGGRESSIVE, 200)));
            players.Add(new Stats(new Champion(PlayerStyles.LOOSE_AGGRESSIVE, 200)));
            players.Add(new Stats(new Champion(PlayerStyles.LOOSE_AGGRESSIVE, 200)));
            players.Add(new Stats(new Champion(PlayerStyles.LOOSE_AGGRESSIVE, 200)));
            players.Add(new Stats(new Champion(PlayerStyles.LOOSE_AGGRESSIVE, 200)));

            var game = new TexasHoldemGame(players);
            game.Start();
        }
    }
}
