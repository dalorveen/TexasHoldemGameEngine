namespace TexasHoldem.Statistics.Indicators
{
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class WWSF : BaseIndicator
    {
        private bool didThePlayerSeeTheFlop;

        private int moneyInTheBeginningOfTheHand;

        public WWSF(int hands = 0)
            : base(hands)
        {
        }

        public WWSF(int hands, int totalTimesWonMoneyAfterSeeingTheFlop, int totalTimesSawTheFlop)
            : this(hands)
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
        public double Percentage
        {
            get
            {
                return this.TotalTimesSawTheFlop == 0
                    ? 0 : ((double)this.TotalTimesWonMoneyAfterSeeingTheFlop / (double)this.TotalTimesSawTheFlop) * 100.0;
            }
        }

        public override void StartHandExtract(IStartHandContext context)
        {
            base.StartHandExtract(context);

            this.moneyInTheBeginningOfTheHand = context.MoneyLeft;
        }

        public override void GetTurnExtract(IGetTurnContext context)
        {
            if (context.RoundType == GameRoundType.Flop)
            {
                this.TotalTimesSawTheFlop++;
                this.didThePlayerSeeTheFlop = true;
            }
        }

        public override void EndHandExtract(IEndHandContext context)
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
            return $"{this.Percentage:0.00}%";
        }

        public override BaseIndicator DeepClone()
        {
            return new WWSF(this.Hands, this.TotalTimesWonMoneyAfterSeeingTheFlop, this.TotalTimesSawTheFlop);
        }
    }
}