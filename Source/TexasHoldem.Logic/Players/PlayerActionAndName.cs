namespace TexasHoldem.Logic.Players
{
    public struct PlayerActionAndName
    {
        public PlayerActionAndName(string playerName, PlayerAction action, GameRoundType round)
        {
            this.PlayerName = playerName;
            this.Action = action;
            this.Round = round;
        }

        public string PlayerName { get; }

        public PlayerAction Action { get; }

        public GameRoundType Round { get; }
    }
}
