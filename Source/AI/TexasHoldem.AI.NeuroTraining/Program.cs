namespace TexasHoldem.AI.NeuroTraining
{
    using System;
    using System.Collections.Generic;

    using TexasHoldem.AI.Champion;
    using TexasHoldem.AI.DummyPlayer;
    using TexasHoldem.AI.SmartPlayer;
    using TexasHoldem.Logic.GameMechanics;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Write("Please wait. The learning process will take a\n" +
                "long time and will be interrupted automatically.\n");

            var players = new List<IPlayer>();

            players.Add(new Learner());
            players.Add(new DummyPlayer());
            players.Add(new SmartPlayer());
            players.Add(new DummyPlayer());

            var game = new TexasHoldemGame(players);
            game.Start();

            Console.ReadKey();
        }
    }
}
