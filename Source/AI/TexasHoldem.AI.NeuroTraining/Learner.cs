namespace TexasHoldem.AI.NeuroTraining
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Xml;

    using SharpNeat.Core;
    using SharpNeat.EvolutionAlgorithms;
    using SharpNeat.Genomes.Neat;
    using SharpNeat.Phenomes;
    using TexasHoldem.AI.NeuroPlayer;
    using TexasHoldem.AI.NeuroPlayer.NeuralNetwork;
    using TexasHoldem.Logic.Players;

    public class Learner : NeuroPlayer
    {
        private Thread sharpNeatThread;

        private NeatEvolutionAlgorithm<NeatGenome> ea;

        private SharpNeatPoker.Evaluator evaluator;

        private int startMoney;

        private int handsPlayed;

        private int profit;

        private bool isTrainingPause;

        private bool isTrainingContinuing;

        private uint bestAgentId;

        public override string Name { get; } = "Learner_" + Guid.NewGuid();

        public override int BuyIn { get; } = -1;

        public override void StartGame(IStartGameContext context)
        {
            base.StartGame(context);

            var signal = new Signal(context);
            var populationSize = 2000UL;

            this.isTrainingContinuing = true;

            SharpNeatPoker.Experiment experiment;

            if (true)
            {
                experiment = new SharpNeatPoker.Experiment(@"..\..\PopulationFiles\allAgents.xml");
            }
            else
            {
                experiment = new SharpNeatPoker.Experiment(
                    signal.InputSignals().Count, signal.OutputSignals().Count, populationSize);
            }

            this.evaluator = experiment.Evaluator;
            this.evaluator.StartGame(populationSize);

            this.sharpNeatThread = new Thread(() => this.SharpNeatThreadStart(experiment));
            this.sharpNeatThread.Start();
        }

        public override void StartHand(IStartHandContext context)
        {
            base.StartHand(context);

            if (this.isTrainingContinuing)
            {
                this.evaluator.StartHand(context);
            }
            else
            {
                this.startMoney = context.MoneyLeft;
            }
        }

        public override void StartRound(IStartRoundContext context)
        {
            base.StartRound(context);

            if (this.isTrainingContinuing)
            {
            }
        }

        public override PlayerAction GetTurn(IGetTurnContext context)
        {
            var currentAction = base.GetTurn(context);

            if (this.isTrainingContinuing)
            {
                this.evaluator.Turn();
            }

            return currentAction;
        }

        public override void EndHand(IEndHandContext context)
        {
            base.EndHand(context);

            if (this.isTrainingContinuing)
            {
                this.evaluator.EndHand(context);

                if (this.isTrainingPause)
                {
                    this.isTrainingContinuing = false;
                }
            }
            else
            {
                this.handsPlayed++;
                this.profit += context.MoneyLeft - this.startMoney;

                if (this.handsPlayed % 10000 == 0)
                {
                    this.isTrainingPause = false;
                    this.isTrainingContinuing = true;
                }
            }

            if (this.ea != null && this.ea.RunState == RunState.Paused)
            {
                var xwSettings = new XmlWriterSettings();

                xwSettings.Indent = true;

                var pathToBestAgent = @"..\..\PopulationFiles\bestAgent.xml";
                var pathToAllAgents = @"..\..\PopulationFiles\allAgents.xml";

                using (XmlWriter xw = XmlWriter.Create(@"..\..\PopulationFiles\bestAgent.xml", xwSettings))
                {
                    NeatGenomeXmlIO.WriteComplete(xw, this.ea.CurrentChampGenome, false);
                    Console.Write($"Player's training is interrupted.\nThe best agent is saved in {pathToBestAgent}\n");
                }

                using (XmlWriter xw = XmlWriter.Create(@"..\..\PopulationFiles\allAgents.xml", xwSettings))
                {
                    NeatGenomeXmlIO.WriteComplete(xw, this.ea.GenomeList, false);
                    Console.Write($"The list of all agents is saved to {pathToAllAgents}\n");
                }

                this.ea.Dispose();
                this.sharpNeatThread.Abort();
                this.ea = null;
            }
        }

        public override IBlackBox Phenome()
        {
            if (this.isTrainingContinuing)
            {
                return this.evaluator.GetCurrentPhenome();
            }
            else
            {
                if (this.ea != null)
                {
                    return (IBlackBox)this.ea.CurrentChampGenome.CachedPhenome;
                }
                else
                {
                    return this.evaluator.GetCurrentPhenome();
                }
            }
        }

        private void SharpNeatThreadStart(SharpNeatPoker.Experiment experiment)
        {
            // Create evolution algorithm and attach update event.
            this.ea = experiment.CreateEvolutionAlgorithm(experiment.GenomeFactory, experiment.GenomeList);
            this.ea.UpdateEvent += new EventHandler(this.EvolutionAlgorithmUpdateEvent);

            // Start algorithm (it will run on a background thread).
            this.ea.StartContinue();
        }

        private void EvolutionAlgorithmUpdateEvent(object sender, EventArgs e)
        {
            var temp = sender as NeatEvolutionAlgorithm<NeatGenome>;

            if (temp.CurrentChampGenome.Id != this.bestAgentId)
            {
                this.bestAgentId = temp.CurrentChampGenome.Id;
                this.handsPlayed = 0;
                this.profit = 0;
            }

            var str = $"{DateTime.Now.ToLongTimeString()} ...\n" +
                $"\t[generation: {temp.Statistics._generation}] [runState: {temp.RunState}]\n";

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(str);

            str = $"\t[best agent fitness: {temp.Statistics._maxFitness}]\n" +
                $"\t[best agent complexity: {temp.CurrentChampGenome.Complexity:N1}]\n" +
                $"\t[best agent money won per hand: {(this.handsPlayed == 0 ? 0 : (double)this.profit / (double)this.handsPlayed):N3} ({this.handsPlayed} hands played)]\n";

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(str);

            str = $"\t[meanFitness: {temp.Statistics._meanFitness:N6}]\n" +
                $"\t[meanComplexity: {temp.Statistics._meanComplexity:N3}]\n" +
                $"\t[meanSpecieChampFitness: {temp.Statistics._meanSpecieChampFitness:N6}]\n";

            foreach (var item in temp.SpecieList.OrderBy(k => k.Idx))
            {
                var bestFitness = item.GenomeList.Max(s => s.EvaluationInfo.Fitness);
                str += $"\t[specie #{item.Id}: {item.GenomeList.Count} agents] [bestFitness: {bestFitness:N6}]\n";
            }

            Console.ResetColor();
            Console.Write(str);

            this.isTrainingPause = true;
        }
    }
}
