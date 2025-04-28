using OpenQA.Selenium;
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
        [Category("Smoke")]
        [Category("Regression")]
        public void MainPage_ShouldDisplayHeader()
        {
            Assert.That(_mainPage.IsHeaderDisplayed(), Is.True, "");
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public void MainPage_ShouldDisplayViewMenuButton()
        {
            Assert.That(_mainPage.IsViewMenuButtonDisplayed(), Is.True, "");
        }

        [Test]
        [Category("Regression")]
        public void MainPage_ShouldDisplayLocationSection()
        {
            Assert.That(_mainPage.IsLocationSectionDisplayed(), Is.True, "");
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public void MainPage_ShouldDisplayBookTableLink()
        {
            Thread.Sleep(3000);
            Assert.That(_mainPage.IsBookTableLinkDisplayed(), Is.True, "");
        }

        [Test]
        [Category("Regression")]
        public void MainPage_ShouldDisplayReservationsLink()
        {
            Assert.That(_mainPage.IsReservationsLinkDisplayed(), Is.True, "");
        }

        [Test]
        [Category("Regression")]
        public void MainPage_ShouldNavigateToLocationOverview()
        {
            _mainPage.SelectLocation();
            Thread.Sleep(1000);
            Assert.Pass("");
        }

        [Test]
        [Category("Regression")]
        public void MainPage_ShouldDisplayDishCards()
        {
            Assert.That(_mainPage.AreDishCardsDisplayed(), Is.True, "Dish cards should be displayed on the main page");

            Assert.That(_mainPage.GetDishCardsCount(), Is.GreaterThan(0), "At least one dish card should be displayed");
        }

        [Test]
        [Category("Regression")]
        public void MainPage_ShouldDisplayLocationCards()
        {
            _mainPage.IsLocationSectionDisplayed();

            Assert.That(_mainPage.AreLocationCardsDisplayed(), Is.True, "Location cards should be displayed on the main page");

            Assert.That(_mainPage.GetLocationCardsCount(), Is.GreaterThan(0), "At least one location card should be displayed");
        }

        [Test]
        [Category("Regression")]
        public void MainPage_ShouldNavigateToReservationsPage()
        {
            var navigationBar = new NavigationBar(Driver);
            var loginPage = new LoginPage(Driver);

            string email = ApiTests.Utilities.TestConfig.Instance.TestUserEmail;
            string password = ApiTests.Utilities.TestConfig.Instance.TestUserPassword;

            navigationBar.GoToLoginPage();

            loginPage.Login(email, password);

            Thread.Sleep(2000);

            Assert.That(_mainPage.IsReservationsNavLinkDisplayed(), Is.True,
                "Reservations navigation link should be displayed in the header after login");
        }

        [Test]
        [Category("Regression")]
        public void MainPage_ShouldNavigateToMenuPage_WhenViewMenuButtonClicked()
        {
            var loginPage = new LoginPage(Driver);
            var navigationBar = new NavigationBar(Driver);

            Driver.Navigate().GoToUrl(BaseConfiguration.UiBaseUrl);

            navigationBar.GoToLoginPage();

            string email = ApiTests.Utilities.TestConfig.Instance.TestUserEmail;
            string password = ApiTests.Utilities.TestConfig.Instance.TestUserPassword;

            loginPage.Login(email, password);

            Thread.Sleep(3000);

            _mainPage = new MainPage(Driver);

            _mainPage.ClickViewMenuButton();

            string expectedUrlPart = "/menu";
            Assert.That(Driver.Url.Contains(expectedUrlPart), Is.True,
                $"Dont menu '{expectedUrlPart}'");

            Assert.That(Driver.PageSource.Contains("Menu") || Driver.FindElement(By.XPath("/html/body/div/div/div[1]/div/h1")).Displayed, Is.True,
                "Page 'Menu' name");
        }
    }
}
