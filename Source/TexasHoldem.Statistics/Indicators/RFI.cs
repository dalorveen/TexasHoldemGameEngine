namespace TexasHoldem.Statistics.Indicators
{
    using System;
    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class RFI : BaseIndicator, IAdd<RFI>
    {
        public RFI(int hands = 0)
            : base(hands)
        {
        }

        public RFI(int hands, int totalTimesRaisedFirstIn, int totalOpportunitiesToOpenThePot)
            : this(hands)
        {
            this.TotalTimesRaisedFirstIn = totalTimesRaisedFirstIn;
            this.TotalOpportunitiesToOpenThePot = totalOpportunitiesToOpenThePot;
        }

        public bool IsOpportunitiesToOpenThePot { get; private set; }

        public int TotalTimesRaisedFirstIn { get; private set; }

        public int TotalOpportunitiesToOpenThePot { get; private set; }

        /// <summary>
        /// Gets the percentage of times the player raised when they had the opportunity to open the pot
        /// </summary>
        /// <value>
        /// Raised First In
        /// </value>
        public double Percentage
        {
            get
            {
                return this.TotalOpportunitiesToOpenThePot == 0
                    ? 0 : ((double)this.TotalTimesRaisedFirstIn / (double)this.TotalOpportunitiesToOpenThePot) * 100.0;
            }
        }

        public override void GetTurnExtract(IGetTurnContext context)
        {
            if (context.RoundType == GameRoundType.PreFlop && context.CurrentPot <= context.SmallBlind * 3)
            {
                this.TotalOpportunitiesToOpenThePot++;
                this.IsOpportunitiesToOpenThePot = true;
            }
        }

        public override void MadeActionExtract(IGetTurnContext context, PlayerAction madeAction)
        {
            if (this.IsOpportunitiesToOpenThePot)
            {
                if (madeAction.Type == PlayerActionType.Raise)
                {
                    this.TotalTimesRaisedFirstIn++;
                }
            }

            this.IsOpportunitiesToOpenThePot = false;
        }

        public override string ToString()
        {
            return $"{this.Percentage:0.00}%";
        }

        public override BaseIndicator DeepClone()
        {
            var copy = new RFI(this.Hands, this.TotalTimesRaisedFirstIn, this.TotalOpportunitiesToOpenThePot);
            copy.IsOpportunitiesToOpenThePot = this.IsOpportunitiesToOpenThePot;
            return copy;
        }

        public RFI Add(RFI otherIndicator)
        {
            return new RFI(
                this.Hands + otherIndicator.Hands,
                this.TotalTimesRaisedFirstIn + otherIndicator.TotalTimesRaisedFirstIn,
                this.TotalOpportunitiesToOpenThePot + otherIndicator.TotalOpportunitiesToOpenThePot);
        }
    }
}