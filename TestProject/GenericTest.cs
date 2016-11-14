using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSpiderLib.Explore;
using WebSpiderLib.Extract;

namespace TestProject
{
    public class GenericTest
    {
        public static void ExtractWebPageTest(WebPage webPage, DataDefinition dataDefinition, string[] expectedValues)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(webPage.Html);
            Data extractedData = dataDefinition.Parse(doc);
            Assert.IsNotNull(extractedData);
            for (int i = 0; i < expectedValues.Length; i++)
            {
                Assert.AreEqual(expectedValues[i], extractedData.Fields[i].Value, "la valeur extraite ne correspond pas pour le champ : " + extractedData.Fields[i].Name);
            }
        }

        public static WebPage LoadWebPageTest(Uri uri, int timeout = 10000)
        {
            var resetEvent = new AutoResetEvent(false);
            WebPage webpage = null;
            
            Assert.IsTrue(resetEvent.WaitOne(timeout), "Timeout");
            Assert.IsNotNull(webpage);
            return webpage;
        }

        public static void XPathTest(Uri uri, string fieldname, string xpath, string expected)
        {
            DataDefinition dataDefinitionCpuBenchmark = new DataDefinition();
            dataDefinitionCpuBenchmark.AddXPathMatching(fieldname, xpath);
            WebPage page = GenericTest.LoadWebPageTest(uri);
            GenericTest.ExtractWebPageTest(page, dataDefinitionCpuBenchmark, new[] { expected });
        }
    }
}
