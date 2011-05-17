using System;
using System.IO;
using System.Text;
using Encog.App.Analyst.CSV.Basic;
using Encog.App.Analyst.Script.Normalize;
using Encog.App.Quant;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Util.CSV;

namespace Encog.App.Analyst.CSV
{
    /// <summary>
    /// Used by the analyst to evaluate a CSV file.
    /// </summary>
    ///
    public class AnalystEvaluateRawCSV : BasicFile
    {
        /// <summary>
        /// The analyst file to use.
        /// </summary>
        ///
        private EncogAnalyst analyst;

        /// <summary>
        /// The ideal count.
        /// </summary>
        ///
        private int idealCount;

        /// <summary>
        /// The input count.
        /// </summary>
        ///
        private int inputCount;

        /// <summary>
        /// The output count.
        /// </summary>
        ///
        private int outputCount;

        /// <summary>
        /// Analyze the data. This counts the records and prepares the data to be
        /// processed.
        /// </summary>
        ///
        /// <param name="theAnalyst">The analyst to use.</param>
        /// <param name="inputFile">The input file.</param>
        /// <param name="headers">True if headers are present.</param>
        /// <param name="format">The format the file is in.</param>
        public void Analyze(EncogAnalyst theAnalyst,
                            FileInfo inputFile, bool headers, CSVFormat format)
        {
            InputFilename = inputFile;
            ExpectInputHeaders = headers;
            InputFormat = format;
            analyst = theAnalyst;

            Analyzed = true;

            PerformBasicCounts();

            inputCount = analyst.DetermineInputCount();
            outputCount = analyst.DetermineOutputCount();
            idealCount = InputHeadings.Length - inputCount;

            if ((InputHeadings.Length != inputCount)
                && (InputHeadings.Length != (inputCount + outputCount)))
            {
                throw new AnalystError("Invalid number of columns("
                                       + InputHeadings.Length + "), must match input("
                                       + inputCount + ") count or input+output("
                                       + (inputCount + outputCount) + ") count.");
            }
        }

        /// <summary>
        /// Prepare the output file, write headers if needed.
        /// </summary>
        ///
        /// <param name="outputFile">The name of the output file.</param>
        /// <param name="method"></param>
        /// <returns>The output stream for the text file.</returns>
        private StreamWriter AnalystPrepareOutputFile(FileInfo outputFile)
        {
            try
            {
                var tw = new StreamWriter(outputFile.OpenRead());
                // write headers, if needed
                if (ProduceOutputHeaders)
                {
                    var line = new StringBuilder();


                    // first handle the input fields
                    foreach (AnalystField field  in  analyst.Script.Normalize.NormalizedFields)
                    {
                        if (field.Input)
                        {
                            field.AddRawHeadings(line, null, OutputFormat);
                        }
                    }

                    // now, handle any ideal fields
                    if (idealCount > 0)
                    {
                        foreach (AnalystField field_0  in  analyst.Script.Normalize.NormalizedFields)
                        {
                            if (field_0.Output)
                            {
                                field_0.AddRawHeadings(line, "ideal:",
                                                       OutputFormat);
                            }
                        }
                    }


                    // now, handle the output fields
                    foreach (AnalystField field_1  in  analyst.Script.Normalize.NormalizedFields)
                    {
                        if (field_1.Output)
                        {
                            field_1.AddRawHeadings(line, "output:", OutputFormat);
                        }
                    }

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
        /// Process the file.
        /// </summary>
        ///
        /// <param name="outputFile">The output file.</param>
        /// <param name="method">The method to use.</param>
        public void Process(FileInfo outputFile, MLRegression method)
        {
            var csv = new ReadCSV(InputFilename.ToString(),
                                  ExpectInputHeaders, InputFormat);

            if (method.InputCount != inputCount)
            {
                throw new AnalystError("This machine learning method has "
                                       + method.InputCount
                                       + " inputs, however, the data has " + inputCount
                                       + " inputs.");
            }

            MLData output = null;
            MLData input = new BasicMLData(method.InputCount);

            StreamWriter tw = AnalystPrepareOutputFile(outputFile);

            ResetStatus();
            while (csv.Next())
            {
                UpdateStatus(false);
                var row = new LoadedRow(csv, idealCount);

                int dataIndex = 0;
                // load the input data
                for (int i = 0; i < inputCount; i++)
                {
                    String str = row.Data[i];
                    double d = InputFormat.Parse(str);
                    input[i] = d;
                    dataIndex++;
                }

                // do we need to skip the ideal values?
                dataIndex += idealCount;

                // compute the result
                output = method.Compute(input);

                // display the computed result
                for (int i_0 = 0; i_0 < outputCount; i_0++)
                {
                    double d_1 = output[i_0];
                    row.Data[dataIndex++] = InputFormat.Format(d_1,
                                                               Precision);
                }

                WriteRow(tw, row);
            }
            ReportDone(false);
            tw.Close();
            csv.Close();
        }
    }
}