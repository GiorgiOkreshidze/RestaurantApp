using NUnit.Framework;
using automation_qa.UI.Pages;
using automation_qa.Framework;
using System.Threading;
using System;
using OpenQA.Selenium;

namespace automation_qa.UI.Tests
{
    [TestFixture]
    [Category("Reservations")]
    public class ReservationPageTests : BaseTest
    {
        private NavigationBar _navigationBar;
        private ReservationPage _reservationPage;
        private LoginPage _loginPage;
        private MainPage _mainPage;
        private BookingPage _bookingPage;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _navigationBar = new NavigationBar(Driver);
            _loginPage = new LoginPage(Driver);
            _mainPage = new MainPage(Driver);
        }

        public void LoginUser()
        {
            Driver.Navigate().GoToUrl(BaseConfiguration.UiBaseUrl);
            _navigationBar.GoToLoginPage();

            string email = ApiTests.Utilities.TestConfig.Instance.TestUserEmail;
            string password = ApiTests.Utilities.TestConfig.Instance.TestUserPassword;
            _loginPage.Login(email, password);

            Thread.Sleep(2000);
        }

        public void NavigateToReservationsPage()
        {
            IWebElement reservationsLink = Driver.FindElement(By.XPath("/html/body/div/header/div/section/div[2]/a[3]"));
            reservationsLink.Click();
            Thread.Sleep(2000);

            _reservationPage = new ReservationPage(Driver);
        }

        [Test]
        [Category("Smoke")]
        public void ReservationPage_ShouldDisplayReservedTag()
        {
            // Arrange
            LoginUser();
            NavigateToReservationsPage();

            // Assert
            Assert.That(_reservationPage.IsReservedTagDisplayed(), Is.True, "Reserved tag should be displayed");
        }

        [Test]
        [Category("Smoke")]
        public void ReservationPage_NoReservations_ShouldNotDisplayMessage()
        {
            LoginUser();
            NavigateToReservationsPage();

            try
            {
                IWebElement noReservationsHeader = Driver.FindElement(By.XPath("/html/body/div/div/div/div/div/p[1]"));
                Assert.That(noReservationsHeader.Displayed, Is.False, "Сообщение 'No Reservations' не должно отображаться");
            }
            catch (NoSuchElementException)
            {
                Assert.Pass("Сообщение 'No Reservations' отсутствует, как и ожидалось");
            }
        }

        [Test]
        [Category("Navigation")]
        public void ReservationPage_NoReservations_ShouldNotHaveBookTableButton()
        {
            LoginUser();
            NavigateToReservationsPage();

            var bookTableButtons = Driver.FindElements(By.XPath("/html/body/div/div/div/div/a"));
            Assert.That(bookTableButtons.Count, Is.EqualTo(0), "Кнопка 'Book a Table' не должна отображаться при отсутствии доступных бронирований");
        }


        [Test]
        [Category("Smoke")]
        public void ReservationPage_ShouldDisplayActionButtons()
        {
            // Arrange
            LoginUser();
            NavigateToReservationsPage();

            // Assert
            Assert.That(_reservationPage.IsCancelButtonDisplayed(), Is.True, "Cancel button should be displayed");
            Assert.That(_reservationPage.IsEditButtonDisplayed(), Is.True, "Edit button should be displayed");
        }

        [Test]
        [Category("Smoke")]
        public void ReservationPage_ShouldClickCancelButton()
        {
            // Arrange
            LoginUser();
            NavigateToReservationsPage();

            // Act
                _reservationPage.ClickCancelButton();
        }

        [Test]
        [Category("Smoke")]
        public void ReservationPage_ShouldClickPreOrderButton()
        {
            // Arrange
            LoginUser();
            NavigateToReservationsPage();

            // Act
            _reservationPage.ClickPreOrderButton();
        }

        [Test]
        [Category("Smoke")]
        [Category("End-to-End")]
        public void CompleteBookingProcess_ShouldNavigateAndCompleteBooking()
        {
            // Arrange - Log in and navigate to the Reservations page
            LoginUser();
            NavigateToReservationsPage();

            // Step 1: Click the "Book a Table" button to go to the booking page
            IWebElement bookTableButton = Driver.FindElement(By.XPath("/html/body/div/div/div/div/a"));
            bookTableButton.Click();
            Thread.Sleep(2000);

            // Verify that we navigated to the booking page
            _bookingPage = new BookingPage(Driver);
            Assert.That(_bookingPage.IsBookingHeaderDisplayed(), Is.True, "The 'Book a Table' header should be displayed");

            // Select a date - click on the calendar icon to open the calendar
            IWebElement datePickerIcon = Driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[2]/div/form/button[2]/span"));
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", datePickerIcon);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", datePickerIcon);
            Thread.Sleep(1000);

