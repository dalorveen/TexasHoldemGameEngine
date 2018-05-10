namespace TexasHoldem.Logic.GameMechanics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic.Cards;
    using TexasHoldem.Logic.Extensions;
    using TexasHoldem.Logic.Helpers;
    using TexasHoldem.Logic.Players;

    internal class HandLogic : IHandLogic
    {
        private readonly int handNumber;

        private readonly int smallBlind;

        private readonly IList<InternalPlayer> players;

        private readonly Deck deck;

        private readonly List<Card> communityCards;

        private readonly BettingLogic bettingLogic;

        private Dictionary<string, ICollection<Card>> showdownCards;

        public HandLogic(IList<InternalPlayer> players, int handNumber, int smallBlind)
        {
            this.handNumber = handNumber;
            this.smallBlind = smallBlind;
            this.players = players;
            this.deck = new Deck();
            this.communityCards = new List<Card>(5);
            this.bettingLogic = new BettingLogic(this.players, smallBlind);
            this.showdownCards = new Dictionary<string, ICollection<Card>>();
        }

        public void Play()
        {
            this.Rebuy();

            var actionPriority = this.players.Count - 1;

            // Start the hand and deal cards to each player
            foreach (var player in this.players)
            {
                var startHandContext = new StartHandContext(
                    this.deck.GetNextCard(),
                    this.deck.GetNextCard(),
                    this.handNumber,
                    player.PlayerMoney.Money,
                    this.smallBlind,
                    actionPriority++ % this.players.Count,
                    this.players.First().Name);
                player.StartHand(startHandContext);
            }

            GameRoundType lastGameRoundType;

            // Pre-flop -> blinds -> betting
            this.PlayRound(GameRoundType.PreFlop, 0);
            lastGameRoundType = GameRoundType.PreFlop;

            // Flop -> 3 cards -> betting
            if (this.players.Count(x => x.PlayerMoney.InHand) > 1)
            {
                this.PlayRound(GameRoundType.Flop, 3);
                lastGameRoundType = GameRoundType.Flop;
            }

            // Turn -> 1 card -> betting
            if (this.players.Count(x => x.PlayerMoney.InHand) > 1)
            {
                this.PlayRound(GameRoundType.Turn, 1);
                lastGameRoundType = GameRoundType.Turn;
            }

            // River -> 1 card -> betting
            if (this.players.Count(x => x.PlayerMoney.InHand) > 1)
            {
                this.PlayRound(GameRoundType.River, 1);
                lastGameRoundType = GameRoundType.River;
            }

            this.DetermineWinnerAndAddPot(this.bettingLogic.Pot, this.bettingLogic.MainPot, this.bettingLogic.SidePots);

            foreach (var player in this.players)
            {
                player.EndHand(new EndHandContext(
                    this.showdownCards, player.PlayerMoney.Money, lastGameRoundType));
            }
        }

        private void DetermineWinnerAndAddPot(int pot, Pot mainPot, List<Pot> sidePot)
        {
            if (this.players.Count(x => x.PlayerMoney.InHand) == 1)
            {
                var winner = this.players.FirstOrDefault(x => x.PlayerMoney.InHand);
                winner.PlayerMoney.Money += pot;
            }
            else
            {
                // showdown
                foreach (var player in this.players)
                {
                    if (player.PlayerMoney.InHand)
                    {
                        this.showdownCards.Add(player.Name, player.Cards);
                    }
                }

                if (this.players.Count == 2)
                {
                    var betterHand = Helpers.CompareCards(
                    this.players[0].Cards.Concat(this.communityCards),
                    this.players[1].Cards.Concat(this.communityCards));
                    if (betterHand > 0)
                    {
                        this.players[0].PlayerMoney.Money += pot;
                    }
                    else if (betterHand < 0)
                    {
                        this.players[1].PlayerMoney.Money += pot;
                    }
                    else
                    {
                        var div = pot / 2;
                        this.players[0].PlayerMoney.Money += div;
                        this.players[1].PlayerMoney.Money += div;
                    }
                }
                else
                {
                    var handRankValueOfPlayers = new SortedDictionary<int, ICollection<string>>();
                    var playersInHand = this.players.Where(p => p.PlayerMoney.InHand);

                    foreach (var player in playersInHand)
                    {
                        var opponents = playersInHand.Where(p => p.Name != player.Name).Select(s => s.Cards);
                        var handRankValue = Helpers.GetHandRankValue(player.Cards, opponents, this.communityCards);

                        if (handRankValueOfPlayers.ContainsKey(handRankValue))
                        {
                            handRankValueOfPlayers[handRankValue].Add(player.Name);
                        }
                        else
                        {
                            handRankValueOfPlayers.Add(handRankValue, new List<string> { player.Name });
                        }
                    }

                    var remainingPots = new Stack<Pot>();
                    var pots = new Stack<Pot>(sidePot);
                    pots.Push(mainPot);

                    foreach (var playersWithTheBestHand in handRankValueOfPlayers.Reverse())
                    {
                        do
                        {
                            var oneOfThePots = pots.Pop();

                            if (oneOfThePots.ActivePlayer.Count == 0)
                            {
                                throw new Exception("There are no players in the pot. Maybe the player folded " +
                                    "his cards when he could check.");
                            }
                            else if (oneOfThePots.ActivePlayer.Count == 1)
                            {
                                continue;
                            }
                            else
                            {
                                var nominees = oneOfThePots.ActivePlayer.Intersect(playersWithTheBestHand.Value);
                                var count = nominees.Count();

                                if (count > 0)
                                {
                                    var prize = oneOfThePots.AmountOfMoney / count; // TODO: If there are odd chips in a split pot.

                                    foreach (var nominee in nominees)
                                    {
                                        var winner = this.players.FirstOrDefault(x => x.Name == nominee);
                                        winner.PlayerMoney.Money += prize;
                                    }
                                }
                                else
                                {
                                    // There were no active players with the current strength of the hands taking this pot
                                    remainingPots.Push(oneOfThePots);
                                }
                            }
                        }
                        while (pots.Count > 0);

                        if (remainingPots.Count == 0)
                        {
                            break;
                        }
                        else
                        {
                            while (remainingPots.Count > 0)
                            {
                                pots.Push(remainingPots.Pop());
                            }
                        }
                    }
                }
            }
        }

        private void PlayRound(GameRoundType gameRoundType, int communityCardsCount)
        {
            for (var i = 0; i < communityCardsCount; i++)
            {
                this.communityCards.Add(this.deck.GetNextCard());
            }

            foreach (var player in this.players)
            {
                var startRoundContext = new StartRoundContext(
                    gameRoundType,
                    this.communityCards.AsReadOnly(),
                    player.PlayerMoney.Money,
                    this.bettingLogic.Pot,
                    this.bettingLogic.MainPot,
                    this.bettingLogic.SidePots);
                player.StartRound(startRoundContext);
            }

            this.bettingLogic.Bet(gameRoundType);

            foreach (var player in this.players)
            {
                var endRoundContext = new EndRoundContext(this.bettingLogic.HandBets, gameRoundType);
                player.EndRound(endRoundContext);
            }
        }

        private void Rebuy()
        {
            foreach (var player in this.players)
            {
                if (player.PlayerMoney.Money <= 0)
                {
                    player.PlayerMoney.Money = RandomProvider.Next(this.smallBlind * 80, (this.smallBlind * 200) + 1);
                }
                else if (player.PlayerMoney.Money > this.smallBlind * 200 * 6)
                {
                    // We will assume that the player has won enough to get out of the table and sit down again
                    player.PlayerMoney.Money = this.smallBlind * 200;
                }
            }
        }
    }
}