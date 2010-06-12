﻿// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Encog.Neural.Genetic;
using Encog.Solve.Genetic;

namespace Encog.Neural.Networks.Training.Genetic
{
    /// <summary>
    /// NeuralGeneticAlgorithm: Implements a genetic algorithm that 
    /// allows a feedforward neural network to be trained using a 
    /// genetic algorithm.  This algorithm is for a feed forward neural 
    /// network.  
    /// 
    /// This class is abstract.  If you wish to train the neural
    /// network using training sets, you should use the 
    /// TrainingSetNeuralGeneticAlgorithm class.  If you wish to use 
    /// a cost function to train the neural network, then
    /// implement a subclass of this one that properly calculates
    /// the cost.
    /// </summary>
    public class NeuralGeneticAlgorithm : GeneticAlgorithm<double>, ITrain
    {
        /// <summary>
        /// Get the current best neural network.
        /// </summary>
        public BasicNetwork TrainedNetwork
        {
            get
            {
                NeuralChromosome c = (NeuralChromosome)GetChromosome(0);
                c.UpdateNetwork();
                return c.Network;
            }
        }
    }
}
