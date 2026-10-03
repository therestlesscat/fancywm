
using FancyWM.Tests.TestUtilities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FancyWM.Utilities.Tests
{
    [TestClass]
    public class WindowMatcherTest
    {
        private readonly WindowMockFactory m_mockFactory = new();

        [TestMethod]
        public void TestByProcessNameExact()
        {
            var matcher = new ByProcessNameMatcher("explorer");
            Assert.IsTrue(matcher.Matches(m_mockFactory.CreateExplorerWindow()));
        }

        [TestMethod]
        public void TestByProcessNameInExact()
        {
            var matcher = new ByProcessNameMatcher("ExPlOrEr");
            Assert.IsTrue(matcher.Matches(m_mockFactory.CreateExplorerWindow()));
        }


        [TestMethod]
        public void TestByProcessNameFails()
        {
            var matcher = new ByProcessNameMatcher("explorer.something");
            Assert.IsFalse(matcher.Matches(m_mockFactory.CreateExplorerWindow()));
        }

        [TestMethod]
        public void TestByWindowTitleExact()
        {
            var matcher = new ByWindowTitleMatcher("uNtItLeD - nOtEpAd");
            Assert.IsTrue(matcher.Matches(m_mockFactory.CreateNotepadWindow()));
        }

        [TestMethod]
        public void TestByWindowTitleMustMatchWholeTitle()
        {
            var matcher = new ByWindowTitleMatcher("Notepad");
            Assert.IsFalse(matcher.Matches(m_mockFactory.CreateNotepadWindow()));
        }

        [TestMethod]
        public void TestByWindowTitleStarWildcard()
        {
            Assert.IsTrue(new ByWindowTitleMatcher("*Notepad").Matches(m_mockFactory.CreateNotepadWindow()));
            Assert.IsTrue(new ByWindowTitleMatcher("Untitled*").Matches(m_mockFactory.CreateNotepadWindow()));
            Assert.IsTrue(new ByWindowTitleMatcher("*titled - Note*").Matches(m_mockFactory.CreateNotepadWindow()));
            Assert.IsTrue(new ByWindowTitleMatcher("*").Matches(m_mockFactory.CreateNotepadWindow()));
            Assert.IsFalse(new ByWindowTitleMatcher("*Word").Matches(m_mockFactory.CreateNotepadWindow()));
        }

        [TestMethod]
        public void TestByWindowTitleQuestionMarkWildcard()
        {
            Assert.IsTrue(new ByWindowTitleMatcher("This P?").Matches(m_mockFactory.CreateExplorerWindow()));
            Assert.IsFalse(new ByWindowTitleMatcher("This P??").Matches(m_mockFactory.CreateExplorerWindow()));
        }

        [TestMethod]
        public void TestByWindowTitleEscapesRegexCharacters()
        {
            Assert.IsFalse(new ByWindowTitleMatcher("Untitled . Notepad").Matches(m_mockFactory.CreateNotepadWindow()));
            Assert.IsFalse(new ByWindowTitleMatcher("Discor[d]").Matches(m_mockFactory.CreateDiscordWindow()));
        }

        [TestMethod]
        public void TestByWindowTitleRegex()
        {
            Assert.IsTrue(new ByWindowTitleMatcher("/^untitled/").Matches(m_mockFactory.CreateNotepadWindow()));
            Assert.IsTrue(new ByWindowTitleMatcher("/Note/").Matches(m_mockFactory.CreateNotepadWindow()));
            Assert.IsFalse(new ByWindowTitleMatcher("/^Notepad/").Matches(m_mockFactory.CreateNotepadWindow()));
        }

        [TestMethod]
        public void TestByWindowTitleInvalidRegexNeverMatches()
        {
            Assert.IsFalse(new ByWindowTitleMatcher("/(/").Matches(m_mockFactory.CreateNotepadWindow()));
        }

        [TestMethod]
        public void TestByWindowTitleEmptyNeverMatches()
        {
            Assert.IsFalse(new ByWindowTitleMatcher("").Matches(m_mockFactory.CreateNotepadWindow()));
        }
    }
}
