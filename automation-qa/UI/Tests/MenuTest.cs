using NUnit.Framework;
using automation_qa.UI.Pages;
using automation_qa.Framework;

namespace automation_qa.UI.Tests
{
    [TestFixture]
    [Category("Menu")]
    public class MenuTests : BaseTest
    {
        private LoginPage _loginPage;
        private NavigationBar _navigationBar;
        private MainPage _mainPage;
        private MenuPage _menuPage;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _loginPage = new LoginPage(Driver);
            _navigationBar = new NavigationBar(Driver);

            Driver.Navigate().GoToUrl(BaseConfiguration.UiBaseUrl);
            _navigationBar.GoToLoginPage();

            Console.WriteLine($"LoginPage URL: {Driver.Url}");
            Assert.That(Driver.Url.Contains("signin"), Is.True, "Should be on the login page after navigating");

            string email = ApiTests.Utilities.TestConfig.Instance.TestUserEmail;
            string password = ApiTests.Utilities.TestConfig.Instance.TestUserPassword;
            _loginPage.Login(email, password);

            Thread.Sleep(2000);

            _mainPage = new MainPage(Driver);
            Console.WriteLine($"MainPage URL after login: {Driver.Url}");

            Assert.That(_mainPage.IsHeaderDisplayed(), Is.True, "MainPage header should be displayed after login");
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public void MenuPage_ShouldDisplayMenuTitleAfterClickingViewMenu()
        {
            Assert.That(_mainPage.IsViewMenuButtonClickable(), Is.True, "View menu button should be clickable before proceeding");

            _menuPage = _mainPage.NavigateToMenuPage();
            Thread.Sleep(5000);

            Console.WriteLine($"Current URL after navigating to MenuPage: {Driver.Url}");

            Assert.That(_menuPage.IsMenuPageDisplayed(), Is.True, "Menu page should display the 'Menu' title");
        }

        [Test]
        [Category("Regression")]
        public void MenuPage_ShouldDisplayFiltersAfterClickingViewMenu()
        {
            Assert.That(_mainPage.IsViewMenuButtonClickable(), Is.True, "View menu button should be clickable before proceeding");

            _menuPage = _mainPage.NavigateToMenuPage();
            Thread.Sleep(2000);

            Console.WriteLine($"MenuPage URL: {Driver.Url}");
            Assert.That(_menuPage.AreFiltersDisplayed(), Is.True, "Menu page should display filters (Appetizers, Main Courses, Desserts)");
        }

        [Test]
        [Category("Regression")]
        public void MenuPage_SpecificButtonShouldBeClickable()
        {
            _menuPage = _mainPage.NavigateToMenuPage();
            Thread.Sleep(2000);

            Assert.That(_menuPage.IsSpecificButtonClickable(), Is.True,
                "The specific button should be clickable");

            _menuPage.ClickSpecificButton();
            Thread.Sleep(1000);
        }

        [Test]
        [Category("Regression")]
        public void MenuPage_MainCoursesFilterShouldBeClickable()
        {
            _menuPage = _mainPage.NavigateToMenuPage();
            Thread.Sleep(2000);

            Assert.That(_menuPage.IsMainCoursesButtonClickable(), Is.True,
                "The Main Courses filter button should be clickable");

            _menuPage.ClickMainCoursesButton();
            Thread.Sleep(1000);
        }

        [Test]
        [Category("Regression")]
        public void MenuPage_DessertsFilterShouldBeClickable()
        {
            _menuPage = _mainPage.NavigateToMenuPage();
            Thread.Sleep(2000);

            Assert.That(_menuPage.IsDessertsButtonClickable(), Is.True,
                "The Desserts filter button should be clickable");

            _menuPage.ClickDessertsButton();
            Thread.Sleep(1000);
        }
    }
}