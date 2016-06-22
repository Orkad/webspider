using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSpiderLib.Explore;
using WebSpiderLib.Extract;

namespace WebSpiderLib
{
    public class MiningTester
    {
        public DataDefinition DataDefinition { get; }

        public event Action<Data> TestResult;

        public MiningTester(DataDefinition dataDefinition)
        {
            DataDefinition = dataDefinition;
        }

        public void TryUri(Uri uri)
        {
            WebPageLoader.Load(uri,LoadSuccess,LoadError);
        }

        private void LoadError(Uri uri)
        {
            TestResult?.Invoke(null);
        }

        private void LoadSuccess(WebPage webPage)
        {
            WebExtractor extractor = new WebExtractor(DataDefinition);
            extractor.SuccessParse += ExtractorOnSuccessParse;
            extractor.ErrorParse += ExtractorOnErrorParse;
            extractor.Extract(webPage.Html);
        }

        private void ExtractorOnErrorParse()
        {
            TestResult?.Invoke(null);
        }

        private void ExtractorOnSuccessParse(Data data)
        {
            TestResult?.Invoke(data);
        }
    }
}
