﻿namespace TexasHoldem.Statistics.Indicators
{
    using TexasHoldem.Logic.Players;

    public class BBper100 : BaseIndicator
    {
        private int moneyLeftAtTheBeginningOfTheHand;

        private int smallBlind;

        public BBper100(int hands = 0)
            : base(hands)
        {
        }

        public BBper100(int hands, double totalBigBlindWon)
            : this(hands)
        {
            this.TotalBigBlindWon = totalBigBlindWon;
        }

        public double TotalBigBlindWon { get; private set; }

        /// <summary>
        /// Gets the average amount of big blinds won or lost per 100 hands
        /// </summary>
        /// <value>
        /// Big Blinds per 100 Hands
        /// </value>
        public double Amount
        {
            get
            {
                return this.Hands == 0 ? 0 : (double)this.TotalBigBlindWon / (this.Hands / 100.0);
            }
        }

        public override void StartHandExtract(IStartHandContext context)
        {
            base.StartHandExtract(context);
            this.moneyLeftAtTheBeginningOfTheHand = context.MoneyLeft;
            this.smallBlind = context.SmallBlind;
        }

        public override void EndHandExtract(IEndHandContext context)
        {
            var balance = context.MoneyLeft - this.moneyLeftAtTheBeginningOfTheHand;
            this.TotalBigBlindWon += (double)balance / (double)(this.smallBlind * 2);
        }

        public override string ToString()
        {
            return $"BBper100:{this.Amount:0.00}";
        }

        public override BaseIndicator DeepClone()
        {
            var copy = new BBper100(this.Hands, this.TotalBigBlindWon);
            copy.moneyLeftAtTheBeginningOfTheHand = this.moneyLeftAtTheBeginningOfTheHand;
            copy.smallBlind = this.smallBlind;
            return copy;
        }
    }
}