namespace TexasHoldem.Statistics.Extensions
{
    using System.Collections.Generic;

    using TexasHoldem.Statistics.Indicators;

    public static class StatsExtensions
    {
        public static TIndicator TotalStats<TIndicator>(this IEnumerable<KeyValuePair<Positions, TIndicator>> source)
            where TIndicator : BaseIndicator<TIndicator>, new()
        {
            var total = new TIndicator();

            foreach (var item in source)
            {
                total.Sum(item.Value);
            }

            return total;
        }
    }
}