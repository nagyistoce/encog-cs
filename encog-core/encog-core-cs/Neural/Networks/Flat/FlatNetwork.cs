// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
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
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Structure;
using Encog.Neural.Activation;
using Encog.Util;
using Encog.Neural.NeuralData;
using Encog.Neural.Data;
using Encog.MathUtil.Error;

namespace Encog.Neural.Networks.Flat
{
    /// <summary>
    /// Implements a flat (vector based) neural network in Encog. This is meant to be
    /// a very highly efficient feedforward neural network. It uses a minimum of
    /// objects and is designed with one principal in mind-- SPEED. Readability, code
    /// reuse, object oriented programming are all secondary in consideration.
    /// 
    /// Currently, the flat networks only support feedforward networks with either a
    /// sigmoid or tanh activation function.  Specifically, a flat network must:
    /// 
    /// 1. Feedforward only, no self-connections or recurrent links
    /// 2. Sigmoid or TANH activation only
    /// 3. All layers the same activation function
    /// 4. Must have threshold values
    /// 
    /// Vector based neural networks are also very good for CL processing. The flat
    /// network classes will make use of the CL if you have enabled CL processing.
    /// See the Encog class for more info.
    /// </summary>
    public class FlatNetwork
    {
        /// <summary>
        /// A linear activation function.
        /// </summary>
        public const int ACTIVATION_LINEAR = 0;

        /// <summary>
        /// A TANH activation function.
        /// </summary>
        public const int ACTIVATION_TANH = 1;

        /// <summary>
        /// A sigmoid activation function.
        /// </summary>
        public const int ACTIVATION_SIGMOID = 2;

        /// <summary>
        /// The number of input neurons in this network.
        /// </summary>
        private int inputCount;

        /// <summary>
        /// The number of neurons in each of the layers.
        /// </summary>
        private int[] layerCounts;

        /// <summary>
        /// An index to where each layer begins (based on the number of neurons in
        /// each layer).
        /// </summary>
        private int[] layerIndex;

        /// <summary>
        /// The outputs from each of the neurons.
        /// </summary>
        private double[] layerOutput;

        /// <summary>
        /// The number of output neurons in this network.
        /// </summary>
        private int outputCount;

        /// <summary>
        /// The index to where the weights and thresholds are stored at for a given
        /// layer.
        /// </summary>
        private int[] weightIndex;

        /// <summary>
        /// The weights and thresholds for a neural network.
        /// </summary>
        private double[] weights;

        /// <summary>
        /// The activation types.
        /// </summary>
        private int[] activationType;

        /// <summary>
        /// Bias values on the input layer serve no value.  But some networks are 
        /// constructed in this way, because they use the default BasicLayer 
        /// constructor.  We need to remember that there was an input bias, so that 
        /// the network is unflattened this way.
        /// </summary>
        public bool HasInputBias { get; set; }

        /// <summary>
        /// Construct a flat network.
        /// </summary>
        /// <param name="network">The network to construct the flat network from.</param>
        public FlatNetwork(BasicNetwork network)
        {
            ValidateForFlat.ValidateNetwork(network);

            ILayer input = network.GetLayer(BasicNetwork.TAG_INPUT);
            ILayer output = network.GetLayer(BasicNetwork.TAG_OUTPUT);

            inputCount = input.NeuronCount;
            outputCount = output.NeuronCount;

            HasInputBias = input.HasBias;

            int layerCount = network.Structure.Layers.Count;

            layerCounts = new int[layerCount];
            weightIndex = new int[layerCount];
            layerIndex = new int[layerCount];
            activationType = new int[layerCount];

            int index = 0;
            int neuronCount = 0;

            foreach (ILayer layer in network.Structure.Layers)
            {
                layerCounts[index] = layer.NeuronCount;

                if (layer.ActivationFunction is ActivationLinear)
                    activationType[index] = FlatNetwork.ACTIVATION_LINEAR;
                else if (layer.ActivationFunction is ActivationTANH)
                    activationType[index] = FlatNetwork.ACTIVATION_TANH;
                else if (layer.ActivationFunction is ActivationSigmoid)
                    activationType[index] = FlatNetwork.ACTIVATION_SIGMOID;

                neuronCount += layer.NeuronCount;

                if (index == 0)
                {
                    weightIndex[index] = 0;
                    layerIndex[index] = 0;
                }
                else
                {
                    weightIndex[index] = weightIndex[index - 1]
                            + (layerCounts[index - 1] + (layerCounts[index] * layerCounts[index - 1]));
                    layerIndex[index] = layerIndex[index - 1]
                            + layerCounts[index - 1];
                }

                index++;
            }

            weights = NetworkCODEC.NetworkToArray(network);
            layerOutput = new double[neuronCount];

        }

