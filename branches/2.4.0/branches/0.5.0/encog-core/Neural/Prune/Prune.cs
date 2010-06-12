﻿// Encog Neural Network and Bot Library for DotNet v0.5
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

using Encog.Neural.Feedforward;
using Encog.Neural.Feedforward.Train.Backpropagation;

namespace Encog.Neural.Prune
{
    /// <summary>
    /// Prune: Perform selective or incramental pruning.
    /// </summary>
    public class Prune
    {
        /// <summary>
        /// Get the current neural network.
        /// </summary>
        public FeedforwardNetwork CurrentNetwork
        {
            get
            {
                return this.currentNetwork;
            }
        }

        /// <summary>
        /// Called to get the current number of cycles.
        /// </summary>
        public int Cycles
        {
            get
            {
                return this.cycles;
            }
        }

        /// <summary>
        /// Called to determine if we are done in an incremental prune.
        /// </summary>
        public bool Done
        {
            get
            {
                return this.done;
            }
        }

        /// <summary>
        /// Called to get the current error.
        /// </summary>
        public double Error
        {
            get
            {
                return this.error;
            }
        }

        /// <summary>
        /// The current number of hidden neurons being evaluated.
        /// </summary>
        protected int HiddenCount
        {
            get
            {
                ICollection<FeedforwardLayer> c = this.currentNetwork
                       .HiddenLayers;
                return ((FeedforwardLayer)c.ElementAt(0)).NeuronCount;
            }
        }

        /// <summary>
        /// Get the number of hidden neurons.
        /// </summary>
        public double HiddenNeuronCount
        {
            get
            {
                return this.hiddenNeuronCount;
            }
        }


        /// <summary>
        /// The neural network that is currently being processed.
        /// </summary>
        protected FeedforwardNetwork currentNetwork;

        /// <summary>
        /// The training set.
        /// </summary>
        protected double[][] train;

        /// <summary>
        /// The ideal results from the training set.
        /// </summary>
        protected double[][] ideal;

        /// <summary>
        /// The desired learning rate.
        /// </summary>
        protected double rate;

        /// <summary>
        /// The desired momentum.
        /// </summary>
        protected double momentum;

        /// <summary>
        /// The maximum error allowed.
        /// </summary>
        protected double maxError;

        /// <summary>
        /// The current error.
        /// </summary>
        protected double error;

        /// <summary>
        /// Used to determine if training is still effectve. Holds the error level
        /// the last time the error level was tracked. This is 1000 cycles ago. If no
        /// significant drop in error occurs for 1000 cycles, training ends.
        /// </summary>
        protected double markErrorRate;

        /// <summary>
        /// Used with markErrorRate. This is the number of cycles since the error was
        /// last marked.
        /// </summary>
        protected int sinceMark;

        /// <summary>
        /// The number of cycles used.
        /// </summary>
        protected int cycles;

        /// <summary>
        /// The number of hidden neurons.
        /// </summary>
        protected int hiddenNeuronCount;

        /// <summary>
        /// Flag to indicate if the incremental prune process is done or not.
        /// </summary>
        protected bool done;

        /// <summary>
        /// The backpropagation object to use for training.
        /// </summary>
        protected Backpropagation backprop;



        /// <summary>
        /// Constructor used to setup the prune object for an incremental prune.
        /// </summary>
        /// <param name="rate">The desired learning rate.</param>
        /// <param name="momentum">The desired momentum.</param>
        /// <param name="train">The training data.</param>
        /// <param name="ideal">The ideal results for the training data.</param>
        /// <param name="maxError">The minimum error that is acceptable.</param>
        public Prune(double rate, double momentum,
                 double[][] train, double[][] ideal,
                 double maxError)
        {
            this.rate = rate;
            this.momentum = momentum;
            this.train = train;
            this.ideal = ideal;
            this.maxError = maxError;
        }


        /// <summary>
        /// Constructor that is designed to setup for a selective prune.
        /// </summary>
        /// <param name="network">The neural network that we wish to prune.</param>
        /// <param name="train">The training set input data.</param>
        /// <param name="ideal">The ideal outputs for the training set input data.</param>
        /// <param name="maxError">The maximum allowed error rate.</param>
        public Prune(FeedforwardNetwork network, double[][] train,
                 double[][] ideal, double maxError)
        {
            this.currentNetwork = network;
            this.train = train;
            this.ideal = ideal;
            this.maxError = maxError;
        }


