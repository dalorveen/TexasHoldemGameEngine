namespace TexasHoldem.Statistics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics.Extensions;
    using TexasHoldem.Statistics.Indicators;

    public class PositionalCollection<TIndicator> : IEnumerable<KeyValuePair<Positions, TIndicator>>, IUpdate
        where TIndicator : BaseIndicator<TIndicator>, new()
    {
        // http://hm2faq.holdemmanager.com/questions/2062/How+Does+Holdem+Manager+Calculate+Positions+Based+on+the+Number+of+Players+at+the+Table%3F
        private static Positions[][] positionChart = new Positions[][]
        {
            new Positions[] { Positions.SB, Positions.BB },
            new Positions[] { Positions.SB, Positions.BB, Positions.BTN },
            new Positions[] { Positions.SB, Positions.BB, Positions.CO, Positions.BTN },
            new Positions[] { Positions.SB, Positions.BB, Positions.MP, Positions.CO, Positions.BTN },
            new Positions[] { Positions.SB, Positions.BB, Positions.EP, Positions.MP, Positions.CO, Positions.BTN },
            new Positions[]
                {
                    Positions.SB,
                    Positions.BB,
                    Positions.EP,
                    Positions.EP,
                    Positions.MP,
                    Positions.CO,
                    Positions.BTN
                },
            new Positions[]
                {
                    Positions.SB,
                    Positions.BB,
                    Positions.EP,
                    Positions.EP,
                    Positions.MP,
                    Positions.MP,
                    Positions.CO,
                    Positions.BTN
                },
            new Positions[]
                {
                    Positions.SB,
                    Positions.BB,
                    Positions.EP,
                    Positions.EP,
                    Positions.EP,
                    Positions.MP,
                    Positions.MP,
                    Positions.CO,
                    Positions.BTN
                },
            new Positions[]
                {
                    Positions.SB,
                    Positions.BB,
                    Positions.EP,
                    Positions.EP,
                    Positions.EP,
                    Positions.MP,
                    Positions.MP,
                    Positions.MP,
                    Positions.CO,
                    Positions.BTN
                }
        };

        private readonly IDictionary<Positions, TIndicator> indicators;

        private int numberOfPlayers;

        public PositionalCollection()
        {
            var temp = Enum.GetValues(typeof(Positions)).Cast<Positions>();
            this.indicators = new Dictionary<Positions, TIndicator>(temp.Count());

            foreach (var item in temp)
            {
                this.indicators.Add(item, new TIndicator());
            }
        }

        public Positions CurrentPosition { get; private set; }

        public TIndicator StatsOfCurrentPosition()
        {
            // TODO:
            // CurrentPosition is not initialized before the start hand.
            // Do I need to throw an exception?
            try
            {
                return this.indicators[this.CurrentPosition];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public TIndicator IndicatorBy(Positions position)
        {
            try
            {
                return this.indicators[position];
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("Position is not found.", nameof(position));
            }
        }

        public TIndicator StatsForAllPositions()
        {
            return this.TotalStats();
        }

        public void Update(IStartGameContext context)
        {
            this.numberOfPlayers = context.PlayerNames.Count;
        }

        public void Update(IStartHandContext context)
        {
            this.CurrentPosition = positionChart[this.numberOfPlayers - 2][context.ActionPriority];
            this.indicators[this.CurrentPosition].Update(context);
        }

        public void Update(IStartRoundContext context)
        {
            this.indicators[this.CurrentPosition].Update(context);
        }

        public void Update(IGetTurnContext context, string playerName)
        {
            this.indicators[this.CurrentPosition].Update(context, playerName);
        }

        public void Update(IGetTurnContext context, PlayerAction playerAction, string playerName)
        {
            this.indicators[this.CurrentPosition].Update(context, playerAction, playerName);
        }

        public void Update(IEndRoundContext context)
        {
            this.indicators[this.CurrentPosition].Update(context);
        }

        public void Update(IEndHandContext context, string playerName)
        {
            this.indicators[this.CurrentPosition].Update(context, playerName);
        }

        public void Update(IEndGameContext context)
        {
            foreach (var item in this.indicators)
            {
                item.Value.Update(context);
            }
        }

        public IEnumerator<KeyValuePair<Positions, TIndicator>> GetEnumerator()
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
                sb.Append($"{item.Key} [{item.Value.Amount:N1}] | ");
            }

            return sb.Remove(sb.Length - 3, 3).ToString();
        }
    }
}