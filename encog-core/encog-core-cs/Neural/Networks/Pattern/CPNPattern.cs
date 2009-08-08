﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Encog.Neural.Activation;
using Encog.Neural.Networks.Layers;

namespace Encog.Neural.Networks.Pattern
{
    /// <summary>
    /// Pattern that creates a CPN neural network.
    /// </summary>
    public class CPNPattern : INeuralNetworkPattern
    {

        /// <summary>
        /// The INSTAR tag.
        /// </summary>
        public const String TAG_INSTAR = "INSTAR";

        /// <summary>
        /// The OUTSTAR tag.
        /// </summary>
        public const String TAG_OUTSTAR = "OUTSTAR";

        /// <summary>
        /// The number of neurons in the instar layer.
        /// </summary>
        int instarCount;

        /// <summary>
        /// The number of neurons in the outstar layer.
        /// </summary>
        int outstarCount;

        /// <summary>
        /// The number of neurons in the hidden layer.
        /// </summary>
        int inputCount;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(CPNPattern));

        /// <summary>
        /// Not used, will throw an error. CPN networks already have a predefined
        /// hidden layer called the instar layer.
        /// </summary>
        /// <param name="count">Not used</param>
        public void AddHiddenLayer(int count)
        {
            String str =
               "A CPN already has a predefined hidden layer.  No additiona" +
               "specification is needed.";
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(str);
            }

        }

        /// <summary>
        /// Clear any parameters that were set.
        /// </summary>
        public void Clear()
        {
            this.inputCount = this.instarCount = this.outstarCount = 0;
        }

        /// <summary>
        /// Generate the network.
        /// </summary>
        /// <returns>The generated network.</returns>
        public BasicNetwork Generate()
        {

            ILayer input, instar, outstar;
            int y = PatternConst.START_Y;

            BasicNetwork network = new BasicNetwork();
            network.AddLayer(input = new BasicLayer(this.inputCount));
            network.AddLayer(instar = new BasicLayer(new ActivationCompetitive(), true, this.instarCount));
            network.AddLayer(outstar = new BasicLayer(this.outstarCount));
            network.Structure.FinalizeStructure();
            network.Reset();

            input.X = PatternConst.START_X;
            input.Y = y;
            y += PatternConst.INC_Y;

            instar.X = PatternConst.START_X;
            instar.Y = y;
            y += PatternConst.INC_Y;

            outstar.X = PatternConst.START_X;
            outstar.Y = y;

            // tag as needed
            network.TagLayer(BasicNetwork.TAG_INPUT, input);
            network.TagLayer(BasicNetwork.TAG_OUTPUT, outstar);
            network.TagLayer(CPNPattern.TAG_INSTAR, instar);
            network.TagLayer(CPNPattern.TAG_OUTSTAR, outstar);

            return network;
        }

        /// <summary>
        /// Not used, can't set activation function for CPN.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            get
            {
                String str =
                "A CPN network will use the BiPolar & competitive activation "
                + "functions, no activation function needs to be specified.";
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
                throw new PatternError(str);
            }
            set
            {
                String str =
                "A CPN network will use the BiPolar & competitive activation "
                + "functions, no activation function needs to be specified.";
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
                throw new PatternError(str);
            }
        }


        /// <summary>
        /// The number of neurons in the instar layer.  This level
        /// is essentially a hidden layer.
        /// </summary>
        public int InstarCount
        {
            get
            {
                return this.instarCount;
            }
            set
            {
                this.instarCount = value;
            }
        }

        /// <summary>
        /// Set the number of neurons in the outstar level, this level
        /// is mapped to the "output" level.
        /// </summary>
        public int OutstarCount
        {
            get
            {
                return this.outstarCount;
            }
            set
            {
                this.outstarCount = value;
            }
        }

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        public int OutputNeurons
        {
            get
            {
                return this.outstarCount;
            }
            set
            {
                this.outstarCount = value;
            }
        }

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        public int InputNeurons
        {
            get
            {
                return this.inputCount;
            }
            set
            {
                this.inputCount = value;
            }
        }

    }

}
