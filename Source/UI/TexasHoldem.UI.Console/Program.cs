﻿namespace TexasHoldem.UI.Console
{
    using System;
    using System.Collections.Generic;

    using TexasHoldem.AI.Champion;
    using TexasHoldem.AI.Champion.Strategy;
    using TexasHoldem.AI.DummyPlayer;
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
            CreateAIPlayers<DummyPlayer>(2);
            //CreateConsolePlayers(1);
            players.Add(new Champion(PlayerStyles.LOOSE_AGGRESSIVE, 200));
            CreateAIPlayers<DummyPlayer>(1);
            CreateAIPlayers<SmartPlayer>(2);

            StatsWrapsAllThePlayers();

            int gameHeight = (6 * players.Count) + NumberOfCommonRows;
            Table(gameHeight);

            var game = new TexasHoldemGame(FillTheTableWithPlayers().ToArray());
            game.Start();
        }

        private static void CreateAIPlayers<T>(int numberOfPlayers)
            where T : BasePlayer, new()
        {
            for (int i = 0; i < numberOfPlayers; i++)
            {
                players.Add(new T());
            }
        }

        private static void CreateConsolePlayers(int numberOfPlayers)
        {
            var count = players.Count;

            for (int i = count; i < numberOfPlayers + count; i++)
            {
                var row = (6 * i) + NumberOfCommonRows;
                players.Add(new ConsolePlayer(row));
            }
        }

        private static List<ConsoleUiDecorator> FillTheTableWithPlayers()
        {
            var list = new List<ConsoleUiDecorator>();

            for (int i = 0; i < players.Count; i++)
            {
                list.Add(new ConsoleUiDecorator(players[i], (6 * i) + NumberOfCommonRows, GameWidth, 1));
            }

            return list;
        }

        private static void StatsWrapsAllThePlayers()
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i] = new Stats(players[i]);
            }
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