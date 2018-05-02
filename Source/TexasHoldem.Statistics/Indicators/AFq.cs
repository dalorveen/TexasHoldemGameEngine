namespace TexasHoldem.Statistics.Indicators
{
    using TexasHoldem.Logic.Players;

    public class AFq : BaseIndicator<AFq>
    {
        public AFq()
            : base(0)
        {
        }

        public AFq(int hands, int totalTimesRaised, int totalTimesCalled, int totalTimesFolded)
            : base(hands)
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

        public override void Update(IGetTurnContext context, PlayerAction madeAction, string playerName)
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

        public override AFq DeepClone()
        {
            return new AFq(this.Hands, this.TotalTimesRaised, this.TotalTimesCalled, this.TotalTimesFolded);
        }

        public override AFq Sum(AFq other)
        {
            return new AFq(
                this.Hands + other.Hands,
                this.TotalTimesRaised + other.TotalTimesRaised,
                this.TotalTimesCalled + other.TotalTimesCalled,
                this.TotalTimesFolded + other.TotalTimesFolded);
        }
    }
}
