using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zeepkist.RandomTrack.Utils;

namespace Zeepkist.RandomTrack.Test
{
    [TestClass]
    public class CsvParserTests
    {
        [TestMethod]
        public void VerifyCsvSplitting()
        {
            var parts = CsvParser.DecodeCsv();
        }
    }
}
