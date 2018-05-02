namespace TexasHoldem.Statistics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics.Indicators;

    public class SeveralStreets<TIndicator> : IEnumerable<KeyValuePair<GameRoundType, SingleStreet<TIndicator>>>,
        IUpdate
        where TIndicator : BaseIndicator<TIndicator>, new()
    {
        private readonly IDictionary<GameRoundType, SingleStreet<TIndicator>> indicators;

        private GameRoundType currentStreet;

        public SeveralStreets(ICollection<GameRoundType> excludedStreets, ICollection<Positions> excludedPositions)
        {
            var temp = Enum.GetValues(typeof(GameRoundType)).Cast<GameRoundType>().Except(excludedStreets);
            this.indicators = new Dictionary<GameRoundType, SingleStreet<TIndicator>>(temp.Count());

            foreach (var item in temp)
            {
                this.indicators.Add(item, new SingleStreet<TIndicator>(excludedPositions));
            }
        }

        public TIndicator CurrentPositionIndicatorBy(GameRoundType street)
        {
            try
            {
                return this.indicators[street].CurrentPosition();
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("Street is not found.", nameof(street));
            }
        }

        public TIndicator IndicatorBy(GameRoundType street, Positions position)
        {
            try
            {
                return this.indicators[street].IndicatorBy(position);
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("Street is not found.", nameof(street));
            }
        }

        public TIndicator Total()
        {
            var temp = new TIndicator();

            foreach (var item in this.indicators)
            {
                temp.Sum(item.Value.Total());
            }

            return temp;
        }

        public TIndicator TotalBy(GameRoundType street)
        {
            try
            {
                return this.indicators[street].Total();
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
            foreach (var item in this.indicators)
            {
                item.Value.Update(context);
            }
        }

        public void Update(IStartRoundContext context)
        {
            this.currentStreet = context.RoundType;
            this.indicators[this.currentStreet].Update(context);
        }

        public void Update(IGetTurnContext context, string playerName)
        {
            this.indicators[this.currentStreet].Update(context, playerName);
        }

        public void Update(IGetTurnContext context, PlayerAction playerAction, string playerName)
        {
            this.indicators[this.currentStreet].Update(context, playerAction, playerName);
        }

        public void Update(IEndRoundContext context)
        {
            this.indicators[this.currentStreet].Update(context);
        }

        public void Update(IEndHandContext context, string playerName)
        {
            this.indicators[this.currentStreet].Update(context, playerName);
        }

        public void Update(IEndGameContext context)
        {
            foreach (var item in this.indicators)
            {
                item.Value.Update(context);
            }
        }

        public IEnumerator<KeyValuePair<GameRoundType, SingleStreet<TIndicator>>> GetEnumerator()
        {
            return (IEnumerator<KeyValuePair<GameRoundType, SingleStreet<TIndicator>>>)this.indicators;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}