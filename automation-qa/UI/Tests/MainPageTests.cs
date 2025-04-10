using NUnit.Framework;
using automation_qa.UI.Pages;
using automation_qa.Framework;

namespace automation_qa.UI.Tests
{
    [TestFixture]
    [Category("Main")]
    public class MainPageTests : BaseTest
    {
        private NavigationBar _navigationBar;
        private MainPage _mainPage;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _mainPage = new MainPage(Driver);
        }

        [Test]
        public void MainPage_ShouldDisplayHeader()
        {
            Assert.That(_mainPage.IsHeaderDisplayed(), Is.True, "");
        }

        [Test]
        public void MainPage_ShouldDisplayViewMenuButton()
        {
            Assert.That(_mainPage.IsViewMenuButtonDisplayed(), Is.True, "");
        }

        [Test]
        public void MainPage_ShouldDisplayLocationSection()
        {
            Assert.That(_mainPage.IsLocationSectionDisplayed(), Is.True, "");
        }

        [Test]
        public void MainPage_ShouldDisplayBookTableLink()
        {
            Thread.Sleep(3000);
            Assert.That(_mainPage.IsBookTableLinkDisplayed(), Is.True, "");
        }

        [Test]
        public void MainPage_ShouldDisplayReservationsLink()
        {
            Assert.That(_mainPage.IsReservationsLinkDisplayed(), Is.True, "");
        }

        [Test]
        public void MainPage_ShouldClickMacAndCheese()
        {
            Assert.That(_mainPage.IsMacAndCheeseDisplayed(), Is.True, "");

            _mainPage.ClickMacAndCheese();

            Thread.Sleep(1000);

            Assert.Pass("");
        }

        [Test]
        public void MainPage_ShouldClickChickenTikkaMasala()
        {
            Assert.That(_mainPage.IsChickenTikkaMasalaDisplayed(), Is.True, "");
            _mainPage.ClickChickenTikkaMasala();
            Thread.Sleep(1000);
            Assert.Pass("");
        }

        [Test]
        public void MainPage_ShouldClickCheeseburger()
        {
            Assert.That(_mainPage.IsCheeseburgerDisplayed(), Is.True, "");
            _mainPage.ClickCheeseburger();
            Thread.Sleep(1000);
            Assert.Pass("");
        }

        [Test]
        public void MainPage_ShouldNavigateToLocationOverview()
        {
            _mainPage.SelectLocation();
            Thread.Sleep(1000);
            Assert.Pass("");
        }
    }
}
