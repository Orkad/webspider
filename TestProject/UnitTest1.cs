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
            DataDefinition dataDefinitionCpuBenchmark = new DataDefinition();
            dataDefinitionCpuBenchmark.AddXPathMatching("cpu", "//table[@class=\"desc\"]//span");
            dataDefinitionCpuBenchmark.AddXPathMatching("benchmark", "//table[@class=\"desc\"]//tr[2]/td[2]/span");
            GenericTest.WebPageTest(new Uri("https://www.cpubenchmark.net/cpu.php?cpu=Intel+Xeon+E5-2679+v4+%40+2.50GHz&id=2805"), dataDefinitionCpuBenchmark, new []{ "Intel Xeon E5-2679 v4 @ 2.50GHz","26730" },1000);
        }


        
    }
}
