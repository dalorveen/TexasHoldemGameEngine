namespace TexasHoldem.Statistics
{
    public struct Proportion
    {
        public Proportion(double preflop, double flop, double turn, double river)
        {
            this.PF = preflop;
            this.F = flop;
            this.T = turn;
            this.R = river;
        }

        public double PF { get; }

        public double F { get; }

        public double T { get; }

        public double R { get; }
    }
}
