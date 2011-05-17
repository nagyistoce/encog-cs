using System;
using System.IO;
using System.Text;
using Encog.App.Analyst.CSV.Basic;
using Encog.App.Analyst.CSV.Normalize;
using Encog.App.Analyst.Util;
using Encog.App.Quant;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Kmeans;
using Encog.Util.CSV;

namespace Encog.App.Analyst.CSV
{
    /// <summary>
    /// Used by the analyst to cluster a CSV file.
    /// </summary>
    ///
    public class AnalystClusterCSV : BasicFile
    {
        /// <summary>
        /// The analyst to use.
        /// </summary>
        ///
        private EncogAnalyst analyst;

        /// <summary>
        /// The headers.
        /// </summary>
        ///
        private CSVHeaders analystHeaders;

        /// <summary>
        /// The training data used to send to KMeans.
        /// </summary>
        ///
        private BasicMLDataSet data;

        /// <summary>
        /// Analyze the data. This counts the records and prepares the data to be
        /// processed.
        /// </summary>
        ///
        /// <param name="theAnalyst">The analyst to use.</param>
        /// <param name="inputFile">The input file to analyze.</param>
        /// <param name="headers">True, if the input file has headers.</param>
        /// <param name="format">The format of the input file.</param>
        public void Analyze(EncogAnalyst theAnalyst,
                            FileInfo inputFile, bool headers, CSVFormat format)
        {
            InputFilename = inputFile;
            ExpectInputHeaders = headers;
            InputFormat = format;

            Analyzed = true;
            analyst = theAnalyst;

            if (OutputFormat == null)
            {
                OutputFormat = InputFormat;
            }

            data = new BasicMLDataSet();
            ResetStatus();
            int recordCount = 0;

            int outputLength = analyst.DetermineUniqueColumns();
            var csv = new ReadCSV(InputFilename.ToString(),
                                  ExpectInputHeaders, InputFormat);
            ReadHeaders(csv);

            analystHeaders = new CSVHeaders(InputHeadings);

            while (csv.Next() && !ShouldStop())
            {
                UpdateStatus(true);

                var row = new LoadedRow(csv, 1);

                double[] inputArray = AnalystNormalizeCSV.ExtractFields(
                    analyst, analystHeaders, csv, outputLength, true);
                var input = new ClusterRow(inputArray, row);
                data.Add(input);

                recordCount++;
            }
            RecordCount = recordCount;
            Count = csv.ColumnCount;

            ReadHeaders(csv);
            csv.Close();
            ReportDone(true);
        }

        /// <summary>
        /// Prepare the output file, write headers if needed.
        /// </summary>
        ///
        /// <param name="outputFile">The output file.</param>
        /// <param name="input">The number of input columns.</param>
        /// <param name="output">The number of output columns.</param>
        /// <returns>The file to be written to.</returns>
        private StreamWriter PrepareOutputFile(FileInfo outputFile,
                                               int input, int output)
        {
            try
            {
                var tw = new StreamWriter(outputFile.OpenRead());

                // write headers, if needed
                if (ProduceOutputHeaders)
                {
                    var line = new StringBuilder();


                    // handle provided fields, not all may be used, but all should
                    // be displayed
                    foreach (String heading  in  InputHeadings)
                    {
                        AppendSeparator(line, OutputFormat);
                        line.Append("\"");
                        line.Append(heading);
                        line.Append("\"");
                    }

                    // now the output fields that will be generated
                    line.Append("\"cluster\"");

                    tw.WriteLine(line.ToString());
                }

                return tw;
            }
            catch (IOException e)
            {
                throw new QuantError(e);
            }
        }

        /// <summary>
        /// Process the file and cluster.
        /// </summary>
        ///
        /// <param name="outputFile">The output file.</param>
        /// <param name="clusters">The number of clusters.</param>
        /// <param name="theAnalyst">The analyst to use.</param>
        /// <param name="iterations">The number of iterations to use.</param>
        public void Process(FileInfo outputFile, int clusters,
                            EncogAnalyst theAnalyst, int iterations)
        {
            StreamWriter tw = PrepareOutputFile(outputFile, analyst.Script.Normalize.CountActiveFields() - 1, 1);

            ResetStatus();

            var cluster = new KMeansClustering(clusters,
                                               data);
            cluster.Iteration(iterations);

            int clusterNum = 0;

            foreach (MLCluster cl  in  cluster.Clusters)
            {
                foreach (MLData item  in  cl.Data)
                {
                    var row = (ClusterRow) item;
                    int clsIndex = row.Input.Count - 1;
                    LoadedRow lr = row.Row;
                    lr.Data[clsIndex] = "" + clusterNum;
                    WriteRow(tw, lr);
                }
                clusterNum++;
            }

            ReportDone(false);
            tw.Close();
        }
    }
}