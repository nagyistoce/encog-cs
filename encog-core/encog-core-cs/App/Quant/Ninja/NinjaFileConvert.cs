using System.IO;
using System.Text;
using Encog.App.Analyst.CSV.Basic;
using Encog.Util.CSV;

namespace Encog.App.Quant.Ninja
{
    /// <summary>
    /// A simple class to convert financial data files into the format used by NinjaTrader for
    /// input.
    /// </summary>
    public class NinjaFileConvert : BasicCachedFile
    {
        /// <summary>
        /// Process the file and output to the target file.
        /// </summary>
        /// <param name="target">The target file to write to.</param>
        public void Process(string target)
        {
            var csv = new ReadCSV(InputFilename.ToString(), ExpectInputHeaders, InputFormat);
            TextWriter tw = new StreamWriter(target);

            ResetStatus();
            while (csv.Next())
            {
                var line = new StringBuilder();
                UpdateStatus(false);
                line.Append(GetColumnData(FileData.Date, csv));
                line.Append(" ");
                line.Append(GetColumnData(FileData.Time, csv));
                line.Append(";");
                line.Append(InputFormat.Format(double.Parse(GetColumnData(FileData.Open, csv)), Precision));
                line.Append(";");
                line.Append(InputFormat.Format(double.Parse(GetColumnData(FileData.High, csv)), Precision));
                line.Append(";");
                line.Append(InputFormat.Format(double.Parse(GetColumnData(FileData.Low, csv)), Precision));
                line.Append(";");
                line.Append(InputFormat.Format(double.Parse(GetColumnData(FileData.Close, csv)), Precision));
                line.Append(";");
                line.Append(InputFormat.Format(double.Parse(GetColumnData(FileData.Volume, csv)), Precision));

                tw.WriteLine(line.ToString());
            }
            ReportDone(false);
            csv.Close();
            tw.Close();
        }

        /// <summary>
        /// Analyze the input file.
        /// </summary>
        /// <param name="input">The name of the input file.</param>
        /// <param name="headers">True, if headers are present.</param>
        /// <param name="format">The format of the input file.</param>
        public override void Analyze(FileInfo input, bool headers, CSVFormat format)
        {
            base.Analyze(input, headers, format);
        }
    }
}