namespace TexasHoldem.AI.NeuroPlayer
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    using SharpNeat.Core;
    using SharpNeat.EvolutionAlgorithms;
    using SharpNeat.Genomes.Neat;
    using TexasHoldem.Logic;

    internal class Program
    {
        private static IGenomeFactory<NeatGenome> genomeFactory;

        private static List<NeatGenome> genomeList;

        private static NeatEvolutionAlgorithm<NeatGenome> ea;

        private static Helpers.Agent agent;

        public static void Main(string[] args)
        {
            var experiment = new SharpNeatPoker.Experiment();

            WriteHelp();

            // Read key commands from the console
            do
            {
                // Read command
                Console.Write(">");
                string cmdString = Console.ReadLine();

                // Parse command.
                string[] cmdArgs = cmdString.Split(' ');

                try
                {
                    // Process command
                    if (cmdArgs[0].Equals("randpop"))
                    {
                        if (ea != null && ea.RunState == RunState.Running)
                        {
                            Console.WriteLine("Error. Cannot create population while algorithm is running.");

                            continue;
                        }

                        // Attempt to parse population size arg
                        if (cmdArgs.Length <= 1)
                        {
                            Console.WriteLine("Error. Missing [size] argument.");

                            continue;
                        }

                        int populationSize;

                        if (!int.TryParse(cmdArgs[1], out populationSize))
                        {
                            Console.WriteLine($"Error. Invalid [size] argument [{cmdArgs[1]}].");

                            continue;
                        }

                        // Create a genome factory with our neat genome parameters object and the appropriate number of input and output neuron genes.
                        genomeFactory = experiment.CreateGenomeFactory();

                        // Create an initial population of randomly generated genomes.
                        genomeList = genomeFactory.CreateGenomeList(populationSize, 0);
                        Console.WriteLine($"Created [{populationSize}] random genomes.");

                        continue;
                    }
                    else if (cmdArgs[0].Equals("loadpop"))
                    {
                        if (ea != null && ea.RunState == RunState.Running)
                        {
                            Console.WriteLine("Error. Cannot load population while algorithm is running.");

                            continue;
                        }

                        // Attempt to get population filename arg.
                        if (cmdArgs.Length <= 1)
                        {
                            Console.WriteLine("Error. Missing {filename} argument.");

                            continue;
                        }

                        // Open and load population XML file.
                        using (XmlReader xr = XmlReader.Create(cmdArgs[1]))
                        {
                            genomeList = experiment.LoadPopulation(xr);
                        }

                        genomeFactory = genomeList[0].GenomeFactory;
                        Console.WriteLine($"Loaded [{genomeList.Count}] genomes.");

                        continue;
                    }
                    else if (cmdArgs[0].Equals("loadseed"))
                    {
                        if (ea != null && ea.RunState == RunState.Running)
                        {
                            Console.WriteLine("Error. Cannot load population while algorithm is running.");

                            continue;
                        }

                        // Attempt to get genome filename arg
                        if (cmdArgs.Length <= 1)
                        {
                            Console.WriteLine("Error. Missing {filename} argument.");

                            continue;
                        }

                        // Attempt to parse population size arg
                        if (cmdArgs.Length <= 2)
                        {
                            Console.WriteLine("Error. Missing [size] argument.");

                            continue;
                        }

                        int populationSize;

                        if (!int.TryParse(cmdArgs[2], out populationSize))
                        {
                            Console.WriteLine($"Error. Invalid [size] argument [{cmdArgs[1]}].");

                            continue;
                        }

                        // Open and load genome XML file.
                        using (XmlReader xr = XmlReader.Create(cmdArgs[1]))
                        {
                            genomeList = experiment.LoadPopulation(xr);
                        }

                        if (genomeList.Count == 0)
                        {
                            Console.WriteLine($"No genome loaded from file [{cmdArgs[1]}]");
                            genomeList = null;

                            continue;
                        }

                        // Create genome list from seed.
                        genomeFactory = genomeList[0].GenomeFactory;
                        genomeList = genomeFactory.CreateGenomeList(populationSize, 0u, genomeList[0]);
                        Console.WriteLine($"Created [{genomeList.Count}] genomes from loaded seed genome.");

                        continue;
                    }
                    else if (cmdArgs[0].Equals("start"))
                    {
                        if (ea == null)
                        {
                            // Create new evolution algorithm
                            if (genomeList == null)
                            {
                                Console.WriteLine("Error. No loaded genomes");

                                continue;
                            }

                            ea = experiment.CreateEvolutionAlgorithm(genomeFactory, genomeList);
                            ea.UpdateEvent += new EventHandler(EvolutionAlgorithmUpdateEvent);
                        }

                        Console.WriteLine("Starting...");
                        ea.StartContinue();

                        continue;
                    }
                    else if (cmdArgs[0].Equals("stop"))
                    {
                        Console.WriteLine("Stopping. Please wait...");
                        ea.RequestPauseAndWait();
                        Console.WriteLine("Stopped.");

                        continue;
                    }
                    else if (cmdArgs[0].Equals("reset"))
                    {
                        if (ea != null && ea.RunState == RunState.Running)
                        {
                            Console.WriteLine("Error. Cannot reset while algorithm is running.");

                            continue;
                        }

                        ea = null;
                        genomeFactory = null;
                        genomeList = null;
                        Console.WriteLine("Reset completed.");

                        continue;
                    }
                    else if (cmdArgs[0].Equals("savepop"))
                    {
                        if (ea != null && ea.RunState == RunState.Running)
                        {
                            Console.WriteLine("Error. Cannot save population while algorithm is running.");

                            continue;
                        }

                        if (genomeList == null)
                        {
                            Console.WriteLine("Error. No population to save.");

                            continue;
                        }

                        // Attempt to get population filename arg
                        if (cmdArgs.Length <= 1)
                        {
                            Console.WriteLine("Error. Missing {filename} argument.");

                            continue;
                        }

                        // Save genomes to xml file.
                        XmlWriterSettings xwSettings = new XmlWriterSettings();
                        xwSettings.Indent = true;

                        using (XmlWriter xw = XmlWriter.Create(cmdArgs[1], xwSettings))
                        {
                            experiment.SavePopulation(xw, genomeList);
                        }

                        Console.WriteLine($"[{genomeList.Count}] genomes saved to file [{cmdArgs[1]}]");

                        continue;
                    }
                    else if (cmdArgs[0].Equals("savebest"))
                    {
                        if (ea != null && ea.RunState == RunState.Running)
                        {
                            Console.WriteLine("Error. Cannot save population while algorithm is running.");

                            continue;
                        }

                        if (ea == null || ea.CurrentChampGenome == null)
                        {
                            Console.WriteLine("Error. No best genome to save.");

                            continue;
                        }

                        // Attempt to get genome filename arg
                        if (cmdArgs.Length <= 1)
                        {
                            Console.WriteLine("Error. Missing {filename} argument.");
                            break;
                        }

                        // Save genome to xml file.
                        XmlWriterSettings xwSettings = new XmlWriterSettings();
                        xwSettings.Indent = true;

                        using (XmlWriter xw = XmlWriter.Create(cmdArgs[1], xwSettings))
                        {
                            experiment.SavePopulation(xw, new NeatGenome[] { ea.CurrentChampGenome });
                        }

                        Console.WriteLine($"Best genome saved to file [{cmdArgs[1]}]");

                        continue;
                    }
                    else if (cmdArgs[0].Equals("help"))
                    {
                        WriteHelp();

                        continue;
                    }
                    else if (cmdArgs[0].Equals("quit") || cmdArgs[0].Equals("exit"))
                    {
                        Console.WriteLine("Stopping. Please wait...");
                        ea.RequestPauseAndWait();
                        Console.WriteLine("Stopped.");

                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Unknown command [{cmdArgs[0]}]");

                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception [{ex.Message}]");
                }
            }
            while (true);

            Console.WriteLine("Press any key to exit...");

            // Hit return to quit.
            Console.ReadKey(true);
        }

        private static void EvolutionAlgorithmUpdateEvent(object sender, EventArgs e)
        {
            var temp = sender as NeatEvolutionAlgorithm<NeatGenome>;

            if (agent == null || (agent != null && temp.CurrentChampGenome.Id != agent.Genome.Id))
            {
                agent = new Helpers.Agent(temp.CurrentChampGenome);
            }

            var str = $"\nUpdated at {DateTime.Now.ToLongTimeString()}\n" +
                $"generation: [{temp.Statistics._generation}]\n" +
                $"state: [{temp.RunState}]\n" +
                $"bestFitness: [{temp.Statistics._maxFitness:N6}]; " +
                $"meanFitness: [{temp.Statistics._meanFitness:N6}]\n" +
                $"bestComplexity: [{temp.CurrentChampGenome.Complexity:N0}]; " +
                $"meanComplexity: [{temp.Statistics._meanComplexity:N3}]\n" +
                $"meanSpecieChampFitness: [{temp.Statistics._meanSpecieChampFitness:N6}]\n" +
                $"--------------------------------------------------------------------------------\n" +
                $"STATISTICS OF THE BEST AGENT:\n" +
                $"Money won per hand: [{agent.MoneyWonPerHand():N3}] ([{agent.HandsPlayed}] hands played)\n" +
                $"{agent.Stats.ToString()}\n";
            Console.Write(str);
        }

        private static void WriteHelp()
        {
            Console.Write(
$@"NeuroTraining command line interface (CLI).

Commands...
Initialisation
    randpop [size]                  Create random population.
    loadpop [filename]              Load existing population from file.
    loadseed [filename] [popSize]   Load seed genome from file.

Execution control:
    start                           Start/Continue evolution algorithm.
    stop                            Stop/Pause evolution algorithm.
    reset                           Discard any current population and evolution algorithm.

Genome saving commands:
    savepop [filename]              Save population to XML file.
    savebest [filename]             Save best genome to XML file.

Misc
    help                            Show this help text.  
    exit, quit                      Exit CLI.

");
        }
    }
}
