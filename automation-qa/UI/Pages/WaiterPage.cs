using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace automation_qa.UI.Pages
{
    public class WaiterPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Page identifiers
        private readonly By _waiterGreeting = By.XPath("/html/body/div/section[2]/div/p");
        private readonly By _reservationsHeading = By.XPath("/html/body/div/header/div/section/div[2]/a[1]");

        // Filter elements
        private readonly By _dateFilter = By.XPath("//button[contains(@class, 'inline-flex') and descendant::span[contains(text(), 'Apr')]]");
        private readonly By _timeFilter = By.XPath("//button[contains(@class, 'inline-flex') and contains(text(), 'Time')]");
        private readonly By _tableFilter = By.XPath("//button[contains(@class, 'inline-flex') and contains(text(), 'Any table')]");
        private readonly By _searchButton = By.XPath("/html/body/div/div/form/button[4]/svg");

        // Reservation card elements
        private readonly By _reservationCard = By.XPath("/html/body/div/div/div[2]/div/article[1]");
        private readonly By _canceledTag = By.XPath("//span[contains(text(), 'Canceled')]");
        private readonly By _cancelButton = By.XPath("/html/body/div/div/div[2]/div/article[1]/footer/button[1]");
        private readonly By _postponeButton = By.XPath("/html/body/div/div/div[2]/div/article[1]/footer/button[2]");

        // Create new reservation button
        private readonly By _createNewReservationButton = By.XPath("//button[contains(text(), 'Create New Reservation')]");

        public WaiterPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// Checks if waiter greeting is displayed
        /// </summary>
        public bool IsWaiterGreetingDisplayed()
        {
            try
            {
                return _driver.FindElement(_waiterGreeting).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the waiter greeting text
        /// </summary>
        public string GetWaiterGreetingText()
        {
            try
            {
                return _driver.FindElement(_waiterGreeting).Text;
            }
            catch (NoSuchElementException)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Checks if reservations heading is displayed
        /// </summary>
        public bool IsReservationsHeadingDisplayed()
        {
            try
            {
                return _driver.FindElement(_reservationsHeading).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the reservations heading text
        /// </summary>
        public string GetReservationsHeadingText()
        {
            try
            {
                return _driver.FindElement(_reservationsHeading).Text;
            }
            catch (NoSuchElementException)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Selects date from the date filter
        /// </summary>
        public void SelectDate()
        {
            _driver.FindElement(_dateFilter).Click();
            Thread.Sleep(500);

            // Select a date from the dropdown
            IWebElement dateOption = _driver.FindElement(By.XPath("//div[@role='listbox']//span[1]"));
            dateOption.Click();
            Thread.Sleep(500);
        }

        /// <summary>
        /// Selects time from the time filter
        /// </summary>
        public void SelectTime()
        {
            _driver.FindElement(_timeFilter).Click();
            Thread.Sleep(500);

            // Select a time from the dropdown
            IWebElement timeOption = _driver.FindElement(By.XPath("//div[@role='listbox']//span[1]"));
            timeOption.Click();
            Thread.Sleep(500);
        }

        /// <summary>
        /// Selects table from the table filter
        /// </summary>
        public void SelectTable()
        {
            _driver.FindElement(_tableFilter).Click();
            Thread.Sleep(500);

            // Select a table from the dropdown
            IWebElement tableOption = _driver.FindElement(By.XPath("//div[@role='listbox']//span[1]"));
            tableOption.Click();
            Thread.Sleep(500);
        }

        /// <summary>
        /// Clicks the search button
        /// </summary>
        public void ClickSearch()
        {
            _driver.FindElement(_searchButton).Click();
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Checks if any reservation cards are displayed
        /// </summary>
        public bool AreReservationCardsDisplayed()
        {
            try
            {
                return _driver.FindElements(_reservationCard).Count > 0;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the number of reservation cards
        /// </summary>
        public int GetReservationCardsCount()
        {
            try
            {
                return _driver.FindElements(_reservationCard).Count;
            }
            catch (NoSuchElementException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Checks if the Cancel button is displayed on any reservation card
        /// </summary>
        public bool IsCancelButtonDisplayed()
        {
            try
            {
                return _driver.FindElement(_cancelButton).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the Postpone button is displayed on any reservation card
        /// </summary>
        public bool IsPostponeButtonDisplayed()
        {
            try
            {
                return _driver.FindElement(_postponeButton).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Clicks the Cancel button on the first reservation card
        /// </summary>
        public void ClickCancelButton()
        {
            _driver.FindElement(_cancelButton).Click();
            Thread.Sleep(1000);

            // Confirm cancellation in the dialog
            IWebElement confirmButton = _driver.FindElement(By.XPath("//button[contains(text(), 'Confirm')]"));
            confirmButton.Click();
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Clicks the Postpone button on the first reservation card
        /// </summary>
        public void ClickPostponeButton()
        {
            _driver.FindElement(_postponeButton).Click();
            Thread.Sleep(1000);

            // Select new date and time in the dialog
            IWebElement confirmButton = _driver.FindElement(By.XPath("//button[contains(text(), 'Confirm')]"));
            confirmButton.Click();
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Checks if the Canceled tag is displayed on any reservation card
        /// </summary>
        public bool IsCanceledTagDisplayed()
        {
            try
            {
                return _driver.FindElement(_canceledTag).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Clicks the Create New Reservation button
        /// </summary>
        public void ClickCreateNewReservation()
        {
            _driver.FindElement(_createNewReservationButton).Click();
            Thread.Sleep(1000);
        }
    }
}