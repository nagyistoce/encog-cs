//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Genetic.Population;
using Encog.MathUtil;

namespace Encog.ML.Genetic.Genome
{
    /// <summary>
    /// A basic abstract genome. Provides base functionality.
    /// </summary>
    ///
    [Serializable]
    public abstract class BasicGenome : IGenome
    {
        /// <summary>
        /// The genetic algorithm.
        /// </summary>
        [NonSerialized] 
        private GeneticAlgorithm _ga;

        /// <summary>
        /// The chromosomes for this gene.
        /// </summary>
        ///
        private readonly IList<Chromosome> _chromosomes;

        /// <summary>
        /// The adjusted score.
        /// </summary>
        ///
        private double _adjustedScore;

        /// <summary>
        /// The amount to spawn.
        /// </summary>
        ///
        private double _amountToSpawn;

        /// <summary>
        /// The genome id.
        /// </summary>
        ///
        private long _genomeID;

        /// <summary>
        /// The organism generated by this gene.
        /// </summary>
        ///
        [NonSerialized]
        private Object _organism;

        /// <summary>
        /// The population this genome belongs to.
        /// </summary>
        ///
        private IPopulation _population;

        /// <summary>
        /// The score of this genome.
        /// </summary>
        ///
        private double _score;

        /// <summary>
        /// Construct the bo
        /// </summary>
        protected BasicGenome()
        {
            _chromosomes = new List<Chromosome>();
            _score = 0;
        }

        #region IGenome Members

        /// <returns>The number of genes in this genome.</returns>
        public int CalculateGeneCount()
        {
            // sum the genes in the chromosomes.
            return _chromosomes.Sum(chromosome => chromosome.Genes.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public int CompareTo(IGenome other)
        {
            // might be null when deserializing
            if( _ga==null )
            {
                return 0;
            }
            // compare
            if ( _ga.CalculateScore.ShouldMinimize)
            {
                if (Math.Abs(Score - other.Score) < EncogFramework.DefaultDoubleEqual)
                {
                    return 0;
                }
                if (Score > other.Score)
                {
                    return 1;
                }
                return -1;
            }
            if (Math.Abs(Score - other.Score) < EncogFramework.DefaultDoubleEqual)
            {
                return 0;
            }
            if (Score > other.Score)
            {
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// Set the adjusted score.
        /// </summary>
        ///
        /// <value>The score.</value>
        public double AdjustedScore
        {
            get { return _adjustedScore; }
            set { _adjustedScore = value; }
        }


        /// <summary>
        /// Set the amount to spawn.
        /// </summary>
        public double AmountToSpawn
        {
            get { return _amountToSpawn; }
            set { _amountToSpawn = value; }
        }


        /// <value>The number of chromosomes.</value>
        public IList<Chromosome> Chromosomes
        {
            get { return _chromosomes; }
        }


        /// <summary>
        /// Set the genetic algorithm to use.
        /// </summary>
        public GeneticAlgorithm GA
        {
            get { return _ga; }
            set { _ga = value; }
        }


        /// <summary>
        /// Set the genome id.
        /// </summary>
        public long GenomeID
        {
            get { return _genomeID; }
            set { _genomeID = value; }
        }


        /// <summary>
        /// Set the organism.
        /// </summary>
        public Object Organism
        {
            get { return _organism; }
            set { _organism = value; }
        }


        /// <value>the population to set</value>
        public IPopulation Population
        {
            get { return _population; }
            set { _population = value; }
        }


        /// <summary>
        /// Set the score.
        /// </summary>
        public double Score
        {
            get { return _score; }
            set { _score = value; }
        }


        /// <summary>
        /// Mate two genomes. Will loop over all chromosomes.
        /// </summary>
        ///
        /// <param name="father">The father.</param>
        /// <param name="child1">The first child.</param>
        /// <param name="child2">The second child.</param>
        public void Mate(IGenome father, IGenome child1,
                         IGenome child2)
        {
            int motherChromosomes = Chromosomes.Count;
            int fatherChromosomes = father.Chromosomes.Count;

            if (motherChromosomes != fatherChromosomes)
            {
                throw new GeneticError(
                    "Mother and father must have same chromosome count, Mother:"
                    + motherChromosomes + ",Father:"
                    + fatherChromosomes);
            }

            for (int i = 0; i < fatherChromosomes; i++)
            {
                Chromosome motherChromosome = _chromosomes[i];
                Chromosome fatherChromosome = father.Chromosomes[i];
                Chromosome offspring1Chromosome = child1.Chromosomes[i];
                Chromosome offspring2Chromosome = child2.Chromosomes[i];

                _ga.Crossover.Mate(motherChromosome,
                                                fatherChromosome, offspring1Chromosome,
                                                offspring2Chromosome);

                if (ThreadSafeRandom.NextDouble() < _ga.MutationPercent)
                {
                    _ga.Mutate.PerformMutation(
                        offspring1Chromosome);
                }

                if (ThreadSafeRandom.NextDouble() < _ga.MutationPercent)
                {
                    _ga.Mutate.PerformMutation(
                        offspring2Chromosome);
                }
            }

            child1.Decode();
            child2.Decode();
            _ga.PerformCalculateScore(child1);
            _ga.PerformCalculateScore(child2);
        }

        /// <summary>
        /// from Encog.ml.genetic.genome.Genome
        /// </summary>
        ///
        public abstract void Decode();

        /// <summary>
        /// from Encog.ml.genetic.genome.Genome
        /// </summary>
        ///
        public abstract void Encode();

        #endregion

        /// <inheritdoc />
        public override sealed String ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[");
            builder.Append(GetType().Name);
            builder.Append(": score=");
            builder.Append(Score);
            return builder.ToString();
        }
    }
}