        /// <summary>
        /// Generate a regular Encog neural network from this flat network. 
        /// </summary>
        /// <returns>A regular Encog neural network.</returns>
        public BasicNetwork Unflatten()
        {
            BasicNetwork result = new BasicNetwork();
            bool useBias = HasInputBias;

            for (int i = this.layerCounts.Length - 1; i >= 0; i--)
            {
                IActivationFunction activation;

                switch (this.activationType[i])
                {
                    case FlatNetwork.ACTIVATION_LINEAR:
                        activation = new ActivationLinear();
                        break;
                    case FlatNetwork.ACTIVATION_SIGMOID:
                        activation = new ActivationSigmoid();
                        break;
                    case FlatNetwork.ACTIVATION_TANH:
                        activation = new ActivationTANH();
                        break;
                    default:
                        activation = null;
                        break;
                }

                ILayer layer = new BasicLayer(activation, useBias, this.layerCounts[i]);
                useBias = true;
                result.AddLayer(layer);
            }
            result.Structure.FinalizeStructure();

            NetworkCODEC.ArrayToNetwork(this.weights, result);

            return result;
        }

        /// <summary>
        /// Calculate the output for the given input. 
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">Output will be placed here.</param>
        public void Compute(double[] input, double[] output)
        {
            int sourceIndex = layerOutput.Length - inputCount;

            EncogArray.ArrayCopy(input, 0, layerOutput, sourceIndex, inputCount);

            for (int i = layerIndex.Length - 1; i > 0; i--)
            {
                ComputeLayer(i);
            }

            EncogArray.ArrayCopy(layerOutput, 0, output, 0, outputCount);
        }


        /// <summary>
        /// Calculate a layer. 
        /// </summary>
        /// <param name="currentLayer">The layer to calculate.</param>
        private void ComputeLayer(int currentLayer)
        {

            int inputIndex = layerIndex[currentLayer];
            int outputIndex = layerIndex[currentLayer - 1];
            int inputSize = layerCounts[currentLayer];
            int outputSize = layerCounts[currentLayer - 1];

            int index = weightIndex[currentLayer - 1];

            // threshold values
            for (int i = 0; i < outputSize; i++)
            {
                layerOutput[i + outputIndex] = weights[index++];
            }

            // weight values
            for (int x = 0; x < outputSize; x++)
            {
                double sum = 0;
                for (int y = 0; y < inputSize; y++)
                {
                    sum += weights[index++] * layerOutput[inputIndex + y];
                }
                layerOutput[outputIndex + x] += sum;

                layerOutput[outputIndex + x] = FlatNetwork.CalculateActivation(
                    this.activationType[0],
                    layerOutput[outputIndex + x]);
            }
        }

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        public int InputCount
        {
            get
            {
                return inputCount;
            }
        }

        /// <summary>
        /// The number of neurons in each layer.
        /// </summary>
        public int[] LayerCounts
        {
            get
            {
                return layerCounts;
            }
        }

        /// <summary>
        /// Indexes into the weights for the start of each layer.
        /// </summary>
        public int[] LayerIndex
        {
            get
            {
                return layerIndex;
            }
        }

