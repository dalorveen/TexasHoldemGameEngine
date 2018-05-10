namespace TexasHoldem.UI.Console
{
    using System;
    using System.Collections.Generic;

    using TexasHoldem.AI.Champion;
    using TexasHoldem.AI.Champion.Strategy;
    using TexasHoldem.AI.DummyPlayer;
    using TexasHoldem.AI.NeuroPlayer;
    using TexasHoldem.AI.SmartPlayer;
    using TexasHoldem.Logic.GameMechanics;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;
    using TexasHoldem.Statistics.Indicators;

    public static class Program
    {
        private const string ProgramName = "TexasHoldem.UI.Console (c) 2015-2018";

        private const int GameWidth = 66;

        private const int NumberOfCommonRows = 3; // place for community cards, pot, main pot, side pots

        private static List<IPlayer> players = new List<IPlayer>();

        public static void Main()
        {
            var trainedNeuralNetwork = @"..\..\..\..\AI\TexasHoldem.AI.NeuroPlayer\PopulationFiles\bestAgent.xml";

            //players.Add(new NeuroPlayer(trainedNeuralNetwork));
            //players.Add(new DummyPlayer());
            //players.Add(new SmartPlayer());
            //players.Add(new DummyPlayer());

            players.Add(new ConsolePlayer((6 * players.Count) + NumberOfCommonRows));
            players.Add(new Champion(new PlayingStyle(), 200));
            players.Add(new Champion(new PlayingStyle(), 200));
            players.Add(new Champion(new PlayingStyle(), 200));
            players.Add(new Champion(new PlayingStyle(), 200));
            players.Add(new Champion(new PlayingStyle(), 200));

            // var gameWithoutAnimation = new TexasHoldemGame(players, 100000);    // remove
            // var stopWatch = new System.Diagnostics.Stopwatch();                 // remove
            // stopWatch.Start();                                                  // remove
            // gameWithoutAnimation.Start();                                       // remove
            // stopWatch.Stop();                                                   // remove

            var gameHeight = (6 * players.Count) + NumberOfCommonRows;
            Table(gameHeight);

            var game = Game();
            game.Start();
        }

        private static ITexasHoldemGame Game()
        {
            var list = new List<IPlayer>();

            for (int i = 0; i < players.Count; i++)
            {
                list.Add(new ConsoleUiDecorator(players[i], (6 * i) + NumberOfCommonRows, GameWidth, 1));
            }

            return new TexasHoldemGame(list, 1000000);
        }

        private static void Table(int gameHeight)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BufferHeight = Console.WindowHeight = gameHeight;
            Console.BufferWidth = Console.WindowWidth = GameWidth;

            ConsoleHelper.WriteOnConsole(gameHeight - 1, GameWidth - ProgramName.Length - 1, ProgramName, ConsoleColor.Green);
        }
    }
}