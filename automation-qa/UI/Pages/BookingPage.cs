using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace automation_qa.UI.Pages
{
    public class BookingPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        private readonly By _bookingHeader = By.XPath("//div[contains(@class, 'content-center')]//h1[contains(text(), 'Book a Table')]");
        private readonly By _locationDropdownButton = By.CssSelector("button:nth-child(1)");
        private readonly By _locationDropdown = By.XPath("//button[contains(@class, 'inline-flex') and descendant::span[contains(text(), 'Location')]]");
        private readonly By _locationDropdownOpen = By.XPath("//button[contains(@class, 'inline-flex')]//svg[contains(@class, 'stroke-foreground')]");
        private readonly By _locationOption = By.XPath("//span[@data-slot='select-value' and contains(text(), '14 Baratashvili Street')]");
        private readonly By _dateDropdown = By.XPath("//button[contains(@class, 'inline-flex') and descendant::span[contains(text(), 'Date')]]");
        private readonly By _dateDropdownOpen = By.CssSelector("#root > div.content-center.min-h-\\[404px\\].bg-cover.bg-no-repeat.bg-center.py-\\[98px\\].bg-\\[var\\(--color-neutral-900\\)\\]\\/80.bg-blend-overlay.flex.flex-col.justify-center > div > form > button:nth-child(3) > svg:nth-child(3)");
        private readonly By _dateOption = By.XPath("//table[@role='grid']//button[text()='6']");
        private readonly By _timeDropdown = By.XPath("//button[contains(@class, 'inline-flex') and descendant::span[contains(text(), 'Time')]]");
        private readonly By _timeDropdownOpen = By.CssSelector("#root > div.content-center.min-h-\\[404px\\].bg-cover.bg-no-repeat.bg-center.py-\\[98px\\].bg-\\[var\\(--color-neutral-900\\)\\]\\/80.bg-blend-overlay.flex.flex-col.justify-center > div > form > button:nth-child(3) > svg:nth-child(3)");
        private readonly By _timeOption = By.XPath("//div[@role='listbox']//span[1]");
        private readonly By _guestsCounter = By.XPath("//div[contains(@class, 'flex') and contains(@class, 'items-center')]");
        private readonly By _guestsIncreaseButton = By.CssSelector("#root > div.content-center.min-h-\\[404px\\].bg-cover.bg-no-repeat.bg-center.py-\\[98px\\].bg-\\[var\\(--color-neutral-900\\)\\]\\/80.bg-blend-overlay.flex.flex-col.justify-center > div > form > div > div > button:nth-child(3)");
        private readonly By _guestsDecreaseButton = By.XPath("//div[contains(@class, 'flex')]//button[1]//svg");
        private readonly By _guestsCount = By.XPath("//div[contains(@class, 'flex')]//span[contains(@class, 'text-center')]");
        private readonly By _findTableButton = By.XPath("//button[contains(@class, 'inline-flex') and contains(text(), 'Find a Table')]");
        private readonly By _availableTablesCounter = By.XPath("//span[contains(text(), 'tables available')]");
        private readonly By _tableCard = By.XPath("/html/body/div/div[2]/ul/li");
        private readonly By _tableCapacity = By.XPath("//p[contains(text(), 'Table seating capacity:')]");
        private readonly By _tableLocation = By.XPath("//span[contains(text(), 'Abashidze Street')]");
        private readonly By _tableTimeSlot = By.XPath("//button[contains(@class, 'inline-flex') and contains(@class, 'rounded')]//span[contains(text(), '1:30 p.m. - 3:00 p.m.')]");

        public BookingPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        public bool IsBookingHeaderDisplayed()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(_bookingHeader)).Displayed;
        }

        public void OpenLocationDropdown()
        {
            var element = _wait.Until(ExpectedConditions.ElementToBeClickable(_locationDropdownButton));

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", element);
        }

        public void SelectLocation()
        {
            var locationOption = _wait.Until(ExpectedConditions.ElementExists(_locationOption));

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", locationOption);

            System.Threading.Thread.Sleep(500);

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", locationOption);
        }

        public void ClickTimeDropdownButton()
        {
            var element = _wait.Until(ExpectedConditions.ElementExists(_timeDropdownOpen));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", element);
        }

        public void OpenDateDropdown()
        {
            var dateDropdown = _wait.Until(ExpectedConditions.ElementToBeClickable(_dateDropdownOpen));
            dateDropdown.Click();
        }

        public void SelectDate()
        {
            var dateOption = _wait.Until(ExpectedConditions.ElementToBeClickable(_dateOption));
            dateOption.Click();
        }

        public void OpenTimeDropdown()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(_timeDropdownOpen)).Click();
        }

        public void SelectTime()
        {
            var timeOption = _driver.FindElements(By.XPath("//div[@role='listbox']//span")).FirstOrDefault();
            if (timeOption != null)
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", timeOption);
            }
        }

        public void IncreaseGuests()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(_guestsIncreaseButton)).Click();
        }

        public void DecreaseGuests()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(_guestsDecreaseButton)).Click();
        }

        public string GetGuestsCount()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(_guestsCount)).Text;
        }

        public void ClickFindTable()
        {
            var findTableButton = _wait.Until(ExpectedConditions.ElementToBeClickable(_findTableButton));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", findTableButton);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", findTableButton);
        }

        public bool AreAvailableTablesDisplayed()
        {
            try
            {
                return _wait.Until(ExpectedConditions.ElementIsVisible(_tableCard)).Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public string GetAvailableTablesCount()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(_availableTablesCounter)).Text;
        }

        public bool IsTableCardDisplayed()
        {
            try
            {
                return _wait.Until(ExpectedConditions.ElementIsVisible(_tableCard)).Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public bool IsTableCapacityDisplayed()
        {
            try
            {
                return _wait.Until(ExpectedConditions.ElementIsVisible(_tableCapacity)).Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public bool IsTableLocationDisplayed()
        {
            try
            {
                return _wait.Until(ExpectedConditions.ElementIsVisible(_tableLocation)).Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public bool IsTableTimeSlotDisplayed()
        {
            try
            {
                return _wait.Until(ExpectedConditions.ElementIsVisible(_tableTimeSlot)).Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public bool IsFindTableButtonClickable()
        {
            try
            {
                var findTableButton = _wait.Until(ExpectedConditions.ElementToBeClickable(_findTableButton));
                return findTableButton != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void CompleteTableSearch()
        {
            OpenLocationDropdown();
            SelectLocation();
            OpenDateDropdown();
            SelectDate();
            OpenTimeDropdown();
            SelectTime();
            IncreaseGuests();
            IncreaseGuests(); 
            ClickFindTable();
        }
    }
}