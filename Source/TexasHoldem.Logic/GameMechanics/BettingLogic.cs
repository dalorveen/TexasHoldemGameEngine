namespace TexasHoldem.Logic.GameMechanics
{
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic.Players;

    internal class BettingLogic
    {
        private readonly int initialPlayerIndex;

        private readonly IList<InternalPlayer> allPlayers;

        private readonly int smallBlind;

        private PotCreator potCreator;

        private MinRaise minRaise;

        public BettingLogic(IList<InternalPlayer> players, int smallBlind)
        {
            this.initialPlayerIndex = players.Count == 2 ? 0 : 1;
            this.allPlayers = players;
            this.smallBlind = smallBlind;
            this.HandBets = new List<PlayerActionAndName>();
            this.potCreator = new PotCreator(this.allPlayers);
            this.minRaise = new MinRaise(this.smallBlind);
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
            this.minRaise.Reset();
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
                var currentMinRaise = this.minRaise.Amount(player.Name);
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
                            currentMinRaise,
                            this.MainPot,
                            this.SidePots,
                            this.CreateOpponents(player)));

                action = player.PlayerMoney.DoPlayerAction(action, maxMoneyPerPlayer);

                if (action.Type == PlayerActionType.Raise && action.Money < currentMinRaise && player.PlayerMoney.Money > 0)
                {
                    throw new System.Exception("The player's bet/raise is less than the minimum raise");
                }

                this.HandBets.Add(new PlayerActionAndName(player.Name, action, gameRoundType));

                if (action.Type == PlayerActionType.Raise)
                {
                    // When raising, all players are required to do action afterwards in current round
                    foreach (var playerToUpdate in this.allPlayers)
                    {
                        playerToUpdate.PlayerMoney.ShouldPlayInRound = playerToUpdate.PlayerMoney.InHand ? true : false;
                    }
                }

                this.minRaise.Update(player.Name, maxMoneyPerPlayer, player.PlayerMoney.CurrentRoundBet);
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

        private ICollection<Opponent> CreateOpponents(InternalPlayer hero)
        {
            var actionPriority = 9;
            var opponents = new List<Opponent>();

            foreach (var item in this.allPlayers.ToList())
            {
                if (item.Name == hero.Name)
                {
                    actionPriority++;
                    continue;
                }

                opponents.Add(new Opponent(
                    item.Name,
                    item.Cards,
                    actionPriority++ % 10,
                    item.PlayerMoney.Money,
                    item.PlayerMoney.CurrentRoundBet,
                    item.PlayerMoney.InHand));
            }

            return opponents;
        }
    }
}