using System;
using Xunit;
using Moq;

namespace Syzoj.Api.Test
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("ThisIsInvalidProblemType")]
        public void ProblemParser_InvalidProblemType_ReturnsNull(string problemType)
        {
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            var problemParserProvider = new ProblemParserProvider(mockServiceProvider.Object);
            Assert.Null(problemParserProvider.GetParser(problemType));
        }
    }
}
