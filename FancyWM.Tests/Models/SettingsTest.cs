using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FancyWM.Models.Tests
{
    [TestClass]
    public class SettingsTest
    {
        [TestMethod]
        public void TestTitleIgnoreListDefaultsToEmptyWhenMissing()
        {
            var settings = JsonSerializer.Deserialize<Settings>("""{ "ProcessIgnoreList": ["Taskmgr"] }""")!;
            Assert.IsNotNull(settings.TitleIgnoreList);
            Assert.AreEqual(0, settings.TitleIgnoreList.Count);
        }

        [TestMethod]
        public void TestTitleIgnoreListRoundTrips()
        {
            var settings = new Settings { TitleIgnoreList = ["*.maxhelp", "/^Jupyter/"] };
            var result = JsonSerializer.Deserialize<Settings>(JsonSerializer.Serialize(settings))!;
            CollectionAssert.AreEqual(settings.TitleIgnoreList, result.TitleIgnoreList);
        }
    }
}
