namespace TexasHoldem.Statistics
{
    public class StreetStorage
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
    }
}