        /// <summary>
        /// The output for each layer.
        /// </summary>
        public double[] LayerOutput
        {
            get
            {
                return layerOutput;
            }
        }

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        public int OutputCount
        {
            get
            {
                return outputCount;
            }
        }

        /// <summary>
        /// The index of each layer in the weight and threshold array.
        /// </summary>
        public int[] WeightIndex
        {
            get
            {
                return weightIndex;
            }
        }

        /// <summary>
        /// The index of each layer in the weight and threshold array.
        /// </summary>
        public double[] Weights
        {
            get
            {
                return weights;
            }
        }

        /// <summary>
        /// Clone the network.
        /// </summary>
        /// <returns>A clone of the network.</returns>
        public FlatNetwork Clone()
        {
            BasicNetwork temp = this.Unflatten();
            return new FlatNetwork(temp);
        }

        /// <summary>
        /// Calculate the error for this neural network. The error is calculated
        /// using root-mean-square(RMS).
        /// </summary>
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        public double CalculateError(INeuralDataSet data)
        {
            ErrorCalculation errorCalculation = new ErrorCalculation();

            double[] actual = new double[OutputCount];

            foreach (INeuralDataPair pair in data)
            {
                Compute(pair.Input.Data, actual);
                errorCalculation.UpdateError(actual, pair.Ideal.Data);
            }
            return errorCalculation.Calculate();
        }

        /// <summary>
        /// The activation types for each of the layers.
        /// </summary>
        public int[] ActivationType
        {
            get
            {
                return this.activationType;
            }
        }

        /// <summary>
        /// Calculate an activation.
        /// </summary>
        /// <param name="type">The type of activation.</param>
        /// <param name="x">The value to calculate the activation for.</param>
        /// <returns>The resulting value.</returns>
        public static double CalculateActivation(int type, double x)
        {
            switch (type)
            {
                case FlatNetwork.ACTIVATION_LINEAR:
                    return x;
                case FlatNetwork.ACTIVATION_TANH:
                    return -1.0 + (2.0 / (1.0 + BoundMath.Exp(-2.0 * x)));
                case FlatNetwork.ACTIVATION_SIGMOID:
                    return 1.0 / (1.0 + BoundMath.Exp(-1.0 * x));
                default:
                    throw new NeuralNetworkError("Unknown activation type: " + type);
            }
        }

        /// <summary>
        /// The neuron count.
        /// </summary>
        public int NeuronCount
        {
            get
            {
                int result = 0;
                for (int i = 0; i < LayerCounts.Length; i++)
                {
                    result += LayerCounts[i];
                }
                return result;
            }
        }

        /// <summary>
        /// Calculate the derivative of the activation.
        /// </summary>
        /// <param name="type">The type of activation.</param>
        /// <param name="x">The value to calculate for.</param>
        /// <returns>The result.</returns>
        public static double CalculateActivationDerivative(int type, double x)
        {
            switch (type)
            {
                case FlatNetwork.ACTIVATION_LINEAR:
                    return 1;
                case FlatNetwork.ACTIVATION_TANH:
                    return (1.0 - (x*x));
                case FlatNetwork.ACTIVATION_SIGMOID:
                    return x * (1.0 - x);
                default:
                    throw new NeuralNetworkError("Unknown activation type: " + type);
            }
        }

        /// <summary>
        /// Neural networks with only one type of activation function offer certain 
        /// optimization options.  This method determines if only a single activation 
        /// function is used.
        /// </summary>
        /// <returns>The number of the single activation function, or -1 if there 
        /// are no activation functions or more than one type of activation function.</returns>
        public int HasSameActivationFunction()
        {
            IList<int> map = new List<int>();

            foreach (int activation in this.activationType)
            {
                if (!map.Contains(activation))
                    map.Add(activation);
            }

            if (map.Count != 1)
                return -1;
            else
                return map[0];
        }

    }
}
