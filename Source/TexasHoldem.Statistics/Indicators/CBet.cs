namespace TexasHoldem.Statistics.Indicators
{
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class CBet : BaseIndicator<CBet>
    {
        public CBet()
            : base(0)
        {
        }

        public CBet(int hands, int totalTimesContinuationBet, int totalContinuationBetOpportunities)
            : base(hands)
        {
            this.TotalTimesContinuationBet = totalTimesContinuationBet;
            this.TotalContinuationBetOpportunities = totalContinuationBetOpportunities;
        }

        public bool IsOpportunity { get; private set; }

        public int TotalTimesContinuationBet { get; private set; }

        public int TotalContinuationBetOpportunities { get; private set; }

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
                    : ((double)this.TotalTimesContinuationBet / (double)this.TotalContinuationBetOpportunities) * 100.0;
            }
        }

        public override void Update(IGetTurnContext context, string playerName)
        {
            if (context.RoundType != GameRoundType.PreFlop && context.MoneyToCall == 0)
            {
                var preflopRaiser = context.PreviousRoundActions
                    .Where(p => p.Round == GameRoundType.PreFlop)
                    .Cast<PlayerActionAndName?>()
                    .LastOrDefault(p => p.Value.Action.Type == PlayerActionType.Raise);

                if (preflopRaiser.HasValue && preflopRaiser.Value.PlayerName == playerName)
                {
                    for (int i = 1; i < (int)context.RoundType; i++)
                    {
                        var previousRoundFirstRaiser = context.PreviousRoundActions
                            .Where(p => p.Round == (GameRoundType)i)
                            .Cast<PlayerActionAndName?>()
                            .LastOrDefault(p => p.Value.Action.Type == PlayerActionType.Raise);

                        if (!previousRoundFirstRaiser.HasValue)
                        {
                            return;
                        }
                        else if (previousRoundFirstRaiser.Value.PlayerName != playerName)
                        {
                            return;
                        }
                    }

                    this.TotalContinuationBetOpportunities++;
                    this.IsOpportunity = true;
                }
            }
        }

        public override void Update(IGetTurnContext context, PlayerAction madeAction, string playerName)
        {
            if (this.IsOpportunity && madeAction.Type == PlayerActionType.Raise)
            {
                this.TotalTimesContinuationBet++;
            }

            this.IsOpportunity = false;
        }

        public override string ToString()
        {
            return $"{this.Amount:0.00}%";
        }

        public override CBet DeepClone()
        {
            var copy = new CBet(this.Hands, this.TotalTimesContinuationBet, this.TotalContinuationBetOpportunities);
            copy.IsOpportunity = this.IsOpportunity;
            return copy;
        }

        public override CBet Sum(CBet other)
        {
            return new CBet(
                this.Hands + other.Hands,
                this.TotalTimesContinuationBet + other.TotalTimesContinuationBet,
                this.TotalContinuationBetOpportunities + other.TotalContinuationBetOpportunities);
        }
    }
}
