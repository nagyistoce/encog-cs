//
// Encog(tm) Unit Tests v3.0 - .Net Version
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
using System.IO;
using Encog.App.Analyst.CSV.Balance;
using Encog.Util;
using Encog.Util.CSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.App.CSV
{
    [TestClass]
    public class TestBalanceCSV
    {
        public static readonly TempDir TempDir = new TempDir();
        public readonly FileInfo InputName = TempDir.CreateFile("test.csv");
        public readonly FileInfo OutputName = TempDir.CreateFile("test2.csv");

        public void GenerateTestFile(bool header)
        {
            var file = new StreamWriter(InputName.ToString());

            if (header)
            {
                file.WriteLine("a,b");
            }
            file.WriteLine("one,1");
            file.WriteLine("two,1");
            file.WriteLine("three,1");
            file.WriteLine("four,2");
            file.WriteLine("five,2");
            file.WriteLine("six,3");

            // close the stream
            file.Close();
        }

        [TestMethod]
        public void TestBalanceCSVHeaders()
        {
            GenerateTestFile(true);
            var norm = new BalanceCSV();
            norm.Analyze(InputName, true, CSVFormat.ENGLISH);
            norm.Process(OutputName, 1, 2);

            var tr = new StreamReader(OutputName.ToString());

            Assert.AreEqual("\"a\",\"b\"", tr.ReadLine());
            Assert.AreEqual("one,1", tr.ReadLine());
            Assert.AreEqual("two,1", tr.ReadLine());
            Assert.AreEqual("four,2", tr.ReadLine());
            Assert.AreEqual("five,2", tr.ReadLine());
            Assert.AreEqual("six,3", tr.ReadLine());
            Assert.AreEqual(2, norm.Counts["1"]);
            Assert.AreEqual(2, norm.Counts["2"]);
            Assert.AreEqual(1, norm.Counts["3"]);
            tr.Close();

            InputName.Delete();
            OutputName.Delete();
        }

        [TestMethod]
        public void TestBalanceCSVNoHeaders()
        {
            GenerateTestFile(false);
            var norm = new BalanceCSV();
            norm.Analyze(InputName, false, CSVFormat.ENGLISH);
            norm.Process(OutputName, 1, 2);

            var tr = new StreamReader(OutputName.ToString());
            Assert.AreEqual("\"field:0\",\"field:1\"", tr.ReadLine());
            Assert.AreEqual("one,1", tr.ReadLine());
            Assert.AreEqual("two,1", tr.ReadLine());
            Assert.AreEqual("four,2", tr.ReadLine());
            Assert.AreEqual("five,2", tr.ReadLine());
            Assert.AreEqual("six,3", tr.ReadLine());
            Assert.AreEqual(2, norm.Counts["1"]);
            Assert.AreEqual(2, norm.Counts["2"]);
            Assert.AreEqual(1, norm.Counts["3"]);
            tr.Close();

            InputName.Delete();
            OutputName.Delete();
        }
    }
}
