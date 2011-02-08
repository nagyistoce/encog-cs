﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Ninja;
using Encog.Util.CSV;
using Encog.App.Quant.Indicators;
using Encog.App.Quant.Basic;
using Encog.App.Quant.Indicators.Predictive;
using Encog.App.Quant.Normalize;
using Encog.Persist;
using System.IO;
using encog_test.Encog.App.Quant;

namespace TestApp
{
    /// <summary>
    /// This app is really just a "sandbox" to test various things as Encog is developed.
    /// </summary>
    public class Program
    {

        static void Main(string[] args)
        {
            TestYahooDownload t = new TestYahooDownload();
            t.TestYahooDownloadCSV();
            Console.WriteLine("Done");

        }
    }
}
