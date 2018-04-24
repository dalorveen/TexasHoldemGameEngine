namespace TexasHoldem.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics.Indicators;

    public class PositionStorage<T> : BaseIndicator
        where T : BaseIndicator
    {
        private static SeatNames[][] availableSeatNames = new SeatNames[][]
        {
            new SeatNames[] { SeatNames.SB, SeatNames.BB },
            new SeatNames[] { SeatNames.SB, SeatNames.BB, SeatNames.BTN },
            new SeatNames[] { SeatNames.SB, SeatNames.BB, SeatNames.CO, SeatNames.BTN },
            new SeatNames[] { SeatNames.SB, SeatNames.BB, SeatNames.MP, SeatNames.CO, SeatNames.BTN },
            new SeatNames[] { SeatNames.SB, SeatNames.BB, SeatNames.UTG, SeatNames.MP, SeatNames.CO, SeatNames.BTN },
            new SeatNames[]
                {
                    SeatNames.SB,
                    SeatNames.BB,
                    SeatNames.UTG,
                    SeatNames.MP,
                    SeatNames.MP1,
                    SeatNames.CO,
                    SeatNames.BTN
                },
            new SeatNames[]
                {
                    SeatNames.SB,
                    SeatNames.BB,
                    SeatNames.UTG,
                    SeatNames.UTG1,
                    SeatNames.MP,
                    SeatNames.MP1,
                    SeatNames.CO,
                    SeatNames.BTN
                },
            new SeatNames[]
                {
                    SeatNames.SB,
                    SeatNames.BB,
                    SeatNames.UTG,
                    SeatNames.UTG1,
                    SeatNames.MP,
                    SeatNames.MP1,
                    SeatNames.MP2,
                    SeatNames.CO,
                    SeatNames.BTN
                },
            new SeatNames[]
                {
                    SeatNames.SB,
                    SeatNames.BB,
                    SeatNames.UTG,
                    SeatNames.UTG1,
                    SeatNames.UTG2,
                    SeatNames.MP,
                    SeatNames.MP1,
                    SeatNames.MP2,
                    SeatNames.CO,
                    SeatNames.BTN
                }
        };

        private readonly string playerName;

        private int numberOfPlayers;

        public PositionStorage(string playerName, Dictionary<SeatNames, T> indicatorByPositions, int hands = 0)
            : base(hands)
        {
            this.playerName = playerName;

            this.IndicatorByPositions = new Dictionary<SeatNames, T>();

            foreach (var item in indicatorByPositions)
            {
                this.IndicatorByPositions.Add(item.Key, (T)item.Value.DeepClone());
            }
        }

        public IDictionary<SeatNames, T> IndicatorByPositions { get; }

        public SeatNames? CurrentPosition { get; private set; }

        public override void StartGameExtract(IStartHandContext context)
        {
            this.CurrentPosition = null;
        }

        public override void StartHandExtract(IStartHandContext context)
        {
            base.StartHandExtract(context);

            this.numberOfPlayers = 4; // TODO: fix!
            var actionPriority = context.ActionPriority == 10 ? (this.numberOfPlayers - 1) : context.ActionPriority;
            this.CurrentPosition = availableSeatNames[this.numberOfPlayers - 2][actionPriority];

            this.IndicatorByPositions[this.CurrentPosition.Value].StartHandExtract(context);
        }

        public override void StartRoundExtract(IStartRoundContext context)
        {
            this.IndicatorByPositions[this.CurrentPosition.Value].StartRoundExtract(context);
        }

        public override void GetTurnExtract(IGetTurnContext context)
        {
            this.IndicatorByPositions[this.CurrentPosition.Value].GetTurnExtract(context);
        }

        public override void MadeActionExtract(IGetTurnContext context, PlayerAction madeAction)
        {
            this.IndicatorByPositions[this.CurrentPosition.Value].MadeActionExtract(context, madeAction);
        }

        public override void EndRoundExtract(IEndRoundContext context)
        {
            this.IndicatorByPositions[this.CurrentPosition.Value].EndRoundExtract(context);
        }

        public override void EndHandExtract(IEndHandContext context)
        {
            this.IndicatorByPositions[this.CurrentPosition.Value].EndHandExtract(context);
        }

        public T AllPositions()
        {
            var statByPositions = this.IndicatorByPositions.Values.ToArray();
            T sum = statByPositions[0];

            for (int i = 1; i < statByPositions.Count(); i++)
            {
                sum = ((IAdd<T>)statByPositions[i]).Add(sum);
            }

            return sum;
        }

        public override BaseIndicator DeepClone()
        {
            var copy = new PositionStorage<T>(
                this.playerName, (Dictionary<SeatNames, T>)this.IndicatorByPositions, this.Hands);

            copy.CurrentPosition = this.CurrentPosition;
            copy.numberOfPlayers = this.numberOfPlayers;

            return copy;
        }

        public override string ToString()
        {
            var output = new StringBuilder();

            output.AppendFormat("[ALL|{0}] ", this.AllPositions());

            foreach (var item in this.IndicatorByPositions)
            {
                output.AppendFormat(" [{0}|{1}] ", item.Key, item.Value.ToString());
            }

            return output.ToString();
        }
    }
}
