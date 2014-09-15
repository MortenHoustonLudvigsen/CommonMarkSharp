using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp.Tests
{
    [TestClass]
    public class CharSetTests
    {
        [TestMethod]
        [TestCategory("Parser - CharSet")]
        public void CharSetCanCreateContains()
        {
            // Arrange
            var chars = "abcABC";

            // Act
            CharSet actual = chars;

            // Assert
        }

        [TestMethod]
        [TestCategory("Parser - CharSet")]
        public void CharSetContains()
        {
            // Arrange
            CharSet chars = "abcABC";

            // Act
            var actual = chars.Contains('B');

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        [TestCategory("Parser - CharSet")]
        public void CharSetNotContains()
        {
            // Arrange
            CharSet chars = "abcABC";

            // Act
            var actual = chars.Contains('X');

            // Assert
            Assert.IsFalse(actual);
        }
    }
}