            // Select the 30th day
            IWebElement date30 = Driver.FindElement(By.XPath("/html/body/div[2]/div/div/div/div/table/tbody/tr[2]/td[1]/button"));
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", date30);
            Thread.Sleep(1000);

            // Step 2: Click the "Find a Table" button
            IWebElement findTableButton = Driver.FindElement(By.XPath("/html/body/div/div[1]/div[2]/div/form/button[4]"));
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", findTableButton);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", findTableButton);
            Thread.Sleep(3000);

            // Verify that available tables are displayed
            try
            {
                IWebElement availableTables = Driver.FindElement(By.XPath("//ul/li/article"));
                Assert.That(availableTables.Displayed, Is.True, "Available tables should be displayed");

                // Step 3: Click the booking button for the second table
                IWebElement tableBookButton = Driver.FindElement(By.XPath("/html/body/div/div[2]/ul/li[2]/article/div[2]/div[2]/button"));
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", tableBookButton);
                Thread.Sleep(500);
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", tableBookButton);
                Thread.Sleep(2000);

                // Step 4: Click the "Make a Reservation" button
                IWebElement makeReservationButton = Driver.FindElement(By.XPath("/html/body/div[3]/form/button"));
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", makeReservationButton);
                Thread.Sleep(2000);

                // Verify that the booking confirmation is shown
                IWebElement confirmationElement = Driver.FindElement(By.XPath("/html/body/div[3]/div/h2"));
                Assert.That(confirmationElement.Displayed, Is.True, "The booking confirmation should be displayed");

                // Step 5: Click the close (X) button to close the confirmation modal
                IWebElement closeButton = Driver.FindElement(By.XPath("/html/body/div[3]/button"));
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", closeButton);
                Thread.Sleep(2000);

                // Verify that the modal is closed
                try
                {
                    Driver.FindElement(By.XPath("/html/body/div[3]/div/h2"));
                    Assert.Fail("The confirmation modal did not close");
                }
                catch (NoSuchElementException)
                {
                    // Expected behavior if the modal is closed
                    Assert.Pass("The modal closed successfully");
                }
            }
            catch (NoSuchElementException)
            {
                // Handle the case where no tables are available for the selected date
                IWebElement noTablesMessage = Driver.FindElement(By.XPath("//div[contains(text(), '0 tables available')]"));
                Assert.That(noTablesMessage.Displayed, Is.True, "A message indicating no available tables should be displayed");
                Assert.Inconclusive("No tables available for the selected date and time");
            }
        }


        [Test]
        [Category("Smoke")]
        [Category("End-to-End")]
        public void MenuPage_ShouldAddItemsAndSubmitPreOrder()
        {
            // Arrange - Log in and navigate to the Reservations page
            LoginUser();
            NavigateToReservationsPage();

            // Step 1: Click the "Pre-order" button to go to the menu page
            IWebElement preOrderButton = Driver.FindElement(By.XPath("//button[contains(text(), 'Pre-order')]"));
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", preOrderButton);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", preOrderButton);
            Thread.Sleep(2000);

            // Step 2: Click the first available "Pre-order" button (e.g., for Roasted Sweet Potato & Lentil Salad)
            IWebElement firstPreOrderItemButton = Driver.FindElement(By.XPath("/html/body/div/div/div[3]/div[2]/div/div[1]/button"));
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", firstPreOrderItemButton);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", firstPreOrderItemButton);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", firstPreOrderItemButton);
            Thread.Sleep(1000); // pause for loading/reaction

            // Step 4: Click the "Submit" button in the cart
            IWebElement submitButton = Driver.FindElement(By.XPath("/html/body/div[3]/div[2]/ul/li/button[2]"));
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitButton);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", submitButton);
            Thread.Sleep(2000);

            // Step 5: Click the close (X) button to close the cart
            IWebElement closeButton = Driver.FindElement(By.XPath("/html/body/div[3]/button"));
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", closeButton);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", closeButton);
            Thread.Sleep(2000);
        }


        [Test]
        [Category("Smoke")]
        public void ReservationPage_ShouldClickEditButton()
        {
            // Arrange
            LoginUser();
            NavigateToReservationsPage();

            // Act
            _reservationPage.ClickEditButton();
        }
    }
}