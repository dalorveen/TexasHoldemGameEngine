namespace TexasHoldem.Statistics.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class CallThreeBet : BaseIndicator<CallThreeBet>
    {
        private Dictionary<GameRoundType, int> totalTimesCalled3BetByStreet;
        private Dictionary<GameRoundType, int> totalTimesFaced3BetByStreet;

        public CallThreeBet()
            : base(0)
        {
            this.totalTimesCalled3BetByStreet = new Dictionary<GameRoundType, int>();
            this.totalTimesFaced3BetByStreet = new Dictionary<GameRoundType, int>();

            foreach (var item in Enum.GetValues(typeof(GameRoundType)).Cast<GameRoundType>())
            {
                this.totalTimesCalled3BetByStreet.Add(item, 0);
                this.totalTimesFaced3BetByStreet.Add(item, 0);
            }
        }

        public bool Faced3Bet { get; private set; }

        public int TotalTimesCalled3Bet
        {
            get
            {
                return this.totalTimesCalled3BetByStreet.Sum(s => s.Value);
            }
        }

        public int TotalTimesFaced3Bet
        {
            get
            {
                return this.totalTimesFaced3BetByStreet.Sum(s => s.Value);
            }
        }

        /// <summary>
        /// Gets the percentage of hands where the player called a 3-bet
        /// </summary>
        /// <value>Percentages called 3-bet</value>
        public override double Amount
        {
            get
            {
                return this.TotalTimesFaced3Bet == 0
                    ? 0 : ((double)this.TotalTimesCalled3Bet / (double)this.TotalTimesFaced3Bet) * 100.0;
            }
        }

        public double AmountByStreet(GameRoundType street)
        {
            return this.totalTimesFaced3BetByStreet[street] == 0
                    ? 0 : ((double)this.totalTimesCalled3BetByStreet[street]
                        / (double)this.totalTimesFaced3BetByStreet[street]) * 100.0;
        }

        public override void Update(IGetTurnContext context, IStatsContext statsContext)
        {
            var raises = context.PreviousRoundActions.Count(x => x.Round == context.RoundType
                && x.Action.Type == PlayerActionType.Raise);

            if (raises == (context.RoundType == GameRoundType.PreFlop ? 2 : 3))
            {
                this.totalTimesFaced3BetByStreet[context.RoundType]++;
                this.Faced3Bet = true;
            }
        }

        public override void Update(IGetTurnContext context, PlayerAction madeAction, IStatsContext statsContext)
        {
            if (this.Faced3Bet && madeAction.Type == PlayerActionType.CheckCall)
            {
                this.totalTimesCalled3BetByStreet[context.RoundType]++;
            }

            this.Faced3Bet = false;
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