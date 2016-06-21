using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSpiderLib.Explore;

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
    }
}
