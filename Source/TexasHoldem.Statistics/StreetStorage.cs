namespace TexasHoldem.Statistics
{
    using System;

    using TexasHoldem.Logic;

    public class StreetStorage : IDeepCloneable<StreetStorage>
    {
        public StreetStorage(int preflop, int flop, int turn, int river)
        {
            this.PF = preflop;
            this.F = flop;
            this.T = turn;
            this.R = river;
        }

        public int PF { get; private set; }

        public int F { get; private set; }

        public int T { get; private set; }

        public int R { get; private set; }

        public int Total
        {
            get
            {
                return this.PF + this.F + this.T + this.R;
            }
        }

        public int this[GameRoundType street]
        {
            get
            {
                switch (street)
                {
                    case GameRoundType.PreFlop:
                        return this.PF;
                    case GameRoundType.Flop:
                        return this.F;
                    case GameRoundType.Turn:
                        return this.T;
                    case GameRoundType.River:
                        return this.R;
                    default:
                        return -1;
                }
            }
        }

        public StreetStorage DeepClone()
        {
            return new StreetStorage(this.PF, this.F, this.T, this.R);
        }
    }
}
