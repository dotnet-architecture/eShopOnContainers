using System;
using System.Threading.Tasks;
using Xunit;

namespace eShopOnContainers.UnitTests
{
    public class DummyTests
    {
        [Fact]
        public void ThisShouldPass_Sync()
        {
            Assert.True(true);
        }

        [Fact]
        public async Task ThisShouldPass_Async()
        {
            await Task.Run(() => { Assert.True(true); });
        }

        [Fact]
        public async Task ThisShouldFail_Async()
        {
            await Task.Run(() => { throw new Exception("Oops!"); });
        }
    }
}