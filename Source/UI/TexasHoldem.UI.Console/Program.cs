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
            //players.Add(new Stats(new DummyPlayer()));
            //players.Add(new Stats(new SmartPlayer()));
            ////players.Add(new ConsolePlayer((6 * players.Count) + NumberOfCommonRows));
            //players.Add(new Stats(new Champion(PlayerStyles.LOOSE_AGGRESSIVE, 200)));
            //players.Add(new Stats(new DummyPlayer()));
            //players.Add(new Stats(new SmartPlayer()));
            //players.Add(new Stats(new DummyPlayer()));

            var trainedNeuralNetwork = @"..\..\..\..\AI\TexasHoldem.AI.NeuroTraining\PopulationFiles\bestAgent.xml";

            players.Add(new Stats(new ConsolePlayer((6 * players.Count) + NumberOfCommonRows)));
            players.Add(new Stats(new NeuroPlayer(trainedNeuralNetwork)));
            players.Add(new Stats(new NeuroPlayer(trainedNeuralNetwork)));
            players.Add(new Stats(new NeuroPlayer(trainedNeuralNetwork)));
            players.Add(new Stats(new NeuroPlayer(trainedNeuralNetwork)));
            players.Add(new Stats(new NeuroPlayer(trainedNeuralNetwork)));

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

            return new TexasHoldemGame(list);
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