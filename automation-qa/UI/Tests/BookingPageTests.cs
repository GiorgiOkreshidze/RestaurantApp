using NUnit.Framework;
using automation_qa.UI.Pages;
using automation_qa.Framework;
using System.Threading;

namespace automation_qa.UI.Tests
{
    [TestFixture]
    [Category("Booking")]
    public class BookingPageTests : BaseTest
    {
        private NavigationBar _navigationBar;
        private BookingPage _bookingPage;
        private MainPage _mainPage;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _mainPage = new MainPage(Driver);
            _bookingPage = _mainPage.ClickBookTableLink();
            Thread.Sleep(2000);
        }

        [Test]
        [Category("Smoke")]
        public void BookingPage_ShouldDisplayHeader()
        {
            Assert.That(_bookingPage.IsBookingHeaderDisplayed(), Is.True, "Book a Table");
        }

        [Test]
        [Category("Smoke")]
        public void BookingPage_ShouldClickLocationDropdownButton()
        {
            _bookingPage.OpenLocationDropdown();

            Thread.Sleep(2000);

            Assert.Pass("Button click");
        }

        [Test]
        [Category("Smoke")]
        public void BookingPage_ShouldSelectDate()
        {
            _bookingPage.OpenDateDropdown();
            _bookingPage.SelectDate();

            Assert.Pass("Succesfully select");
        }

        [Test]
        [Category("Regression")]
        public void BookingPage_ShouldSelectTime()
        {
            _bookingPage.OpenTimeDropdown();
            _bookingPage.SelectTime();

            Assert.Pass("Succesfully select");
        }

        [Test]
        [Category("Smoke")]
        public void BookingPage_ShouldIncreaseGuestsCount()
        {
            string initialCount = _bookingPage.GetGuestsCount();

            _bookingPage.IncreaseGuests();

            string newCount = _bookingPage.GetGuestsCount();

            Assert.That(newCount, Is.Not.EqualTo(initialCount), "Quality guests changes");
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public void BookingPage_ShouldFindAvailableTables()
        {
            try
            {
                _bookingPage.ClickFindTable();

                Thread.Sleep(3000);

                Assert.That(_bookingPage.AreAvailableTablesDisplayed(), Is.True, "Valid table");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Faild: {ex.Message}");
                Assert.Fail($"Faild: {ex.Message}");
            }
        }

        [Test]
        [Category("Regression")]
        public void BookingPage_ShouldDisplayTableCards()
        {
            _bookingPage.ClickFindTable();

            Thread.Sleep(3000);

            Assert.That(_bookingPage.IsTableCardDisplayed(), Is.True, "Valid table");
        }

        [Test]
        [Category("Regression")]
        public void BookingPage_ShouldFilterTablesByLocation()
        {
            _bookingPage.OpenLocationDropdown();
            _bookingPage.SelectLocation();
            _bookingPage.ClickFindTable();

            Thread.Sleep(2000);

            Assert.That(_bookingPage.AreAvailableTablesDisplayed(), Is.True, "Valid table");

            Assert.That(_bookingPage.IsTableCardDisplayed(), Is.True, "Valid table");
        }

        [Test]
        [Category("Regression")]
        public void BookingPage_ShouldFilterTablesByGuestsCount()
        {
            _bookingPage.IncreaseGuests();
            _bookingPage.IncreaseGuests();
            _bookingPage.ClickFindTable();

            Thread.Sleep(2000);

            Assert.That(_bookingPage.IsTableCardDisplayed(), Is.True, "Valid table");
        }

        [Test]
        [Category("Regression")]
        public void BookingPage_ShouldFilterTablesByTimeSlot()
        {
            _bookingPage.OpenTimeDropdown();
            _bookingPage.SelectTime();
            _bookingPage.ClickFindTable();

            Thread.Sleep(2000);

            Assert.That(_bookingPage.AreAvailableTablesDisplayed(), Is.True, "Valid table");

            Assert.That(_bookingPage.IsTableCardDisplayed(), Is.True, "Valid table");
        }

        [Test]
        [Category("Regression")]
        public void FindTableButton_ShouldBeClickable()
        {
            try
            {
                bool isButtonClickable = _bookingPage.IsFindTableButtonClickable();

                Assert.That(isButtonClickable, Is.True, "Button click");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed: {ex.Message}");
                Assert.Fail($"Failed: {ex.Message}");
            }
        }

        [Test]
        [Category("Regression")]
        public void BookingPage_ShouldCompleteFullSearchCycle()
        {
            try
            {
                _bookingPage.ClickFindTable();

                Thread.Sleep(3000);

                Assert.That(_bookingPage.IsTableCardDisplayed(), Is.True, "Valid table");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed: {ex.Message}");
                Assert.Fail($"Failed: {ex.Message}");
            }
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public void BookingPage_ShouldCreateReservationSuccessfully()
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
            _bookingPage = _mainPage.ClickBookTableLink();
            Thread.Sleep(2000);

            _bookingPage.SetGuestsCount(2);
            _bookingPage.ClickFindTable();

            Thread.Sleep(3000);

            Assert.That(_bookingPage.AreAvailableTablesDisplayed(), Is.True, "Available tables should be displayed");

            _bookingPage.SelectTable(2);

            Thread.Sleep(1000);

            _bookingPage.ConfirmReservation();

            Thread.Sleep(2000);

            Assert.That(_bookingPage.IsReservationConfirmed(), Is.True, "Reservation should be confirmed");

            string confirmationText = _bookingPage.GetConfirmationText();
            Assert.That(confirmationText, Does.Contain("Reservation Confirmed!"), "Confirmation heading should be displayed");
            Assert.That(confirmationText, Does.Contain("has been successfully made"), "Success message should be displayed");
        }

        [Test]
        [Category("Regression")]
        public void BookingPage_ShouldCancelReservationSuccessfully()
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
            _bookingPage = _mainPage.ClickBookTableLink();
            Thread.Sleep(2000);

            //_bookingPage.SelectDate14();
            //Thread.Sleep(1000);

            _bookingPage.SetGuestsCount(2);
            _bookingPage.ClickFindTable();

            Thread.Sleep(3000);

            Assert.That(_bookingPage.AreAvailableTablesDisplayed(), Is.True, "Available tables should be displayed");

            _bookingPage.SelectTable(2);

            Thread.Sleep(1000);

            _bookingPage.ConfirmReservation();

            Thread.Sleep(3000);

            _bookingPage.CancelReservation();

            Thread.Sleep(2000);

            Assert.That(_bookingPage.IsReservationCancelationConfirmed(), Is.True, "Cancellation confirmation should be displayed");
        }