        /// <summary>
        /// Internal method used to clip the hidden neurons.
        /// </summary>
        /// <param name="neuron">The neuron to clip.</param>
        /// <returns>Returns the new neural network.</returns>
        protected FeedforwardNetwork ClipHiddenNeuron(int neuron)
        {
            FeedforwardNetwork result = (FeedforwardNetwork)this.currentNetwork
                   .Clone();
            ICollection<FeedforwardLayer> c = result.HiddenLayers;
            ((FeedforwardLayer)c.ElementAt(0)).Prune(neuron);
            return result;
        }


        /// <summary>
        /// Internal method to determine the error for a neural network.
        /// </summary>
        /// <param name="network">The neural network that we are seeking a error rate for.</param>
        /// <returns>The error for the specified neural network.</returns>
        protected double DetermineError(FeedforwardNetwork network)
        {
            return network.CalculateError(this.train, this.ideal);

        }


        /// <summary>
        /// Internal method that will loop through all hidden neurons and prune them
        /// if pruning the neuron does not cause too great of an increase in error.
        /// </summary>
        /// <returns>True if a prune was made, false otherwise.</returns>
        protected bool FindNeuron()
        {

            for (int i = 0; i < this.HiddenCount; i++)
            {
                FeedforwardNetwork trial = this.ClipHiddenNeuron(i);
                double e2 = DetermineError(trial);
                if (e2 < this.maxError)
                {
                    this.currentNetwork = trial;
                    return true;
                }
            }
            return false;
        }



        /// <summary>
        /// Internal method that is called at the end of each incremental cycle.
        /// </summary>
        protected void Increment()
        {
            bool doit = false;

            if (this.markErrorRate == 0)
            {
                this.markErrorRate = this.error;
                this.sinceMark = 0;
            }
            else
            {
                this.sinceMark++;
                if (this.sinceMark > 10000)
                {
                    if ((this.markErrorRate - this.error) < 0.01)
                    {
                        doit = true;
                    }
                    this.markErrorRate = this.error;
                    this.sinceMark = 0;
                }
            }

            if (this.error < this.maxError)
            {
                this.done = true;
            }

            if (doit)
            {
                this.cycles = 0;
                this.hiddenNeuronCount++;

                this.currentNetwork = new FeedforwardNetwork();
                this.currentNetwork.AddLayer(new FeedforwardLayer(
                        this.train[0].Length));
                this.currentNetwork.AddLayer(new FeedforwardLayer(
                        this.hiddenNeuronCount));
                this.currentNetwork.AddLayer(new FeedforwardLayer(
                        this.ideal[0].Length));
                this.currentNetwork.Reset();

                this.backprop = new Backpropagation(this.currentNetwork,
                        this.train, this.ideal, this.rate, this.momentum);
            }
        }

        /// <summary>
        /// Method that is called to prune the neural network incramentaly.
        /// </summary>
        public void PruneIncramental()
        {
            if (this.done)
            {
                return;
            }

            this.backprop.Iteration();

            this.error = this.backprop.Error;
            this.cycles++;
            Increment();
        }

        /// <summary>
        /// Called to complete the selective pruning process.
        /// </summary>
        /// <returns>The number of neurons that were pruned.</returns>
        public int PruneSelective()
        {
            int i = this.HiddenCount;
            while (FindNeuron())
            {
                ;
            }
            return (i - this.HiddenCount);
        }

        /// <summary>
        /// Method that is called to start the incremental prune process.
        /// </summary>
        public void StartIncremental()
        {
            this.hiddenNeuronCount = 1;
            this.cycles = 0;
            this.done = false;

            this.currentNetwork = new FeedforwardNetwork();
            this.currentNetwork
                    .AddLayer(new FeedforwardLayer(this.train[0].Length));
            this.currentNetwork.AddLayer(new FeedforwardLayer(
                    this.hiddenNeuronCount));
            this.currentNetwork
                    .AddLayer(new FeedforwardLayer(this.ideal[0].Length));
            this.currentNetwork.Reset();

            this.backprop = new Backpropagation(this.currentNetwork, this.train,
                    this.ideal, this.rate, this.momentum);

        }
    }
}
