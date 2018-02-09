namespace TexasHoldem.UI.Console
{
    using System;
    using System.Collections.Generic;

    using TexasHoldem.AI.DummyPlayer;
    using TexasHoldem.AI.SelfLearningPlayer;
    using TexasHoldem.AI.SelfLearningPlayer.Strategy;
    using TexasHoldem.AI.SmartPlayer;
    using TexasHoldem.Logic.GameMechanics;
    using TexasHoldem.Statistics;

    public static class Program
    {
        private const string ProgramName = "TexasHoldem.UI.Console (c) 2015-2018";

        private const int GameWidth = 66;

        private const int NumberOfCommonRows = 3; // place for community cards, pot, main pot, side pots

        public static void Main()
        {
            // var game = HeadsUp();
            // var game = HumanVsDummy(4);
            // var game = HumanVsHuman(6);
            // var game = HumanVsSmart(6);
            var game = HumanVsChampion(6);
            // var game = ChampionVsChampion(6);

            game.Start();
        }

        private static List<ConsoleUiDecorator> CreatePlayers(int numberOfPlayers, int playerTypeId)
        {
            var players = new List<ConsoleUiDecorator>(numberOfPlayers);
            for (int i = 0; i < numberOfPlayers; i++)
            {
                switch (playerTypeId)
                {
                    case 1:
                        players.Add(new ConsoleUiDecorator(new DummyPlayer(), (6 * i) + NumberOfCommonRows, GameWidth, 1));
                        break;
                    case 2:
                        players.Add(new ConsoleUiDecorator(new SmartPlayer(), (6 * i) + NumberOfCommonRows, GameWidth, 1));
                        break;
                    case 3:
                        var row = (6 * i) + NumberOfCommonRows;
                        players.Add(new ConsoleUiDecorator(
                            new ConsolePlayer(row, "Human_" + i + 1, 250 - (i * 20)), row, GameWidth, 1));
                        break;
                    case 4:
                        var looseAggressivePlayer = new PlayingStyle(
                            0.26,
                            0.22,
                            new Proportion(0.09, 0, 0, 0),
                            new Proportion(0.05, 0, 0, 0));
                        var stats = new Stats(new Champion(looseAggressivePlayer, 100 - (i * 4)));
                        players.Add(new ConsoleUiDecorator(stats, (6 * i) + NumberOfCommonRows, GameWidth, 1));
                        break;
                    default:
                        break;
                }
            }

            return players;
        }

        private static ITexasHoldemGame HumanVsComputer(int numberOfPlayers, int opponentTypeId)
        {
            int gameHeight = (6 * numberOfPlayers) + NumberOfCommonRows;
            Stand(gameHeight);

            var players = CreatePlayers(numberOfPlayers - 1, opponentTypeId);
            var row = (6 * (numberOfPlayers - 1)) + NumberOfCommonRows;
            players.Add(new ConsoleUiDecorator(
                new ConsolePlayer(row, "Human_1", 120),
                row,
                GameWidth,
                1));

            return new TexasHoldemGame(players.ToArray());
        }

        private static ITexasHoldemGame ComputerVsComputer(int numberOfPlayers, int opponentTypeId)
        {
            int gameHeight = (6 * numberOfPlayers) + NumberOfCommonRows;
            Stand(gameHeight);

            var players = CreatePlayers(numberOfPlayers, opponentTypeId);

            return new TexasHoldemGame(players.ToArray());
        }

        private static ITexasHoldemGame HumanVsDummy(int numberOfPlayers)
        {
            return HumanVsComputer(numberOfPlayers, 1);
        }

        private static ITexasHoldemGame HumanVsSmart(int numberOfPlayers)
        {
            return HumanVsComputer(numberOfPlayers, 2);
        }

        private static ITexasHoldemGame HumanVsHuman(int numberOfPlayers)
        {
            return HumanVsComputer(numberOfPlayers, 3);
        }

        private static ITexasHoldemGame HumanVsChampion(int numberOfPlayers)
        {
            return HumanVsComputer(numberOfPlayers, 4);
        }

        private static ITexasHoldemGame ChampionVsChampion(int numberOfPlayers)
        {
            return ComputerVsComputer(numberOfPlayers, 4);
        }

        private static void Stand(int gameHeight)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BufferHeight = Console.WindowHeight = gameHeight;
            Console.BufferWidth = Console.WindowWidth = GameWidth;

            ConsoleHelper.WriteOnConsole(gameHeight - 1, GameWidth - ProgramName.Length - 1, ProgramName, ConsoleColor.Green);
        }
    }
}
