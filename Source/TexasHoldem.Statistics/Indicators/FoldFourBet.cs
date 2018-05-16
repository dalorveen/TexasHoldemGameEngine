namespace TexasHoldem.Statistics.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class FoldFourBet : BaseIndicator<FoldFourBet>
    {
        private Dictionary<GameRoundType, int> totalTimesFoldedTo4BetByStreet;
        private Dictionary<GameRoundType, int> totalTimesFaced4BetByStreet;

        public FoldFourBet()
            : base(0)
        {
            this.totalTimesFoldedTo4BetByStreet = new Dictionary<GameRoundType, int>();
            this.totalTimesFaced4BetByStreet = new Dictionary<GameRoundType, int>();

            foreach (var item in Enum.GetValues(typeof(GameRoundType)).Cast<GameRoundType>())
            {
                this.totalTimesFoldedTo4BetByStreet.Add(item, 0);
                this.totalTimesFaced4BetByStreet.Add(item, 0);
            }
        }

        public bool Faced4Bet { get; private set; }

        public int TotalTimesFoldedTo4Bet
        {
            get
            {
                return this.totalTimesFoldedTo4BetByStreet.Sum(s => s.Value);
            }
        }

        public int TotalTimesFaced4Bet
        {
            get
            {
                return this.totalTimesFaced4BetByStreet.Sum(s => s.Value);
            }
        }

        /// <summary>
        /// Gets the percentage of times the player folded when facing a 4-bet
        /// </summary>
        /// <value>Percentages of fold to 4Bet</value>
        public override double Amount
        {
            get
            {
                return this.TotalTimesFaced4Bet == 0
                    ? 0 : ((double)this.TotalTimesFoldedTo4Bet / (double)this.TotalTimesFaced4Bet) * 100.0;
            }
        }

        public double AmountByStreet(GameRoundType street)
        {
            return this.totalTimesFaced4BetByStreet[street] == 0
                    ? 0 : ((double)this.totalTimesFoldedTo4BetByStreet[street]
                        / (double)this.totalTimesFaced4BetByStreet[street]) * 100.0;
        }

        public override void Update(IGetTurnContext context, IStatsContext statsContext)
        {
            var raises = context.PreviousRoundActions.Count(x => x.Round == context.RoundType
                && x.Action.Type == PlayerActionType.Raise);

            if (raises == (context.RoundType == GameRoundType.PreFlop ? 3 : 4))
            {
                this.totalTimesFaced4BetByStreet[context.RoundType]++;
                this.Faced4Bet = true;
            }
        }

        public override void Update(IGetTurnContext context, PlayerAction madeAction, IStatsContext statsContext)
        {
            if (this.Faced4Bet && madeAction.Type == PlayerActionType.Fold)
            {
                this.totalTimesFoldedTo4BetByStreet[context.RoundType]++;
            }

            this.Faced4Bet = false;
        }

        public override string ToString()
        {
            return this.ToStreetFormat(
                this.AmountByStreet(GameRoundType.PreFlop),
                this.AmountByStreet(GameRoundType.Flop),
                this.AmountByStreet(GameRoundType.Turn),
                this.AmountByStreet(GameRoundType.River));
        }
    }
}