using System;
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
            WebPageLoader.Load(new Uri("http://google.fr"),page => Assert.Inconclusive());
        }
    }
}
