using FluentAssertions;
using System;
using Xunit;

namespace XUnitTestProject_reference
{
    public class UnitTest_GuidGen
    {
        [Fact]
        public void Test_GuidGen()
        {
            var guid = Guid.NewGuid().ToString();
            guid.Should().NotBeNullOrEmpty();

        }
    }
}
