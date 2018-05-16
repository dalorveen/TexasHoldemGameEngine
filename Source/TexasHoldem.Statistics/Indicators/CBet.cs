namespace TexasHoldem.Statistics.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class CBet : BaseIndicator<CBet>
    {
        private Dictionary<GameRoundType, int> totalTimesCBetByStreet;
        private Dictionary<GameRoundType, int> totalCBetOpportunitiesByStreet;

        public CBet()
            : base(0)
        {
            this.totalTimesCBetByStreet = new Dictionary<GameRoundType, int>();
            this.totalCBetOpportunitiesByStreet = new Dictionary<GameRoundType, int>();

            foreach (var item in Enum.GetValues(typeof(GameRoundType)).Cast<GameRoundType>())
            {
                this.totalTimesCBetByStreet.Add(item, 0);
                this.totalCBetOpportunitiesByStreet.Add(item, 0);
            }
        }

        public bool IsOpportunity { get; private set; }

        public int TotalTimesContinuationBet
        {
            get
            {
                return this.totalTimesCBetByStreet.Sum(s => s.Value);
            }
        }

        public int TotalContinuationBetOpportunities
        {
            get
            {
                return this.totalCBetOpportunitiesByStreet.Sum(s => s.Value);
            }
        }

        /// <summary>
        /// Gets the percentage of time a player followed their [pre-flop raise by betting the flop] OR
        /// [flop continuation bet by betting the turn (and bet again on the river) when they had the opportunity]
        /// </summary>
        /// <value>Percentages of continuation bet</value>
        public override double Amount
        {
            get
            {
                return this.TotalContinuationBetOpportunities == 0
                    ? 0
                    : ((double)this.TotalTimesContinuationBet
                        / (double)this.TotalContinuationBetOpportunities) * 100.0;
            }
        }

        public double AmountByStreet(GameRoundType street)
        {
            return this.totalCBetOpportunitiesByStreet[street] == 0
                    ? 0
                    : ((double)this.totalTimesCBetByStreet[street]
                        / (double)this.totalCBetOpportunitiesByStreet[street]) * 100.0;
        }

        public override void Update(IGetTurnContext context, IStatsContext statsContext)
        {
            if (context.RoundType != GameRoundType.PreFlop && context.MoneyToCall == 0)
            {
                var preflopRaiser = context.PreviousRoundActions
                    .Where(p => p.Round == GameRoundType.PreFlop)
                    .Cast<PlayerActionAndName?>()
                    .LastOrDefault(p => p.Value.Action.Type == PlayerActionType.Raise);

                if (preflopRaiser.HasValue && preflopRaiser.Value.PlayerName == statsContext.PlayerName)
                {
                    for (int i = 1; i < (int)context.RoundType; i++)
                    {
                        var playerWhoMadeOpeningBetInPreviousRound = context.PreviousRoundActions
                            .Where(p => p.Round == (GameRoundType)i)
                            .Cast<PlayerActionAndName?>()
                            .FirstOrDefault(p => p.Value.Action.Type == PlayerActionType.Raise);

                        if (!playerWhoMadeOpeningBetInPreviousRound.HasValue)
                        {
                            return;
                        }
                        else if (playerWhoMadeOpeningBetInPreviousRound.Value.PlayerName != statsContext.PlayerName)
                        {
                            return;
                        }
                    }

                    this.totalCBetOpportunitiesByStreet[context.RoundType]++;
                    this.IsOpportunity = true;
                }
            }
        }

        public override void Update(IGetTurnContext context, PlayerAction madeAction, IStatsContext statsContext)
        {
            if (this.IsOpportunity && madeAction.Type == PlayerActionType.Raise)
            {
                this.totalTimesCBetByStreet[context.RoundType]++;
            }

            this.IsOpportunity = false;
        }

        public override string ToString()
        {
            return this.ToStreetFormat(
                this.AmountByStreet(GameRoundType.Flop),
                this.AmountByStreet(GameRoundType.Turn),
                this.AmountByStreet(GameRoundType.River));
        }
    }
}
