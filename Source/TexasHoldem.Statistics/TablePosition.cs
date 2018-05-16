namespace TexasHoldem.Statistics
{
    using TexasHoldem.Logic.Players;

    public class TablePosition
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

        private readonly int numberOfPlayers;

        public TablePosition(IStartGameContext context)
        {
            this.numberOfPlayers = context.PlayerNames.Count;
        }

        public Positions CurrentPosition { get; private set; }

        public void SetCurrentPosition(IStartHandContext context)
        {
            this.CurrentPosition = positionChart[this.numberOfPlayers - 2][context.ActionPriority];
        }
    }
}