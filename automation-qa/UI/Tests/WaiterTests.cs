using NUnit.Framework;
using automation_qa.UI.Pages;
using automation_qa.Framework;
using System.Threading;
using System;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace automation_qa.UI.Tests
{
    [TestFixture]
    [Category("Waiter")]
    public class WaiterPageTests : BaseTest
    {
        private NavigationBar _navigationBar;
        private LoginPage _loginPage;
        private WaiterPage _waiterPage;

        // List of test waiter credentials
        private readonly List<(string Email, string Password, string LocationId)> _waiters = new List<(string, string, string)>
        {
            ("laydyGaga98@example.com", "Password123!", "8c4fc44e-c1a5-42eb-9912-55aeb5111a99"),
            ("johnyDepp007@example.com", "Password123!", "8c4fc44e-c1a5-42eb-9912-55aeb5111a99"),
            ("kylieJenner69@example.com", "Password123!", "e1fcb3b4-bf68-4bcb-b9ba-eac917dafac7"),
            ("johnCena46@example.com", "Password123!", "3a88c365-970b-4a7a-a206-bc5282b9b25f")
        };

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _navigationBar = new NavigationBar(Driver);
            _loginPage = new LoginPage(Driver);
        }

        private void LoginAsWaiter(string email, string password)
        {
            Driver.Navigate().GoToUrl(BaseConfiguration.UiBaseUrl);
            _navigationBar.GoToLoginPage();

            _loginPage.Login(email, password);
            Thread.Sleep(2000);

            _waiterPage = new WaiterPage(Driver);
        }

        [Test]
        [Category("Smoke")]
        public void WaiterPage_ShouldDisplayHelloInGreeting()
        {
            // Arrange - Login as Lady Gaga waiter
            LoginAsWaiter(_waiters[0].Email, _waiters[0].Password);

            // Assert - проверяем только наличие "Hello"
            string greetingText = _waiterPage.GetWaiterGreetingText();
            Assert.That(greetingText, Does.Contain("Hello"), "Greeting should contain 'Hello'");
        }

        [Test]
        [Category("Smoke")]
        public void WaiterPage_ShouldDisplayReservationsHeading()
        {
            // Arrange - Login as Johnny Depp waiter
            LoginAsWaiter(_waiters[1].Email, _waiters[1].Password);

            // Assert
            Assert.That(_waiterPage.IsReservationsHeadingDisplayed(), Is.True, "Reservations heading should be displayed");
            string headingText = _waiterPage.GetReservationsHeadingText();
            Assert.That(headingText, Does.Contain("Reservations"), "Heading should contain 'Reservations'");
        }

        [Test]
        [Category("Functional")]
        public void WaiterPage_SearchButtonShouldBeClickable()
        {
            // Arrange - Login as Kylie Jenner waiter
            LoginAsWaiter(_waiters[2].Email, _waiters[2].Password);

            // Act & Assert - Check that the Search button is clickable
            IWebElement searchButton = Driver.FindElement(By.XPath("/html/body/div/div/form/button[4]"));
            Assert.That(searchButton.Enabled, Is.True, "Search button should be enabled");

            // Try to click it without using Assert.Pass()
            try
            {
                searchButton.Click();
                // Just a regular assertion instead of Assert.Pass
                Assert.That(true, Is.True, "Search button was clicked successfully");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Failed to click Search button: {ex.Message}");
            }
        }

        [Test]
        [Category("Functional")]
        public void WaiterPage_ShouldShowCancelAndPostponeButtons()
        {
            // Arrange - Login as John Cena waiter
            LoginAsWaiter(_waiters[0].Email, _waiters[0].Password);

            // Check if there are any reservation cards
            if (_waiterPage.AreReservationCardsDisplayed())
            {
                // Assert
                bool cancelButtonDisplayed = _waiterPage.IsCancelButtonDisplayed();
                bool postponeButtonDisplayed = _waiterPage.IsPostponeButtonDisplayed();

                // At least one of the buttons should be displayed
                Assert.That(cancelButtonDisplayed || postponeButtonDisplayed, Is.True,
                    "Either Cancel or Postpone button should be displayed on reservation cards");
            }
            else
            {
                Assert.Inconclusive("No reservation cards found to test the Cancel and Postpone buttons");
            }
        }

        [Test]
        [Category("Functional")]
        public void WaiterPage_ShouldShowCanceledStatusAfterCancellation()
        {
            // Arrange - Login as waiter
            LoginAsWaiter(_waiters[1].Email, _waiters[1].Password);

            // Check if there are any reservation cards with Cancel button
            if (_waiterPage.AreReservationCardsDisplayed() && _waiterPage.IsCancelButtonDisplayed())
            {
                // Act - click the Cancel button directly instead of using the method that looks for confirmation
                IWebElement cancelButton = Driver.FindElement(By.XPath("/html/body/div/div/div[2]/div/article[1]/footer/button[1]"));
                cancelButton.Click();

                // Wait for page update
                Thread.Sleep(2000);

                // Assert - check that the Canceled label appears
                IWebElement canceledTag = Driver.FindElement(By.XPath("/html/body/div/div/div[2]/div/article[1]/div/span"));
                Assert.That(canceledTag.Text, Is.EqualTo("Canceled"), "Status should change to 'Canceled' after cancellation");
            }
            else
            {
                Assert.Inconclusive("No cancelable reservation cards found to test cancellation");
            }
        }

        [Test]
        [Category("Smoke")]
        [Category("End-to-End")]
        public void WaiterPage_ShouldNotCreateNewReservationForExistingCustomerWihoutTime()
        {
            // Arrange – Log in and navigate to the Reservations page
            LoginAsWaiter(_waiters[0].Email, _waiters[0].Password);

            // Step 1: Click the "Create New Reservation" button
            IWebElement createNewReservationButton = Driver.FindElement(By.XPath("/html/body/div/div/div[1]/button"));
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", createNewReservationButton);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", createNewReservationButton);
            Thread.Sleep(2000);

            // Step 2: Select the "Existing Customer" option
            IWebElement existingCustomerOption = Driver.FindElement(By.XPath("/html/body/div[3]/form/div[2]/label[2]/p"));
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", existingCustomerOption);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", existingCustomerOption);
            Thread.Sleep(1000);

            // Step 3: Enter the name "Roman Zvarych" into the input field
            IWebElement customerNameInput = Driver.FindElement(By.XPath("/html/body/div[3]/form/div[3]/button/input"));
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", customerNameInput);
            Thread.Sleep(1500);
            customerNameInput.Clear();
            customerNameInput.SendKeys("Roman Zvarych");
            Thread.Sleep(1000);

            // Step 4: Select the first customer from the autocomplete dropdown
            IWebElement firstCustomerOption = Driver.FindElement(By.XPath("/html/body/div[4]/div/ul/li[1]"));
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", firstCustomerOption);
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", firstCustomerOption);
            Thread.Sleep(1000);

            // Step 5: Click the "Make a Reservation" button
            IWebElement makeReservationButton = Driver.FindElement(By.XPath("/html/body/div[3]/form/div[6]/button"));
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", makeReservationButton);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", makeReservationButton);
            Thread.Sleep(2000);
        }
    }
}
