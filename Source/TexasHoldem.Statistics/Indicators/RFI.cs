namespace TexasHoldem.Statistics.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class RFI : BaseIndicator<RFI>
    {
        private Dictionary<Positions, int> totalTimesRaisedFirstInByPosition;
        private Dictionary<Positions, int> totalOpportunitiesToOpenThePotByPosition;

        public RFI()
            : base(0)
        {
            this.totalTimesRaisedFirstInByPosition = new Dictionary<Positions, int>();
            this.totalOpportunitiesToOpenThePotByPosition = new Dictionary<Positions, int>();

            foreach (var item in Enum.GetValues(typeof(Positions)).Cast<Positions>())
            {
                this.totalTimesRaisedFirstInByPosition.Add(item, 0);
                this.totalOpportunitiesToOpenThePotByPosition.Add(item, 0);
            }
        }

        public bool IsOpportunitiesToOpenThePot { get; private set; }

        public int TotalTimesRaisedFirstIn
        {
            get
            {
                return this.totalTimesRaisedFirstInByPosition.Sum(s => s.Value);
            }
        }

        public int TotalOpportunitiesToOpenThePot
        {
            get
            {
                return this.totalOpportunitiesToOpenThePotByPosition.Sum(s => s.Value);
            }
        }

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

        public double AmountByPosition(Positions position)
        {
            return this.totalOpportunitiesToOpenThePotByPosition[position] == 0
                    ? 0
                    : ((double)this.totalTimesRaisedFirstInByPosition[position]
                        / (double)this.totalOpportunitiesToOpenThePotByPosition[position]) * 100.0;
        }

        public override void Update(IGetTurnContext context, IStatsContext statsContext)
        {
            if (context.RoundType == GameRoundType.PreFlop && context.CurrentPot <= context.SmallBlind * 3)
            {
                this.totalOpportunitiesToOpenThePotByPosition[statsContext.Position.CurrentPosition]++;
                this.IsOpportunitiesToOpenThePot = true;
            }
        }

        public override void Update(IGetTurnContext context, PlayerAction madeAction, IStatsContext statsContext)
        {
            if (this.IsOpportunitiesToOpenThePot)
            {
                if (madeAction.Type == PlayerActionType.Raise)
                {
                    this.totalTimesRaisedFirstInByPosition[statsContext.Position.CurrentPosition]++;
                }
            }

            this.IsOpportunitiesToOpenThePot = false;
        }

        public override string ToString()
        {
            return this.ToPositionFormat(
                this.AmountByPosition(Positions.SB),
                this.AmountByPosition(Positions.BB),
                this.AmountByPosition(Positions.EP),
                this.AmountByPosition(Positions.MP),
                this.AmountByPosition(Positions.CO),
                this.AmountByPosition(Positions.BTN));
        }
    }
}