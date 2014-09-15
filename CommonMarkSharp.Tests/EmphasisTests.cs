using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMarkSharp.Tests
{
    [TestClass]
    public class EmphasisTests
    {
        private CommonMark _cm = new CommonMark();

        [TestMethod]
        [TestCategory("Inlines - Emphasis and strong emphasis")]
        public void UnderscoreWithinEmphasis()
        {
            // See https://github.com/jgm/stmd/issues/51 for additional info
            // The rule is that inlines are processed left-to-right

            // Arrange
            var commonMark = Helpers.Normalize("*_*_");
            var expected = Helpers.Normalize("<p><em>_</em>_</p>");

            // Act
            var actual = _cm.RenderAsHtml(commonMark);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
        }
    }
}
