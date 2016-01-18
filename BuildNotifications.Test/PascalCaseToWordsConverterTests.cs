using BuildNotifications.Converters;
using BuildNotifications.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildNotifications.Test
{
    [TestClass]
    public class PascalCaseToWordsConverterTests
    {
        private PascalCaseToWordsConverter _converter;

        [TestInitialize]
        public void TestSetup()
        {
            _converter = new PascalCaseToWordsConverter();
        }

        [TestMethod]
        public void Convert_Null_ReturnsNull()
        {
            BuildResult? input = null;
            string expectedOutput = null;

            string output = (string) _converter.Convert(input, typeof (string), null, null);

            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        public void Convert_OneWord_ReturnsSame()
        {
            BuildResult input = BuildResult.Succeeded;
            string expectedOutput = "Succeeded";

            string output = (string) _converter.Convert(input, typeof (string), null, null);

            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        public void Convert_MultipleWords_ReturnsSpaceSeparatedWords()
        {
            BuildResult input = BuildResult.PartiallySucceeded;
            string expectedOutput = "Partially Succeeded";

            string output = (string) _converter.Convert(input, typeof (string), null, null);

            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        public void Convert_MultipleWordsIncludingSingleLetterWord_ReturnsSpaceSeparatedWords()
        {
            string input = "ThisIsALongStringOfWords";
            string expectedOutput = "This Is A Long String Of Words";

            string output = (string) _converter.Convert(input, typeof (string), null, null);

            Assert.AreEqual(expectedOutput, output);
        }
    }
}
