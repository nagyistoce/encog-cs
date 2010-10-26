// Encog(tm) Artificial Intelligence Framework v2.3
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
using Encog.MathUtil.RBF;
using Encog.MathUtil;
using Encog.MathUtil.Randomize;
using Encog.Neural.Data;
using Encog.Neural.Data.Basic;
using Encog.Persist;
using Encog.Persist.Persistors;
#if logging
using log4net;
using Encog.Engine.Network.Activation;
using Encog.Engine.Util;
#endif
namespace Encog.Neural.Networks.Layers
{
    /// <summary>
    /// This layer type makes use of several radial basis function to scale the
    /// output from this layer. Each RBF can have a different center, peak, and
    /// width. Proper selection of these values will greatly impact the success of
    /// the layer. Currently, Encog provides no automated way of determining these
    /// values. There is one RBF per neuron.
    /// 
    /// Radial basis function layers have neither thresholds nor a regular activation
    /// function. Calling any methods that deal with the activation function or
    /// thresholds will result in an error.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class RadialBasisFunctionLayer : BasicLayer
    {

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));
#endif

        /// <summary>
        /// The radial basis functions to use, there should be one for each neuron.
        /// </summary>
        private IRadialBasisFunction[] radialBasisFunction;

        /// <summary>
        /// Default constructor, mainly so the workbench can easily create a default
        /// layer.
        /// </summary>
        public RadialBasisFunctionLayer()
            : this(1)
        {

        }

        /// <summary>
        /// Construct a radial basis function layer.
        /// </summary>
        /// <param name="neuronCount">The neuron count.</param>
        public RadialBasisFunctionLayer(int neuronCount)
            : base(new ActivationLinear(), false, neuronCount)
        {
            this.radialBasisFunction = new IRadialBasisFunction[neuronCount];
        }


        /// <summary>
        /// Compute the values before sending output to the next layer.
        /// This function allows the activation functions to be called.
        /// </summary>
        /// <param name="pattern">The incoming Project.</param>
        /// <returns>The output from this layer.</returns>
        public override INeuralData Compute(INeuralData pattern)
        {
            INeuralData result = new BasicNeuralData(NeuronCount);

            for (int i = 0; i < NeuronCount; i++)
            {

                if (this.radialBasisFunction[i] == null)
                {
                    String str =
               "Error, must define radial functions for each neuron";
#if logging
                    if (RadialBasisFunctionLayer.logger.IsErrorEnabled)
                    {
                        RadialBasisFunctionLayer.logger.Error(str);
                    }
#endif
                    throw new NeuralNetworkError(str);
                }

                IRadialBasisFunction f = this.radialBasisFunction[i];
                double total = 0;
                for (int j = 0; j < pattern.Count; j++)
                {
                    double value = f.Calculate(pattern[j]);
                    total += value * value;
                }

                result[i] = BoundMath.Sqrt(total);

            }

            return result;
        }

        /// <summary>
        /// Create a persistor for this layer.
        /// </summary>
        /// <returns></returns>
        public override IPersistor CreatePersistor()
        {
            return new RadialBasisFunctionLayerPersistor();
        }

        /// <summary>
        /// An array of radial basis functions.
        /// </summary>
        public IRadialBasisFunction[] RadialBasisFunction
        {
            get
            {
                return this.radialBasisFunction;
            }
            set
            {
                this.radialBasisFunction = value;
            }
        }

        /// <summary>
        /// Set the gausian components to random values.
        /// </summary>
        /// <param name="min">The minimum value for the centers, widths and peaks.</param>
        /// <param name="max">The maximum value for the centers, widths and peaks.</param>
        /// <param name="RBFType">The RBF to use.</param>
        public void RandomizeRBFCentersAndWidths(double min, double max, RBFEnum RBFType)
        {
            for (int i = 0; i < this.NeuronCount; i++)
            {
                SetRBFFunction(i, RBFType, RangeRandomizer.Randomize(min, max), RangeRandomizer.Randomize(min, max), RangeRandomizer.Randomize(min, max));
            }
        }

        /// <summary>
        /// Set the RBF centers and widths.
        /// </summary>
        /// <param name="centers">The centers.</param>
        /// <param name="widths">The widths.</param>
        /// <param name="RBFType">The types of functions.</param>
        public void SetRBFCentersAndWidths(double[] centers, double[] widths, RBFEnum RBFType)
        {
            for (int i = 0; i < this.NeuronCount; i++)
                SetRBFFunction(i, RBFType, centers[i], RangeRandomizer.Randomize(0, 1), widths[i]);
        }

        /// <summary>
        /// Set the type of RBF function.
        /// </summary>
        /// <param name="RBFIndex">The RBF index.</param>
        /// <param name="RBFType">The RBF type.</param>
        /// <param name="center">The centers.</param>
        /// <param name="peak">The peaks.</param>
        /// <param name="width">The widths.</param>
        private void SetRBFFunction(int RBFIndex, RBFEnum RBFType, double center, double peak, double width)
        {
            if (RBFType == RBFEnum.Gaussian)
                this.radialBasisFunction[RBFIndex] = new GaussianFunction(center, peak, width);
            else if (RBFType == RBFEnum.GaussianMulti)
                throw new NotSupportedException();
            else if (RBFType == RBFEnum.Multiquadric)
                this.radialBasisFunction[RBFIndex] = new MultiquadricFunction(center, peak, width);
            else if (RBFType == RBFEnum.MultiquadricMulti)
                throw new NotSupportedException();
            else if (RBFType == RBFEnum.InverseMultiquadric)
                this.radialBasisFunction[RBFIndex] = new InverseMultiquadricFunction(center, peak, width);
            else if (RBFType == RBFEnum.InverseMultiquadricMulti)
                throw new NotSupportedException();
        }
    }

}
