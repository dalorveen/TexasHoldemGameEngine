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

            if (outputSignals[0] > 0.5 && outputSignals[1] <= 0.5)
            {
                // A signal to rise
                var wager = this.GetTurnContext.MoneyLeft * outputSignals[2];
                var moneyToRaise = (int)wager - this.GetTurnContext.MoneyToCall;

                if (this.GetTurnContext.CanRaise)
                {
                    if (moneyToRaise >= this.GetTurnContext.MinRaise)
                    {
                        return PlayerAction.Raise(moneyToRaise);
                    }
                    else
                    {
                        return PlayerAction.Raise(this.GetTurnContext.MinRaise);
                    }
                }
                else
                {
                    return PlayerAction.CheckOrCall();
                }
            }
            else if (outputSignals[0] <= 0.5 && outputSignals[1] > 0.5)
            {
                // A signal to check or call
                return PlayerAction.CheckOrCall();
            }
            else
            {
                return PlayerAction.Fold();
            }
        }

        //public PlayerAction React()
        //{
        //    var outputSignals = this.OutputSignals();
        //
        //    var wager = this.GetTurnContext.MoneyLeft * outputSignals[0];
        //    var moneyToRaise = (int)wager - this.GetTurnContext.MoneyToCall;
        //
        //    if (this.GetTurnContext.CanRaise && moneyToRaise >= this.GetTurnContext.MinRaise)
        //    {
        //        return PlayerAction.Raise(moneyToRaise);
        //    }
        //    else if (moneyToRaise >= this.GetTurnContext.MoneyToCall)
        //    {
        //        return PlayerAction.CheckOrCall();
        //    }
        //    else
        //    {
        //        return PlayerAction.Fold();
        //    }
        //}
    }
}
