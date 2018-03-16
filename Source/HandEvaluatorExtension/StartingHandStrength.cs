namespace HandEvaluatorExtension
{
    using System.Collections.Generic;
    using System.Linq;

    public class StartingHandStrength
    {
        public static readonly IReadOnlyDictionary<string, HoldemHand.PocketHands> SortedFromStrongToWeak;

        static StartingHandStrength()
        {
            var strength = new Dictionary<string, double>();

            foreach (ulong item in HoldemHand.PocketHands.Hands169())
            {
                var pocketHand169Str = HoldemHand.Hand.PocketHand169Type(item).ToString().Replace("Pocket", string.Empty);

                strength.Add(pocketHand169Str, HoldemHand.PocketHands.WinOdds(item));
            }

            var pockets = new Dictionary<string, HoldemHand.PocketHands>();

            foreach (var item in strength.OrderByDescending(k => k.Value))
            {
                pockets.Add(item.Key, HoldemHand.PocketHands.PocketCards169(item.Key));
            }

            SortedFromStrongToWeak = new Dictionary<string, HoldemHand.PocketHands>(pockets);
        }

        public static int StrengthIndex(ulong pocket)
        {
            return SortedFromStrongToWeak.TakeWhile(p => !p.Value.Contains(pocket)).Count();
        }
    }
}
