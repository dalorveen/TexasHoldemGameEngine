namespace TexasHoldem.Statistics.Indicators
{
    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class WWSF : BaseIndicator<WWSF>
    {
        private bool didThePlayerSeeTheFlop;

        private int moneyInTheBeginningOfTheHand;

        public WWSF()
            : base(0)
        {
        }

        public WWSF(int hands, int totalTimesWonMoneyAfterSeeingTheFlop, int totalTimesSawTheFlop)
            : base(hands)
        {
            this.TotalTimesWonMoneyAfterSeeingTheFlop = totalTimesWonMoneyAfterSeeingTheFlop;
            this.TotalTimesSawTheFlop = totalTimesSawTheFlop;
        }

        public int TotalTimesWonMoneyAfterSeeingTheFlop { get; private set; }

        public int TotalTimesSawTheFlop { get; private set; }

        /// <summary>
        /// Gets The percentage of times the player won money after seeing the flop
        /// </summary>
        /// <value>
        /// Won Money When Saw the Flop
        /// </value>
        public override double Amount
        {
            get
            {
                return this.TotalTimesSawTheFlop == 0
                    ? 0
                    : ((double)this.TotalTimesWonMoneyAfterSeeingTheFlop / (double)this.TotalTimesSawTheFlop) * 100.0;
            }
        }

        public override void Update(IStartHandContext context)
        {
            base.Update(context);
            this.moneyInTheBeginningOfTheHand = context.MoneyLeft;
        }

        public override void Update(IGetTurnContext context, string playerName)
        {
            if (context.RoundType == GameRoundType.Flop)
            {
                this.TotalTimesSawTheFlop++;
                this.didThePlayerSeeTheFlop = true;
            }
        }

        public override void Update(IEndHandContext context, string playerName)
        {
            if (this.didThePlayerSeeTheFlop)
            {
                var balance = context.MoneyLeft - this.moneyInTheBeginningOfTheHand;

                this.TotalTimesWonMoneyAfterSeeingTheFlop += balance > 0 ? 1 : 0;
            }

            this.didThePlayerSeeTheFlop = false;
        }

        public override string ToString()
        {
            return $"{this.Amount:0.00}%";
        }

        public override WWSF DeepClone()
        {
            return new WWSF(this.Hands, this.TotalTimesWonMoneyAfterSeeingTheFlop, this.TotalTimesSawTheFlop);
        }

        public override WWSF Sum(WWSF other)
        {
            return new WWSF(
                this.Hands + other.Hands,
                this.TotalTimesWonMoneyAfterSeeingTheFlop + other.TotalTimesWonMoneyAfterSeeingTheFlop,
                this.TotalTimesSawTheFlop + other.TotalTimesSawTheFlop);
        }
    }
}