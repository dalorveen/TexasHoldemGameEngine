namespace TexasHoldem.AI.NeuroPlayer.NeuralNetwork
{
    using TexasHoldem.Logic.Players;

    public class Reaction : Signal
    {
        public Reaction(IStartGameContext context)
            : base(context)
        {
        }

        public PlayerAction React()
        {
            var outputSignals = this.OutputSignals();

            if (outputSignals[0] > 0.75 && outputSignals[1] <= 0.25)
            {
                // A signal to rise
                var moneyToRaise = this.GetTurnContext.MinRaise + (int)(this.GetTurnContext.MoneyLeft * outputSignals[2]);

                if (this.GetTurnContext.CanRaise)
                {
                    if (moneyToRaise >= this.GetTurnContext.MoneyLeft - this.GetTurnContext.MoneyToCall)
                    {
                        // All-In
                        return PlayerAction.Raise(this.GetTurnContext.MoneyLeft - this.GetTurnContext.MoneyToCall);
                    }
                    else
                    {
                        return PlayerAction.Raise(moneyToRaise);
                    }
                }
                else
                {
                    return PlayerAction.CheckOrCall();
                }
            }
            else if (outputSignals[0] <= 0.25 && outputSignals[1] > 0.75)
            {
                // A signal to check or call
                return PlayerAction.CheckOrCall();
            }
            else
            {
                return PlayerAction.Fold();
            }
        }
    }
}
