namespace TexasHoldem.Statistics.Indicators
{
    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class RFI : BaseIndicator<RFI>
    {
        public RFI()
            : base(0)
        {
        }

        public RFI(int hands, int totalTimesRaisedFirstIn, int totalOpportunitiesToOpenThePot)
            : base(hands)
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
        public override double Amount
        {
            get
            {
                return this.TotalOpportunitiesToOpenThePot == 0
                    ? 0 : ((double)this.TotalTimesRaisedFirstIn / (double)this.TotalOpportunitiesToOpenThePot) * 100.0;
            }
        }

        public override void Update(IGetTurnContext context, string playerName)
        {
            if (context.RoundType == GameRoundType.PreFlop && context.CurrentPot <= context.SmallBlind * 3)
            {
                this.TotalOpportunitiesToOpenThePot++;
                this.IsOpportunitiesToOpenThePot = true;
            }
        }

        public override void Update(IGetTurnContext context, PlayerAction madeAction, string playerName)
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
            return $"{this.Amount:0.00}%";
        }

        public override RFI DeepClone()
        {
            var copy = new RFI(this.Hands, this.TotalTimesRaisedFirstIn, this.TotalOpportunitiesToOpenThePot);
            copy.IsOpportunitiesToOpenThePot = this.IsOpportunitiesToOpenThePot;
            return copy;
        }

        public override RFI Sum(RFI other)
        {
            return new RFI(
                this.Hands + other.Hands,
                this.TotalTimesRaisedFirstIn + other.TotalTimesRaisedFirstIn,
                this.TotalOpportunitiesToOpenThePot + other.TotalOpportunitiesToOpenThePot);
        }
    }
}