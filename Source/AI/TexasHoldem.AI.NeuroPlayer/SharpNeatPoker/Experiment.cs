namespace TexasHoldem.AI.NeuroPlayer.SharpNeatPoker
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Xml;

    using SharpNeat.Core;
    using SharpNeat.Decoders;
    using SharpNeat.Decoders.Neat;
    using SharpNeat.DistanceMetrics;
    using SharpNeat.EvolutionAlgorithms;
    using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
    using SharpNeat.Genomes.Neat;
    using SharpNeat.Phenomes;
    using SharpNeat.SpeciationStrategies;

    public class Experiment
    {
        private NetworkActivationScheme activationScheme;

        private ParallelOptions parallelOptions;

        private NeatEvolutionAlgorithmParameters eaParams;

        private NeatGenomeParameters neatGenomeParams;

        public Experiment()
        {
            this.InputCount = 39;
            this.OutputCount = 3;

            this.eaParams = new NeatEvolutionAlgorithmParameters();
            this.eaParams.SpecieCount = 100;                    // default: 10
            this.eaParams.ElitismProportion = 0.05;             // default: 0.2
            this.eaParams.SelectionProportion = 0.15;           // default: 0.2
            this.eaParams.OffspringAsexualProportion = 0.7;     // default: 0.5
            this.eaParams.OffspringSexualProportion = 0.3;      // default: 0.5
            this.eaParams.InterspeciesMatingProportion = 0.01;  // default: 0.01

            this.neatGenomeParams = new NeatGenomeParameters();
            this.neatGenomeParams.ConnectionWeightRange = 3.0;                  // default: 5
            this.neatGenomeParams.ConnectionWeightMutationProbability = 0.95;   // default: 0.94;
            this.neatGenomeParams.AddNodeMutationProbability = 0.01;            // default: 0.01;
            this.neatGenomeParams.AddConnectionMutationProbability = 0.075;     // default: 0.025;
            this.neatGenomeParams.DeleteConnectionMutationProbability = 0.075;  // default: 0.025;

            this.parallelOptions = new ParallelOptions();

            // NetworkActivationScheme.CreateCyclicFixedTimestepsScheme(1);
            this.activationScheme = NetworkActivationScheme.CreateAcyclicScheme();
            this.neatGenomeParams.FeedforwardOnly = this.activationScheme.AcyclicNetwork;
        }

        public int InputCount { get; private set; }

        public int OutputCount { get; private set; }

        public List<NeatGenome> LoadPopulation(XmlReader xr)
        {
            var genomeFactory = (NeatGenomeFactory)this.CreateGenomeFactory();

            return NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, genomeFactory);
        }

        public void SavePopulation(XmlWriter xw, IList<NeatGenome> genomeList)
        {
            // Writing node IDs is not necessary for NEAT.
            NeatGenomeXmlIO.WriteComplete(xw, genomeList, false);
        }

        public IGenomeDecoder<NeatGenome, IBlackBox> CreateGenomeDecoder()
        {
            return new NeatGenomeDecoder(this.activationScheme);
        }

        public IGenomeFactory<NeatGenome> CreateGenomeFactory()
        {
            return new NeatGenomeFactory(this.InputCount, this.OutputCount, this.neatGenomeParams);

            // return new SharpNeat.Genomes.HyperNeat.CppnGenomeFactory(
            //    this.InputCount,
            //    this.OutputCount,
            //    SharpNeat.Network.DefaultActivationFunctionLibrary.CreateLibraryCppn(),
            //    this.neatGenomeParams);
        }

        public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(
            IGenomeFactory<NeatGenome> genomeFactory, List<NeatGenome> genomeList)
        {
            // Create distance metric. Mismatched genes have a fixed distance of 10;
            // for matched genes the distance is their weight difference.
            IDistanceMetric distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
            ISpeciationStrategy<NeatGenome> speciationStrategy =
                new ParallelKMeansClusteringStrategy<NeatGenome>(distanceMetric);

            // Create complexity regulation strategy.
            IComplexityRegulationStrategy complexityRegulationStrategy = new NullComplexityRegulationStrategy();

            // Create the evolution algorithm.
            NeatEvolutionAlgorithm<NeatGenome> ea =
                new NeatEvolutionAlgorithm<NeatGenome>(this.eaParams, speciationStrategy, complexityRegulationStrategy);

            // Create genome decoder.
            IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder = this.CreateGenomeDecoder();

            var evaluator = new Evaluator();

            // Create a genome list evaluator. This packages up the genome decoder with the genome evaluator.
            // IGenomeListEvaluator<NeatGenome> innerEvaluator =
            //    new SerialGenomeListEvaluator<NeatGenome, IBlackBox>(genomeDecoder, evaluator);
            IGenomeListEvaluator<NeatGenome> innerEvaluator =
                new ParallelGenomeListEvaluator<NeatGenome, IBlackBox>(genomeDecoder, evaluator);

            // Wrap the list evaluator in a 'selective' evaluator that will only evaluate new genomes.
            // That is, we skip re-evaluating any genomes that were in the population in previous
            // generations (elite genomes). This is determined by examining each genome's evaluation info object.
            IGenomeListEvaluator<NeatGenome> selectiveEvaluator =
                new SelectiveGenomeListEvaluator<NeatGenome>(
                    innerEvaluator,
                    SelectiveGenomeListEvaluator<NeatGenome>.CreatePredicate_OnceOnly());

            // Initialize the evolution algorithm.
            ea.Initialize(selectiveEvaluator, genomeFactory, genomeList);
            ea.UpdateScheme = new UpdateScheme(1);

            // Finished. Return the evolution algorithm
            return ea;
        }
    }
}
