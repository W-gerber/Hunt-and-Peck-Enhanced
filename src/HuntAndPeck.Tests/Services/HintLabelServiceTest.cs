using HuntAndPeck.Services;
using Xunit;
using System.Linq;

namespace HuntAndPeck.Tests.Services
{
    public class HintLabelServiceTest
    {
        [Fact]
        public void GetHintStrings_UniqueStrings()
        {

            const int hintCount = 256;
            var hintService = new HintLabelService();

            var hints = hintService.GetHintStrings(hintCount);

            Assert.Equal(hintCount, hints.Distinct().Count());
        }
    }
}
