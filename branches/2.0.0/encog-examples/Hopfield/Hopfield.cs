﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData.Bipolar;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Hopfield;
using Encog.Neural.Activation;
using Encog.Util.Logging;
using Encog.Neural.Data;
using Encog.Neural.Data.Basic;

namespace Hopfield
{
    class Hopfield
    {
        /// <summary>
        /// Convert a boolean array to the form [T,T,F,F]
        /// </summary>
        /// <param name="b">A boolen array.</param>
        /// <returns>The boolen array in string form.</returns>
        public static String FormatBoolean(INeuralData b)
        {
            StringBuilder result = new StringBuilder();
            result.Append('[');
            for (int i = 0; i < b.Count; i++)
            {
                if (b[i]>0)
                {
                    result.Append("T");
                }
                else
                {
                    result.Append("F");
                }
                if (i != b.Count - 1)
                {
                    result.Append(",");
                }
            }
            result.Append(']');
            return (result.ToString());
        }


        static void Main(string[] args)
        {
            Logging.StopConsoleLogging();
		
		// Create the neural network.
		BasicLayer hopfield;
		BasicNetwork network = new BasicNetwork();
		network.AddLayer(hopfield = new BasicLayer(new ActivationBiPolar(), false, 4 ));
		hopfield.AddNext(hopfield);
		// This pattern will be trained
		 bool[] pattern1 = { true, true, false, false };
		// This pattern will be presented
		 bool[] pattern2 = { true, false, false, false };
		INeuralData result;
		
		BiPolarNeuralData data1 = new BiPolarNeuralData(pattern1);
		BiPolarNeuralData data2 = new BiPolarNeuralData(pattern2);
		BasicNeuralDataSet set = new BasicNeuralDataSet();
		set.Add(data1);
		network.Structure.FinalizeStructure();

		// train the neural network with pattern1
		Console.WriteLine("Training Hopfield network with: "
				+ FormatBoolean(data1));

		TrainHopfield train = new TrainHopfield(set, network);
		train.Iteration();
		// present pattern1 and see it recognized
		result = network.Compute(data1);
		Console.WriteLine("Presenting pattern:" + FormatBoolean(data1)
				+ ", and got " + FormatBoolean(result));
		// Present pattern2, which is similar to pattern 1. Pattern 1
		// should be recalled.
		result = network.Compute(data2);
		Console.WriteLine("Presenting pattern:" + FormatBoolean(data2)
				+ ", and got " + FormatBoolean(result));
        }
    }
}
