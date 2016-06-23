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
    public class WebTesting
    {
        [TestMethod]
        public void TestPageLoader()
        {
            GenericTest.LoadWebPageTest(new Uri("http://www.rueducommerce.fr/Composants/Processeur/Processeur-INTEL/INTEL/4960133-Core-i7-6950X-3-00GHz.htm"));
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
        public void TestUriBaseOf()
        {
            Uri uri1 = new Uri("http://google.fr");
            Uri uri2 = new Uri("http://google.fr/test");
            uri1.IsBaseOf(uri2);
        }

        [TestMethod]
        public void CpuBenchmarkDataDefTest()
        {
            Uri cpuBenchmarkUri = new Uri("https://www.cpubenchmark.net/cpu.php?cpu=Intel+Xeon+E5-2679+v4+%40+2.50GHz&id=2805");
            GenericTest.XPathTest(cpuBenchmarkUri, "cpu","//table[@class=\"desc\"]//span", "Intel Xeon E5-2679 v4 @ 2.50GHz");
            GenericTest.XPathTest(cpuBenchmarkUri, "benchmark","//table[@class=\"desc\"]//tr[2]/td[2]/span", "26730");
        }

        [TestMethod]
        public void RdcDataDefTest()
        {
            Uri rdcUriTest = new Uri("http://www.rueducommerce.fr/Composants/Processeur/Processeur-INTEL/INTEL/4960133-Core-i7-6950X-3-00GHz.htm");
            GenericTest.XPathTest(rdcUriTest, "ref", "//h3[@class='ficheProduit_reference']", "Réf : BX80671I76950X");
            GenericTest.XPathTest(rdcUriTest,"name", "//div[@class=\"ficheProduit_brandName\"]/../h2", "Processeur Intel Core i7-6950X 3.00GHz LGA2011-V3 BOX - Broadwell E");
            GenericTest.XPathTest(rdcUriTest, "price", ".//*[@class='newPrice']", "1 799,90 1 799€90");
        }
        
    }
}
