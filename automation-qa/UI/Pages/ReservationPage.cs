using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace automation_qa.UI.Pages
{
    public class ReservationPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Basic page elements from the screenshot
        private readonly By _customerGreeting = By.XPath("//div[contains(text(), 'Hello, Irina Khrol')]");
        private readonly By _reservationCard = By.XPath("//div[contains(@class, 'p-4') and contains(@class, 'rounded-lg')]");
        private readonly By _locationText = By.XPath("//div[contains(text(), 'Baratashvili Street')]");
        private readonly By _dateText = By.XPath("//div[contains(text(), 'Apr 29, 2025')]");
        private readonly By _timeText = By.XPath("//div[contains(text(), '1:30 p.m. - 3:00 p.m.')]");
        private readonly By _guestsText = By.XPath("//div[contains(text(), '2 Guests')]");
        // Updated Reserved tag locator with specific XPath
        private readonly By _reservedTag = By.XPath("/html/body/div/header/div/section/div[2]/a[3]");

        // Action buttons
        private readonly By _cancelButton = By.XPath("/html/body/div/div/div/article[1]/footer/button[1]");
        private readonly By _editButton = By.XPath("/html/body/div/div/div/article[1]/footer/button[2]");
        private readonly By _preOrderButton = By.XPath("/html/body/div/div/div/article[1]/footer/button[3]");

        public ReservationPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        public bool IsCustomerGreetingDisplayed()
        {
            try
            {
                return _driver.FindElement(_customerGreeting).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsReservationCardDisplayed()
        {
            try
            {
                return _driver.FindElement(_reservationCard).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsLocationTextDisplayed()
        {
            try
            {
                return _driver.FindElement(_locationText).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsDateTextDisplayed()
        {
            try
            {
                return _driver.FindElement(_dateText).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsTimeTextDisplayed()
        {
            try
            {
                return _driver.FindElement(_timeText).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsGuestsTextDisplayed()
        {
            try
            {
                return _driver.FindElement(_guestsText).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsReservedTagDisplayed()
        {
            try
            {
                return _driver.FindElement(_reservedTag).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

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

        public bool IsEditButtonDisplayed()
        {
            try
            {
                return _driver.FindElement(_editButton).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsPreOrderButtonDisplayed()
        {
            try
            {
                return _driver.FindElement(_preOrderButton).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void ClickCancelButton()
        {
            _driver.FindElement(_cancelButton).Click();
        }

        public void ClickEditButton()
        {
            _driver.FindElement(_editButton).Click();
        }

        public void ClickPreOrderButton()
        {
            _driver.FindElement(_preOrderButton).Click();
        }
    }
}
