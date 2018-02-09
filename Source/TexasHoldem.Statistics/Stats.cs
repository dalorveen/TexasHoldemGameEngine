namespace TexasHoldem.Statistics
{
    using System;
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class Stats : IPlayer, IStats
    {
        private readonly IPlayer player;

        private int vpip;

        private int pfr;

        private int[] totalTimes3Bet;

        private int[] total3BetOpportunities;

        private int[] totalTimes4BetAndMore;

        private int[] total4BetAndMoreOpportunities;

        private int[] call;

        private int[] raise;

        private int moneyLeftAtTheBeginningOfTheHand;

        private int smallBlind;

        private double balanceInBigBet;

        public Stats(IPlayer player)
        {
            this.player = player;
            this.totalTimes3Bet = new int[4];
            this.total3BetOpportunities = new int[4];
            this.totalTimes4BetAndMore = new int[4];
            this.total4BetAndMoreOpportunities = new int[4];
            this.call = new int[4];
            this.raise = new int[4];
        }

        /// <summary>
        /// Gets the total number of hands
        /// </summary>
        /// <value>
        /// Played hands
        /// </value>
        public int Hands { get; private set; }

        /// <summary>
        /// Gets the percentage of time a player voluntarily put money into the pot
        /// </summary>
        /// <value>
        /// Voluntarily put money into the pot
        /// </value>
        public double VPIP
        {
            get
            {
                return ((double)this.vpip / (double)this.Hands) * 100.0;
            }
        }

        /// <summary>
        /// Gets the percentage of time a player raised pre-flop
        /// </summary>
        /// <value>
        /// Pre-flop raise
        /// </value>
        public double PFR
        {
            get
            {
                return ((double)this.pfr / (double)this.Hands) * 100.0;
            }
        }

        /// <summary>
        /// Gets the measure of how aggressive versus passive a player is on all streets
        /// </summary>
        /// <value>
        /// Aggression factor
        /// </value>
        public double AF
        {
            get
            {
                return (double)this.raise.Sum() / (double)this.call.Sum();
            }
        }

        /// <summary>
        /// Gets the average amount of big bets won or lost per 100 hands
        /// </summary>
        /// <value>
        /// Big Bets per 100 Hands
        /// </value>
        public double BBPer100
        {
            get
            {
                return this.balanceInBigBet / (this.Hands / 100.0);
            }
        }

        /// <summary>
        /// Gets the percentage of time a player re-raised a raiser on the (pre-flop/flop/turn/river)
        /// </summary>
        /// <value>
        /// Percentages of 3Bet on the streets
        /// </value>
        public Proportion ThreeBet
        {
            get
            {
                var proportion = new double[4];
                for (int i = 0; i < proportion.Length; i++)
                {
                    proportion[i] = (double)this.totalTimes3Bet[i] / (double)this.total3BetOpportunities[i];
                }

                return new Proportion(proportion[0], proportion[1], proportion[2], proportion[3]);
            }
        }

        /// <summary>
        /// Gets the percentage of time a player re-raised a 3Bet/4Bet/5Bet/XBet on the (pre-flop/flop/turn/river)
        /// </summary>
        /// <value>
        /// Percentages of 4Bet/5Bet/XBet on the streets
        /// </value>
        public Proportion FourBetAndMore
        {
            get
            {
                var proportion = new double[4];
                for (int i = 0; i < proportion.Length; i++)
                {
                    proportion[i] = (double)this.totalTimes4BetAndMore[i] / (double)this.total4BetAndMoreOpportunities[i];
                }

                return new Proportion(proportion[0], proportion[1], proportion[2], proportion[3]);
            }
        }

        public int BuyIn
        {
            get
            {
                return this.player.BuyIn;
            }
        }

        public string Name
        {
            get
            {
                return this.player.Name;
            }
        }

        public PlayerAction GetTurn(IGetTurnContext context)
        {
            var playerAction = this.player.GetTurn(context);
            var tracker = new Tracker(context);

            if (context.RoundType == Logic.GameRoundType.PreFlop
                && context.PreviousRoundActions.Any(p => p.PlayerName != this.Name))
            {
                if (playerAction.Type == PlayerActionType.Raise)
                {
                    this.vpip++;
                    this.pfr++;
                }
                else if (playerAction.Type == PlayerActionType.CheckCall)
                {
                    this.vpip++;
                }
            }

            if (tracker.ThreeBetOpportunity(context.RoundType))
            {
                if (playerAction.Type == PlayerActionType.Raise)
                {
                    this.totalTimes3Bet[(int)context.RoundType]++;
                }

                this.total3BetOpportunities[(int)context.RoundType]++;
            }

            if (tracker.FourBetAndMoreOpportunity(context.RoundType))
            {
                if (playerAction.Type == PlayerActionType.Raise)
                {
                    this.totalTimes4BetAndMore[(int)context.RoundType]++;
                }

                this.total4BetAndMoreOpportunities[(int)context.RoundType]++;
            }

            if (playerAction.Type == PlayerActionType.Raise)
            {
                this.raise[(int)context.RoundType]++;
            }
            else if (playerAction.Type == PlayerActionType.CheckCall)
            {
                this.call[(int)context.RoundType]++;
            }

            return playerAction;
        }

        public PlayerAction PostingBlind(IPostingBlindContext context)
        {
            return this.player.PostingBlind(context);
        }

        public void StartGame(IStartGameContext context)
        {
            this.player.StartGame(context);
        }

        public void StartHand(IStartHandContext context)
        {
            this.player.StartHand(context);
            this.Hands++;
            this.moneyLeftAtTheBeginningOfTheHand = context.MoneyLeft;
            this.smallBlind = context.SmallBlind;
        }

        public void StartRound(IStartRoundContext context)
        {
            this.player.StartRound(context);
        }

        public void EndRound(IEndRoundContext context)
        {
            this.player.EndRound(context);
        }

        public void EndHand(IEndHandContext context)
        {
            this.player.EndHand(context);
            var balance = context.MoneyLeft - this.moneyLeftAtTheBeginningOfTheHand;
            this.balanceInBigBet += (double)balance / (double)(this.smallBlind * 2);
        }

        public void EndGame(IEndGameContext context)
        {
            this.player.EndGame(context);
        }
    }
}
