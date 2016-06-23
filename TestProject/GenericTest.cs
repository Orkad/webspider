using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSpiderLib.Explore;
using WebSpiderLib.Extract;

namespace TestProject
{
    public class GenericTest
    {
        public static void WebPageTest(Uri uri, DataDefinition dataDefinition, string[] expectedValues, int timeout)
        {
            var resetEvent = new AutoResetEvent(false);
            Data extractedData = null;
            WebExtractor extractor = new WebExtractor(dataDefinition);
            extractor.SuccessParse += data =>
            {
                extractedData = data;
                resetEvent.Set();
            };
            extractor.ErrorParse += Assert.Fail;
            WebPageLoader.Load(uri, page => extractor.Extract(page.Html), _ => Assert.Fail());
            Assert.IsTrue(resetEvent.WaitOne(timeout), "Timeout");
            Assert.IsNotNull(extractedData);
            for (int i = 0; i < expectedValues.Length; i++)
            {
                Assert.AreEqual(expectedValues[i], extractedData.Fields[i].Value);
            }
        }


    }
}
