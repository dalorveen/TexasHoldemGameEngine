namespace TexasHoldem.Statistics
{
    using TexasHoldem.Logic;

    public interface IStats
    {
        double VPIP { get; }

        double PFR { get; }

        double AF { get; }

        double BBPer100 { get; }

        Proportion ThreeBet { get; }

        Proportion FourBetAndMore { get; }
    }
}
