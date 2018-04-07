namespace TexasHoldem.AI.NeuroPlayer.NeuralNetwork
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using HandEvaluatorExtension;
    using HoldemHand;
    using SharpNeat.Phenomes;
    using TexasHoldem.Logic.Players;

    public class Signal
    {
        private readonly Normalization.Normalization normalization;

        private ICardAdapter pocket;

        private bool isUpdated;

        public Signal(IStartGameContext context)
        {
            this.normalization = new Normalization.Normalization(context);
        }

        protected IGetTurnContext GetTurnContext { get; private set; }

        protected IBlackBox BlackBox { get; private set; }

        public void Update(ICardAdapter pocket, ICardAdapter communityCards, IGetTurnContext context, IBlackBox box)
        {
            this.pocket = pocket;
            this.normalization.Update(pocket, communityCards, context);
            this.GetTurnContext = context;
            this.BlackBox = box;
            this.isUpdated = true;
        }

        public IList<double> InputSignals()
        {
            var input = new List<double>();
            var hero = this.normalization.Hero();
            var opponents = this.normalization.Opponents();

            input.AddRange(this.normalization.HandStrength());
            input.AddRange(this.normalization.Draw());
            input.Add(this.normalization.NormalizedPot());
            input.AddRange(this.StreetToVector()); //input.AddRange(this.StreetToBinary());
            //input.AddRange(this.StartingHandsToVector()); //input.AddRange(this.StartingHandsToBinary());
            input.Add(hero.Money);
            input.Add(hero.CurrentRoundBet);
            input.Add(hero.Position);

            foreach (var item in opponents)
            {
                input.Add(item.Money);
                input.Add(item.CurrentRoundBet);
                input.Add(item.InHand);
            }

            return input;
        }

        public IList<double> OutputSignals()
        {
            if (this.BlackBox == null)
            {
                // Out1 = raise
                // Out2 = check or call
                // Out3 = size of raise
                return new List<double> { 0, 0, 0 };
            }
            else
            {
                ISignalArray inputSignalArray = this.BlackBox.InputSignalArray;
                ISignalArray outputSignalArray = this.BlackBox.OutputSignalArray;

                this.BlackBox.ResetState();

                var inputSignals = this.InputSignals();

                for (int i = 0; i < inputSignals.Count; i++)
                {
                    Debug.Assert(inputSignals[i] >= -1.0 && inputSignals[i] <= 1, "Outside the range of acceptable values");
                    inputSignalArray[i] = inputSignals[i];
                }

                this.BlackBox.Activate();

                if (!this.BlackBox.IsStateValid)
                {
                    // Any black box that gets itself into an invalid state is unlikely to be any good, so lets just bail out here
                    // TODO: how to return a zero fitness
                    throw new System.Exception();
                }

                return new List<double> { outputSignalArray[0], outputSignalArray[1], outputSignalArray[2] };
            }
        }

        private double[] StreetToVector()
        {
            var vector = new double[3];

            if (!this.isUpdated)
            {
                return vector;
            }
            else
            {
                var roundType = (int)this.GetTurnContext.RoundType;

                if (roundType != 0)
                {
                    vector[roundType - 1] = 1.0;
                }

                return vector;
            }
        }

        private double[] StreetToBinary()
        {
            var arrayLength = 2;

            if (!this.isUpdated)
            {
                return new double[arrayLength];
            }
            else
            {
                var n = (int)this.GetTurnContext.RoundType;

                return this.IntToBinary(n, arrayLength);
            }
        }

        private double[] StartingHandsToVector()
        {
            var vector = new double[8];

            if (!this.isUpdated)
            {
                return vector;
            }
            else
            {
                var group = (int)PocketHands.GroupType(this.pocket.Mask);

                if (group != 8)
                {
                    vector[group] = 1.0;
                }

                return vector;
            }
        }

        private double[] StartingHandsToBinary()
        {
            var arrayLength = 4;

            if (!this.isUpdated)
            {
                return new double[arrayLength];
            }
            else
            {
                var n = (int)PocketHands.GroupType(this.pocket.Mask);

                if (n == 8)
                {
                    var ranks = Hand.Cards(this.pocket.Mask).Select(s => Hand.CardRank(Hand.ParseCard(s)));

                    var isContainedAce = Hand.Cards(this.pocket.Mask)
                        .Select(s => Hand.CardRank(Hand.ParseCard(s)) == Hand.RankAce)
                        .Any(p => p == true);

                    if (ranks.Contains(Hand.RankAce))
                    {
                        n = 8;
                    }
                    else if (ranks.Contains(Hand.RankKing))
                    {
                        n = 9;
                    }
                    else if (Hand.IsSuited(this.pocket.Mask))
                    {
                        var gap = Hand.GapCount(this.pocket.Mask);

                        Debug.Assert(gap != 0 && gap != 1, "It is expected that the gap is 2, 3, -1");

                        if (gap == 2)
                        {
                            n = 10;
                        }
                        else if (gap == 3)
                        {
                            n = 11;
                        }
                        else
                        {
                            n = 12;
                        }
                    }
                    else
                    {
                        var gap = Hand.GapCount(this.pocket.Mask);

                        if (gap == 0)
                        {
                            // 43o, 32o
                            n = 13;
                        }
                        else if (gap == 1)
                        {
                            n = 14;
                        }
                        else
                        {
                            n = 15;
                        }
                    }
                }

                return this.IntToBinary(n, arrayLength);
            }
        }

        private double[] IntToBinary(int number, int arrayLength)
        {
            string convertedString = System.Convert.ToString(number, 2);

            double[] bitArray = convertedString.PadLeft(arrayLength, '0')
                .Select(c => double.Parse(c.ToString()))
                .ToArray();

            return bitArray;
        }
    }
}
