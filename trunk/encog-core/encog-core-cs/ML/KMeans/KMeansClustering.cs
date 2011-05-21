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
using System.Linq;
using Encog.ML.Data;
using Encog.ML.Data.Basic;

namespace Encog.ML.Kmeans
{
    /// <summary>
    /// This class performs a basic K-Means clustering. This class can be used on
    /// either supervised or unsupervised data. For supervised data, the ideal values
    /// will be ignored.
    /// http://en.wikipedia.org/wiki/Kmeans
    /// </summary>
    ///
    public class KMeansClustering : IMLClustering
    {
        /// <summary>
        /// The clusters.
        /// </summary>
        ///
        private readonly KMeansCluster[] _clusters;

        /// <summary>
        /// The dataset to cluster.
        /// </summary>
        ///
        private readonly IMLDataSet _set;

        /// <summary>
        /// Within-cluster sum of squares (WCSS).
        /// </summary>
        ///
        private double _wcss;

        /// <summary>
        /// Construct the K-Means object.
        /// </summary>
        ///
        /// <param name="k">The number of clusters to use.</param>
        /// <param name="theSet">The dataset to cluster.</param>
        public KMeansClustering(int k, IMLDataSet theSet)
        {
            _clusters = new KMeansCluster[k];
            for (int i = 0; i < k; i++)
            {
                _clusters[i] = new KMeansCluster();
            }
            _set = theSet;

            SetInitialCentroids();

            // break up the data over the clusters
            int clusterNumber = 0;


            foreach (IMLDataPair pair  in  _set)
            {
                _clusters[clusterNumber].Add(pair.Input);

                clusterNumber++;

                if (clusterNumber >= _clusters.Length)
                {
                    clusterNumber = 0;
                }
            }

            CalcWcss();


            foreach (KMeansCluster element  in  _clusters)
            {
                element.Centroid.CalcCentroid();
            }

            CalcWcss();
        }


        /// <value>Within-cluster sum of squares (WCSS).</value>
        public double WCSS
        {
            get { return _wcss; }
        }

        #region MLClustering Members

        /// <value>The clusters.</value>
        public IMLCluster[] Clusters
        {
            get { return _clusters; }
        }


        /// <summary>
        /// Perform a single training iteration.
        /// </summary>
        ///
        public void Iteration()
        {
            // loop over all clusters
            foreach (KMeansCluster element  in  _clusters)
            {
                for (int k = 0; k < element.Size(); k++)
                {
                    IMLData data = element.Get(k);
                    double distance = CalculateEuclideanDistance(
                        element.Centroid, data);
                    KMeansCluster tempCluster = null;
                    bool match = false;


                    foreach (KMeansCluster cluster  in  _clusters)
                    {
                        double d = CalculateEuclideanDistance(cluster.Centroid,
                                                              element.Get(k));
                        if (distance > d)
                        {
                            distance = d;
                            tempCluster = cluster;
                            match = true;
                        }
                    }

                    if (match)
                    {
                        tempCluster.Add(element.Get(k));
                        element.Remove(element.Get(k));

                        foreach (KMeansCluster element2  in  _clusters)
                        {
                            element2.Centroid.CalcCentroid();
                        }
                        CalcWcss();
                    }
                }
            }
        }

        /// <summary>
        /// The number of iterations to perform.
        /// </summary>
        public void Iteration(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Iteration();
            }
        }


        /// <returns>The number of clusters.</returns>
        public int NumClusters()
        {
            return _clusters.Length;
        }

        #endregion

        /// <summary>
        /// Calculate the euclidean distance between a centroid and data.
        /// </summary>
        ///
        /// <param name="c">The centroid to use.</param>
        /// <param name="data">The data to use.</param>
        /// <returns>The distance.</returns>
        public static double CalculateEuclideanDistance(Centroid c,
                                                        IMLData data)
        {
            double[] d = data.Data;
            double sum = c.Centers.Select((t, i) => Math.Pow(d[i] - t, 2)).Sum();

            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Calculate the within-cluster sum of squares (WCSS).
        /// </summary>
        ///
        private void CalcWcss()
        {
            double temp = _clusters.Aggregate<KMeansCluster, double>(0, (current, element) => current + element.SumSqr);

            _wcss = temp;
        }

        /// <summary>
        /// Get the maximum, over all the data, for the specified index.
        /// </summary>
        ///
        /// <param name="index">An index into the input data.</param>
        /// <returns>The maximum value.</returns>
        private double GetMaxValue(int index)
        {
            double result = Double.MinValue;
            long count = _set.Count;

            for (int i = 0; i < count; i++)
            {
                IMLDataPair pair = BasicMLDataPair.CreatePair(
                    _set.InputSize, _set.IdealSize);
                _set.GetRecord(index, pair);
                result = Math.Max(result, pair.InputArray[index]);
            }
            return result;
        }

        /// <summary>
        /// Get the minimum, over all the data, for the specified index.
        /// </summary>
        ///
        /// <param name="index">An index into the input data.</param>
        /// <returns>The minimum value.</returns>
        private double GetMinValue(int index)
        {
            double result = Double.MaxValue;
            long count = _set.Count;
            IMLDataPair pair = BasicMLDataPair.CreatePair(
                _set.InputSize, _set.IdealSize);

            for (int i = 0; i < count; i++)
            {
                _set.GetRecord(index, pair);
                result = Math.Min(result, pair.InputArray[index]);
            }
            return result;
        }

        /// <summary>
        /// Setup the initial centroids.
        /// </summary>
        ///
        private void SetInitialCentroids()
        {
            for (int n = 1; n <= _clusters.Length; n++)
            {
                var temp = new double[_set.InputSize];
                for (int j = 0; j < temp.Length; j++)
                {
                    temp[j] = (((GetMaxValue(j) - GetMinValue(j))/(_clusters.Length + 1))*n)
                              + GetMinValue(j);
                }
                var c1 = new Centroid(temp);
                _clusters[n - 1].Centroid = c1;
                c1.Cluster = _clusters[n - 1];
            }
        }
    }
}
