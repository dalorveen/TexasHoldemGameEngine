namespace TexasHoldem.Statistics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics.Indicators;

    public class StreetCollection<TIndicator>
        : IEnumerable<KeyValuePair<GameRoundType, PositionalCollection<TIndicator>>>,
        IUpdate
        where TIndicator : BaseIndicator<TIndicator>, new()
    {
        private readonly IDictionary<GameRoundType, PositionalCollection<TIndicator>> indicators;

        public StreetCollection()
        {
            var temp = Enum.GetValues(typeof(GameRoundType)).Cast<GameRoundType>();
            this.indicators = new Dictionary<GameRoundType, PositionalCollection<TIndicator>>(temp.Count());

            foreach (var item in temp)
            {
                this.indicators.Add(item, new PositionalCollection<TIndicator>());
            }
        }

        public GameRoundType CurrentStreet { get; private set; }

        public TIndicator Total()
        {
            var temp = new TIndicator();

            foreach (var item in this.indicators)
            {
                temp.Sum(item.Value.StatsForAllPositions());
            }

            return temp;
        }

        public PositionalCollection<TIndicator> StatsOfCurrentStreet()
        {
            return this.GetStatsBy(this.CurrentStreet);
        }

        public PositionalCollection<TIndicator> GetStatsBy(GameRoundType street)
        {
            try
            {
                return this.indicators[street];
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("Street is not found.", nameof(street));
            }
        }

        public void Update(IStartGameContext context)
        {
            foreach (var item in this.indicators)
            {
                item.Value.Update(context);
            }
        }

        public void Update(IStartHandContext context)
        {
            this.CurrentStreet = GameRoundType.PreFlop;

            foreach (var item in this.indicators)
            {
                item.Value.Update(context);
            }
        }

        public void Update(IStartRoundContext context)
        {
            this.CurrentStreet = context.RoundType;
            this.indicators[this.CurrentStreet].Update(context);
        }

        public void Update(IGetTurnContext context, string playerName)
        {
            this.indicators[this.CurrentStreet].Update(context, playerName);
        }

        public void Update(IGetTurnContext context, PlayerAction playerAction, string playerName)
        {
            this.indicators[this.CurrentStreet].Update(context, playerAction, playerName);
        }

        public void Update(IEndRoundContext context)
        {
            this.indicators[this.CurrentStreet].Update(context);
        }

        public void Update(IEndHandContext context, string playerName)
        {
            this.indicators[this.CurrentStreet].Update(context, playerName);
        }

        public void Update(IEndGameContext context)
        {
            foreach (var item in this.indicators)
            {
                item.Value.Update(context);
            }
        }

        public IEnumerator<KeyValuePair<GameRoundType, PositionalCollection<TIndicator>>> GetEnumerator()
        {
            return this.indicators.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var item in this.indicators)
            {
                sb.Append($"{item.Key} [{item.Value.ToString()}]\n");
            }

            return sb.Remove(sb.Length - 2, 2).ToString();
        }

        public string ToSimplifiedString()
        {
            var sb = new StringBuilder();

            foreach (var item in this.indicators)
            {
                sb.Append($"{item.Key} [{item.Value.StatsForAllPositions().ToString()}] \n");
            }

            return sb.Remove(sb.Length - 2, 2).ToString();
        }
    }
}