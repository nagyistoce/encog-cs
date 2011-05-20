using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.SOM;

namespace Encog.Neural.Som.Training.Clustercopy
{
    /// <summary>
    /// SOM cluster copy is a very simple trainer for SOM's. Using this triner all of
    /// the training data is copied to the SOM weights. This can provide a functional
    /// SOM, or can be used as a starting point for training.
    /// </summary>
    ///
    public class SOMClusterCopyTraining : BasicTraining
    {
        /// <summary>
        /// The SOM to train.
        /// </summary>
        ///
        private readonly SOMNetwork network;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="network_0">The network to train.</param>
        /// <param name="training">The training data.</param>
        public SOMClusterCopyTraining(SOMNetwork network_0, MLDataSet training)
            : base(TrainingImplementationType.OnePass)
        {
            network = network_0;
            Training = training;
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public override MLMethod Method
        {
            get { return network; }
        }

        /// <summary>
        /// Copy the specified input pattern to the weight matrix. This causes an
        /// output neuron to learn this pattern "exactly". This is useful when a
        /// winner is to be forced.
        /// </summary>
        ///
        /// <param name="outputNeuron">The output neuron to set.</param>
        /// <param name="input">The input pattern to copy.</param>
        private void CopyInputPattern(int outputNeuron, MLData input)
        {
            for (int inputNeuron = 0; inputNeuron < network.InputCount; inputNeuron++)
            {
                network.Weights[inputNeuron, outputNeuron] = input[inputNeuron];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            int outputNeuron = 0;

            foreach (MLDataPair pair  in  Training)
            {
                CopyInputPattern(outputNeuron++, pair.Input);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override void Resume(TrainingContinuation state)
        {
        }
    }
}