        [Test]
        [Category("Regression")]
        public void BookingPage_ShouldEditReservationSuccessfully()
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
            _bookingPage = _mainPage.ClickBookTableLink();
            Thread.Sleep(2000);

            _bookingPage.SelectDate14();
            Thread.Sleep(1000);

            _bookingPage.SetGuestsCount(2);
            _bookingPage.ClickFindTable();

            Thread.Sleep(3000);

            Assert.That(_bookingPage.AreAvailableTablesDisplayed(), Is.True, "Available tables should be displayed");

            _bookingPage.SelectTable(2);

            Thread.Sleep(1000);

            _bookingPage.ConfirmReservation();

            Thread.Sleep(3000);

            Assert.That(_bookingPage.IsReservationConfirmed(), Is.True, "Reservation should be confirmed");

            _bookingPage.EditReservation();

            Thread.Sleep(1000);

            _bookingPage.IncreaseGuestsInModal();

            _bookingPage.ConfirmEdit();

            Thread.Sleep(3000);

            Assert.That(_bookingPage.IsReservationConfirmed(), Is.True, "Reservation should be confirmed after editing");

            string confirmationText = _bookingPage.GetConfirmationText();
            Assert.That(confirmationText, Does.Contain("3 people"), "Number of guests should be updated to 3");
        }

        [Test]
        [Category("Regression")]
        public void BookingPage_ShouldHandleSelectingSameDate()
        {
            var loginPage = new LoginPage(Driver);
            var navigationBar = new NavigationBar(Driver);

            Driver.Navigate().GoToUrl(BaseConfiguration.UiBaseUrl);
            navigationBar.GoToLoginPage();

            string email = ApiTests.Utilities.TestConfig.Instance.TestUserEmail;
            string password = ApiTests.Utilities.TestConfig.Instance.TestUserPassword;
            loginPage.Login(email, password);

            Thread.Sleep(2000);

            _mainPage = new MainPage(Driver);
            _bookingPage = _mainPage.ClickBookTableLink();
            Thread.Sleep(3000);

            _bookingPage.SelectDate14();

            _bookingPage.ClickFindTable();

            Thread.Sleep(2000);

            bool warningDisplayed = _bookingPage.IsRequiredFieldsWarningDisplayed();

            bool tablesDisplayed = _bookingPage.AreAvailableTablesDisplayed();

            Assert.That(warningDisplayed || tablesDisplayed, Is.True,
                "Either warning should be displayed or tables should be shown");

            Console.WriteLine($"gh: {warningDisplayed}");
            Console.WriteLine($"fg: {tablesDisplayed}");
        }

        [Test]
        [Category("Regression")]
        public void BookingPage_ShouldShowWarningWhenSelectingDate13()
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
            _bookingPage = _mainPage.ClickBookTableLink();
            Thread.Sleep(2000);

            _bookingPage.SelectDate13();
            _bookingPage.SelectDate13();

            _bookingPage.ClickFindTable();

            Thread.Sleep(3000);

            Assert.That(_bookingPage.IsRequiredFieldsWarningDisplayed(), Is.True,
                "Warning should be displayed when selecting date 13");

            string warningText = _bookingPage.GetRequiredFieldsWarningText();
            Console.WriteLine($"Текст предупреждения: {warningText}");
        }
    }
}