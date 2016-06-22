using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSpiderLib;
using WebSpiderLib.Explore;
using WebSpiderLib.Extract;

namespace TestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestPageLoader()
        {
            //WebPageLoader.Load(new Uri("http://google.fr"),page => Assert.Inconclusive());
        }

        [TestMethod]
        public void TestUriEquality()
        {
            Uri uri1 = new Uri("http://google.fr");
            Uri uri2 = new Uri("http://google.fr");
            bool test = uri1 == uri2;
            Assert.IsTrue(test);
            HashSet<Uri> uriSet = new HashSet<Uri> {uri1, uri2};
            Assert.AreEqual(1,uriSet.Count);
        }

        [TestMethod]
        public void CpuBenchmarkDataDefinition()
        {
            var resetEvent = new AutoResetEvent(false);
            Data extractedData = null;
            DataDefinition dataDefinitionCpuBenchmark = new DataDefinition();
            dataDefinitionCpuBenchmark.AddXPathMatching("cpu", "//table[@class=\"desc\"]//span");
            dataDefinitionCpuBenchmark.AddXPathMatching("benchmark", "//table[@class=\"desc\"]//tr[2]/td[2]/span");
            MiningTester tester = new MiningTester(dataDefinitionCpuBenchmark);
            tester.TestResult += data => { extractedData = data;resetEvent.Set(); };
            tester.TryUri(new Uri("https://www.cpubenchmark.net/cpu.php?cpu=Intel+Xeon+E5-2679+v4+%40+2.50GHz&id=2805"));
            Assert.IsTrue(resetEvent.WaitOne(10000),"Timeout");
            Assert.IsNotNull(extractedData);
            //Assert.AreEqual(extractedData.Fields[0].Value = );
        }


        public void GenericWebPageTest(string url, DataDefinition dataDefinition, string[] expectedValues)
        {
            
        }
    }
}
