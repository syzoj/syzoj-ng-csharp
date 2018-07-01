using System;
using System.Linq;
using Syzoj.Api.Utils;
using Xunit;

namespace Syzoj.Api.Test
{
    public class UtilsTest
    {
        [Fact]
        public void AwaysTure()
        {
            Assert.Equal(5, 5);
        }

        [Theory]
        [InlineData("1232`423434")]
        [InlineData("salfdhaf;lash")]
        public void PasswordIsSame(string password)
        {
            var (salt, hash) = HashUtils.GenerateHashedPassword(password);

            Assert.True(HashUtils.VerifyHash(salt, hash, password));
        }
    }
}
