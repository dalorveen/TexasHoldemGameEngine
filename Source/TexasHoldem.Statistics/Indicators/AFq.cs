namespace TexasHoldem.Statistics.Indicators
{
    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class AFq : BaseIndicator, IAdd<AFq>
    {
        public AFq(int hands = 0)
            : base(hands)
        {
        }

        public AFq(int hands, int totalTimesRaised, int totalTimesCalled, int totalTimesFolded)
            : this(hands)
        {
            this.TotalTimesRaised = totalTimesRaised;
            this.TotalTimesCalled = totalTimesCalled;
            this.TotalTimesFolded = totalTimesFolded;
        }

        public int TotalTimesRaised { get; private set; }

        public int TotalTimesCalled { get; private set; }

        public int TotalTimesFolded { get; private set; }

        /// <summary>
        /// Gets the measure of how frequently a player is aggressive
        /// </summary>
        /// <value>Percentages of aggression frequency</value>
        public double Percentage
        {
            get
            {
                return this.TotalTimesRaised == 0
                    ? 0
                    : ((double)this.TotalTimesRaised /
                        (double)(this.TotalTimesRaised + this.TotalTimesCalled + this.TotalTimesFolded)) * 100.0;
            }
        }

        public override void MadeActionExtract(IGetTurnContext context, PlayerAction madeAction)
        {
            if (madeAction.Type == PlayerActionType.Raise)
            {
                this.TotalTimesRaised++;
            }
            else if (madeAction.Type == PlayerActionType.CheckCall)
            {
                this.TotalTimesCalled++;
            }
            else if (madeAction.Type == PlayerActionType.Fold)
            {
                this.TotalTimesFolded++;
            }
        }

        public override string ToString()
        {
            return $"{this.Percentage:0.00}%";
        }

        public override BaseIndicator DeepClone()
        {
            return new AFq(this.Hands, this.TotalTimesRaised, this.TotalTimesCalled, this.TotalTimesFolded);
        }

        public AFq Add(AFq otherIndicator)
        {
            return new AFq(
                this.Hands + otherIndicator.Hands,
                this.TotalTimesRaised + otherIndicator.TotalTimesRaised,
                this.TotalTimesCalled + otherIndicator.TotalTimesCalled,
                this.TotalTimesFolded + otherIndicator.TotalTimesFolded);
        }
    }
}
