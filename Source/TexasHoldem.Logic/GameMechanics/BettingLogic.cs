﻿namespace TexasHoldem.Logic.GameMechanics
{
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic.Players;

    internal class BettingLogic
    {
        private readonly int initialPlayerIndex;

        private readonly IList<InternalPlayer> allPlayers;

        private readonly int smallBlind;

        private int lastRoundBet;

        private int lastStepBet;

        private string aggressorName;

        private PotCreator potCreator;

        public BettingLogic(IList<InternalPlayer> players, int smallBlind)
        {
            this.initialPlayerIndex = players.Count == 2 ? 0 : 1;
            this.allPlayers = players;
            this.smallBlind = smallBlind;
            this.HandBets = new List<PlayerActionAndName>();

            this.lastRoundBet = 2 * smallBlind; // Big blind
            this.lastStepBet = this.lastRoundBet;
            this.aggressorName = string.Empty;

            this.potCreator = new PotCreator(this.allPlayers);
        }

        public int Pot
        {
            get
            {
                return this.allPlayers.Sum(x => x.PlayerMoney.CurrentlyInPot);
            }
        }

        public Pot MainPot
        {
            get
            {
                return this.potCreator.MainPot;
            }
        }

        public List<Pot> SidePots
        {
            get
            {
                return this.potCreator.SidePots;
            }
        }

        public List<PlayerActionAndName> HandBets { get; }

        public void Bet(GameRoundType gameRoundType)
        {
            var playerIndex = gameRoundType == GameRoundType.PreFlop
                ? this.initialPlayerIndex
                : 1;

            if (gameRoundType == GameRoundType.PreFlop)
            {
                this.PlaceBlinds();
                playerIndex = this.initialPlayerIndex + 2;
            }

            if (this.allPlayers.Count(x => x.PlayerMoney.ShouldPlayInRound) <= 1)
            {
                return;
            }

            while (this.allPlayers.Count(x => x.PlayerMoney.InHand) >= 2
                   && this.allPlayers.Any(x => x.PlayerMoney.ShouldPlayInRound))
            {
                var player = this.allPlayers[playerIndex % this.allPlayers.Count];
                if (player.PlayerMoney.Money <= 0)
                {
                    // Players who are all-in are not involved in betting round
                    player.PlayerMoney.ShouldPlayInRound = false;
                    playerIndex++;
                    continue;
                }

                if (!player.PlayerMoney.InHand || !player.PlayerMoney.ShouldPlayInRound)
                {
                    if (player.PlayerMoney.InHand == player.PlayerMoney.ShouldPlayInRound)
                    {
                        playerIndex++;
                    }

                    continue;
                }

                var maxMoneyPerPlayer = this.allPlayers.Max(x => x.PlayerMoney.CurrentRoundBet);
                var action =
                    player.GetTurn(
                        new GetTurnContext(
                            gameRoundType,
                            this.HandBets.AsReadOnly(),
                            this.smallBlind,
                            player.PlayerMoney.Money,
                            this.Pot,
                            player.PlayerMoney.CurrentRoundBet,
                            maxMoneyPerPlayer,
                            this.MinRaise(maxMoneyPerPlayer, player.PlayerMoney.CurrentRoundBet, player.Name),
                            this.MainPot,
                            this.SidePots,
                            this.CreateOpponents(player)));

                action = player.PlayerMoney.DoPlayerAction(action, maxMoneyPerPlayer);
                this.HandBets.Add(new PlayerActionAndName(player.Name, action, gameRoundType));

                if (action.Type == PlayerActionType.Raise)
                {
                    // When raising, all players are required to do action afterwards in current round
                    foreach (var playerToUpdate in this.allPlayers)
                    {
                        playerToUpdate.PlayerMoney.ShouldPlayInRound = playerToUpdate.PlayerMoney.InHand ? true : false;
                    }
                }

                player.PlayerMoney.ShouldPlayInRound = false;
                playerIndex++;
            }

            if (this.allPlayers.Count == 2)
            {
                // works only for heads-up
                this.ReturnMoneyInCaseOfAllIn();
            }
            else
            {
                this.ReturnMoneyInCaseUncalledBet();
            }
        }

        private void PlaceBlinds()
        {
            // Small blind
            this.HandBets.Add(
                new PlayerActionAndName(
                    this.allPlayers[this.initialPlayerIndex].Name,
                    this.allPlayers[this.initialPlayerIndex].PostingBlind(
                        new PostingBlindContext(
                            this.allPlayers[this.initialPlayerIndex].PlayerMoney.DoPlayerAction(PlayerAction.Post(this.smallBlind), 0),
                            0,
                            this.allPlayers[this.initialPlayerIndex].PlayerMoney.Money)),
                    GameRoundType.PreFlop));

            // Big blind
            this.HandBets.Add(
                new PlayerActionAndName(
                    this.allPlayers[this.initialPlayerIndex + 1].Name,
                    this.allPlayers[this.initialPlayerIndex + 1].PostingBlind(
                        new PostingBlindContext(
                            this.allPlayers[this.initialPlayerIndex + 1].PlayerMoney.DoPlayerAction(PlayerAction.Post(2 * this.smallBlind), 0),
                            this.Pot,
                            this.allPlayers[this.initialPlayerIndex + 1].PlayerMoney.Money)),
                    GameRoundType.PreFlop));
        }

        private void ReturnMoneyInCaseOfAllIn()
        {
            var minMoneyPerPlayer = this.allPlayers.Min(x => x.PlayerMoney.CurrentRoundBet);
            foreach (var player in this.allPlayers)
            {
                player.PlayerMoney.NormalizeBets(minMoneyPerPlayer);
            }
        }

        private void ReturnMoneyInCaseUncalledBet()
        {
            var group = this.allPlayers.GroupBy(x => x.PlayerMoney.CurrentRoundBet).OrderByDescending(k => k.Key);
            if (group.First().Count() == 1)
            {
                group.First().First().PlayerMoney.NormalizeBets(group.ElementAt(1).First().PlayerMoney.CurrentRoundBet);
            }
        }

        private int MinRaise(int maxMoneyPerPlayer, int currentRoundBet, string currentPlayerName)
        {
            /*
             * Examples:
             * 1. SB->5$; BB->10$; UTG->raise 35$; MP->minimum raise=60$ (35$ + 25$)
             * 2. SB->5$; BB->10$; UTG->raise 35$; MP->re-raise 150$; CO->call 150$; BTN->minimum raise=265$ (150$ + 115$)
             * 3. SB->5$; BB->10$; UTG->raise 12$(ALL-IN); MP->minimum raise=22$ (12$ + 10$)
            */

            if (maxMoneyPerPlayer == 0)
            {
                // Start postflop. Players did not bet.
                this.lastRoundBet = 0;
                this.lastStepBet = 2 * this.smallBlind; // big blind
                this.aggressorName = string.Empty;
            }

            if (maxMoneyPerPlayer > this.lastRoundBet)
            {
                if (this.lastRoundBet + this.lastStepBet <= maxMoneyPerPlayer)
                {
                    // full raise
                    this.lastStepBet = maxMoneyPerPlayer - this.lastRoundBet;
                    this.lastRoundBet = maxMoneyPerPlayer;
                    this.aggressorName = this.HandBets.Last().PlayerName;
                }
                else
                {
                    /*
                     * For example, we are sitting on CO
                     * SB->5$; BB->10$; UTG->raise 35$; MP->re-raise 37$(ALL-IN); CO->minimum raise = 62$ (37$ + 25$)
                    */
                    this.lastRoundBet = maxMoneyPerPlayer;
                }
            }

            if (this.aggressorName == currentPlayerName)
            {
                /*
                 * SB->5$; BB->10$; BTN->raise 32$; SB->re-raise 34$(ALL-IN); BB->call 34$; BTN->minimum raise is not possible
                 * Since no players completed a full raise over BTN's initial raise,
                 * neither BTN nor BB are allowed to re-raise here. Their only options are to call the 34$, or fold.
                */
                return -1;
            }

            return this.lastStepBet;
        }

        private ICollection<Opponent> CreateOpponents(InternalPlayer hero)
        {
            var shifted = this.allPlayers.ToList();
            shifted.Add(shifted.First());
            shifted.RemoveAt(0);

            int heroActionPriority = shifted.TakeWhile(p => p.Name != hero.Name).Count();
            var opponentActionPriority = -heroActionPriority;
            var opponents = new List<Opponent>();

            foreach (var item in this.allPlayers)
            {
                if (item.Name == hero.Name)
                {
                    opponentActionPriority++;
                    continue;
                }

                opponents.Add(new Opponent(
                    item.Name,
                    item.Cards,
                    opponentActionPriority++,
                    item.PlayerMoney.Money,
                    item.PlayerMoney.CurrentRoundBet,
                    item.PlayerMoney.InHand));
            }

            return opponents;
        }
    }
}