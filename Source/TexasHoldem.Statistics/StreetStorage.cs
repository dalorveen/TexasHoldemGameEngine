namespace TexasHoldem.Statistics
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics.Indicators;

    public class StreetStorage<T> : BaseIndicator
        where T : BaseIndicator
    {
        public StreetStorage(IDictionary<GameRoundType, T> indicatorByStreets, int hands = 0)
            : base(hands)
        {
            this.IndicatorByStreets = new Dictionary<GameRoundType, T>(indicatorByStreets);
        }

        public IReadOnlyDictionary<GameRoundType, T> IndicatorByStreets { get; }

        public override void StartHandExtract(IStartHandContext context)
        {
            base.StartHandExtract(context);

            if (this.IndicatorByStreets.ContainsKey(GameRoundType.PreFlop))
            {
                this.IndicatorByStreets[GameRoundType.PreFlop].StartHandExtract(context);
            }
        }

        public override void StartRoundExtract(IStartRoundContext context)
        {
            if (this.IndicatorByStreets.ContainsKey(context.RoundType))
            {
                this.IndicatorByStreets[context.RoundType].StartRoundExtract(context);
            }
        }

        public override void GetTurnExtract(IGetTurnContext context)
        {
            if (this.IndicatorByStreets.ContainsKey(context.RoundType))
            {
                this.IndicatorByStreets[context.RoundType].GetTurnExtract(context);
            }
        }

        public override void MadeActionExtract(IGetTurnContext context, PlayerAction madeAction)
        {
            if (this.IndicatorByStreets.ContainsKey(context.RoundType))
            {
                this.IndicatorByStreets[context.RoundType].MadeActionExtract(context, madeAction);
            }
        }

        public override void EndRoundExtract(IEndRoundContext context)
        {
            if (this.IndicatorByStreets.ContainsKey(context.CompletedRoundType))
            {
                this.IndicatorByStreets[context.CompletedRoundType].EndRoundExtract(context);
            }
        }

        public override void EndHandExtract(IEndHandContext context)
        {
            if (this.IndicatorByStreets.ContainsKey(context.LastGameRoundType))
            {
                this.IndicatorByStreets[context.LastGameRoundType].EndHandExtract(context);
            }
        }

        public T AllStreets()
        {
            var statByRounds = this.IndicatorByStreets.Values.ToArray();
            T sum = statByRounds[0];

            for (int i = 1; i < statByRounds.Count(); i++)
            {
                sum = ((IAdd<T>)statByRounds[i]).Add(sum);
            }

            return sum;
        }

        public override BaseIndicator DeepClone()
        {
            var copy = new Dictionary<GameRoundType, T>();

            foreach (var item in this.IndicatorByStreets)
            {
                copy.Add(item.Key, (T)item.Value.DeepClone());
            }

            return new StreetStorage<T>(copy, this.Hands);
        }

        public override string ToString()
        {
            var output = new StringBuilder();

            output.AppendFormat("[ALL|{0}] ", this.AllStreets());

            foreach (var item in this.IndicatorByStreets)
            {
                output.AppendFormat(" [{0}|{1}] ", item.Key, item.Value.ToString());
            }

            return output.ToString();
        }
    }
}
