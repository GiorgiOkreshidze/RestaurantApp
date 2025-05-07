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
        private readonly By _dateDropdownOpen = By.CssSelector("form button:nth-of-type(2) svg");
        private readonly By _dateOption = By.XPath("/html/body/div[2]/div/div/div/div/table/tbody/tr[3]/td[6]/button");
        private readonly By _timeDropdown = By.XPath("//button[contains(@class, 'inline-flex') and descendant::span[contains(text(), 'Time')]]");
        private readonly By _timeDropdownOpen = By.CssSelector("#root > div.content-center.min-h-\\[404px\\].bg-cover.bg-no-repeat.bg-center.py-\\[98px\\].bg-\\[var\\(--color-neutral-900\\)\\]\\/80.bg-blend-overlay.flex.flex-col.justify-center > div > form > button:nth-child(3) > svg:nth-child(3)");
        private readonly By _timeOption = By.XPath("//div[@role='listbox']//span[1]");
        private readonly By _guestsCounter = By.XPath("//div[contains(@class, 'flex') and contains(@class, 'items-center')]");
        private readonly By _guestsIncreaseButton = By.XPath("/html/body/div/div[1]/div[2]/div/form/div/div/button[2]");
        private readonly By _guestsDecreaseButton = By.XPath("//div[contains(@class, 'flex')]//button[1]//svg");
        private readonly By _guestsCount = By.XPath("//div[contains(@class, 'flex')]//span[contains(@class, 'text-center')]");
        private readonly By _findTableButton = By.XPath("//button[contains(@class, 'inline-flex') and contains(text(), 'Find a Table')]");
        private readonly By _availableTablesCounter = By.XPath("//span[contains(text(), 'tables available')]");
        private readonly By _tableCard = By.XPath("/html/body/div/div[2]/ul/li");
        private readonly By _tableCapacity = By.XPath("//p[contains(text(), 'Table seating capacity:')]");
        private readonly By _tableLocation = By.XPath("//span[contains(text(), 'Abashidze Street')]");
        private readonly By _tableTimeSlot = By.XPath("//button[contains(@class, 'inline-flex') and contains(@class, 'rounded')]//span[contains(text(), '1:30 p.m. - 3:00 p.m.')]");

        private readonly By _table1Card = By.XPath("//*[@id='root']/div[2]/ul/li[1]/article");
        private readonly By _table2Card = By.XPath("//*[@id='root']/div[2]/ul/li[2]/article");
        private readonly By _table3Card = By.XPath("//*[@id='root']/div[2]/ul/li[3]/article");
        private readonly By _table4Card = By.XPath("//*[@id='root']/div[2]/ul/li[4]/article");

        private readonly By _tableTimeSlotButton = By.XPath("//button[contains(text(), '3:15 p.m. - 4:45 p.m.')]");

        private readonly By _reservationModal = By.XPath("//div[contains(@role, 'dialog')]");
        private readonly By _makeReservationButton = By.XPath("//button[contains(text(), 'Make a Reservation')]");

        private readonly By _reservationConfirmedHeader = By.XPath("//h2[contains(text(), 'Reservation Confirmed')]");
        private readonly By _reservationSuccessMessage = By.XPath("//div[contains(text(), 'has been successfully made')]");
        private readonly By _editReservationButton = By.XPath("//button[contains(text(), 'Edit Reservation')]");
        private readonly By _cancelReservationButton = By.XPath("//button[contains(text(), 'Cancel Reservation')]");
        private readonly By _successToast = By.XPath("//div[contains(@class, 'Toastify__toast-container--top-right')]//div[contains(@class, 'Toastify__toast--success')]");
        private readonly By _reservationCanceledToast = By.XPath("//div[contains(@class, 'Toastify__toast-container')]//div[contains(text(), 'Reservation canceled successfully')]");
        private readonly By _toastNotification = By.XPath("//div[contains(@class, 'Toastify__toast-container')]//div[contains(@class, 'Toastify__toast-body')]");
        private readonly By _date14Option = By.XPath("/html/body/div[2]/div/div/div/div/table/tbody/tr[1]/td[6]/button");


        private readonly By _increaseGuestsInModalButton = By.XPath("/html/body/div[3]/form/div[1]/div/button[2]");
        private readonly By _confirmEditButton = By.XPath("/html/body/div[3]/form/button");
        private readonly By _requiredFieldsWarning = By.XPath("//div[contains(@class, 'Toastify__toast--warning')]");
        private readonly By _date13Option = By.XPath("/html/body/div[2]/div/div/div/div/table/tbody/tr[1]/td[6]/button");

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

        public void SetGuestsCount(int count)
        {
            string currentCountText = GetGuestsCount();
            int currentCount = int.Parse(currentCountText);

            if (currentCount < count)
            {
                for (int i = currentCount; i < count; i++)
                {
                    IncreaseGuests();
                    Thread.Sleep(200);
                }
            }
            else if (currentCount > count)
            {
                for (int i = currentCount; i > count; i--)
                {
                    DecreaseGuests();
                    Thread.Sleep(200);
                }
            }
        }

        public void SelectTable(int index)
        {
            By bookButtonSelector = By.XPath($"/html/body/div/div[2]/ul/li[{index}]/article/div[2]/div[2]/button");

            try
            {
                IWebElement bookButton = _wait.Until(ExpectedConditions.ElementToBeClickable(bookButtonSelector));

                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", bookButton);

                Thread.Sleep(500);

                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", bookButton);
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine($"Не удалось найти кнопку бронирования для стола {index}");

                By anyButtonSelector = By.XPath($"//li[{index}]//button");
                IWebElement anyButton = _driver.FindElement(anyButtonSelector);

                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", anyButton);
            }
        }

        public void ConfirmReservation()
        {
            IWebElement makeReservationButton = _wait.Until(ExpectedConditions.ElementToBeClickable(_makeReservationButton));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", makeReservationButton);
        }

        public bool IsReservationConfirmed()
        {
            try
            {
                By confirmationHeaderSelector = By.XPath("//h2[contains(text(), 'Reservation Confirmed!')]");

                IWebElement confirmationHeader = _wait.Until(ExpectedConditions.ElementIsVisible(confirmationHeaderSelector));

                return confirmationHeader.Text.Contains("Reservation Confirmed!");
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public string GetConfirmationText()
        {
            try
            {
                By confirmationModalSelector = By.XPath("/html/body/div[3]/div");

                IWebElement confirmationModal = _wait.Until(ExpectedConditions.ElementIsVisible(confirmationModalSelector));

                return confirmationModal.Text;
            }
            catch (WebDriverTimeoutException)
            {
                return string.Empty;
            }
        }

        public bool CanEditReservation()
        {
            try
            {
                return _wait.Until(ExpectedConditions.ElementToBeClickable(_editReservationButton)).Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public bool CanCancelReservation()
        {
            try
            {
                return _wait.Until(ExpectedConditions.ElementToBeClickable(_cancelReservationButton)).Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public void CompleteFullBookingProcess()
        {
            ClickFindTable();
            Thread.Sleep(2000);

            SelectTable(1);
            Thread.Sleep(1000);

            ConfirmReservation();
        }

        public void CancelReservation()
        {
            By cancelButtonSelector = By.XPath("/html/body/div[3]/footer/button[1]");

            IWebElement cancelButton = _wait.Until(ExpectedConditions.ElementToBeClickable(cancelButtonSelector));

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", cancelButton);
        }

        public bool IsReservationCancelationConfirmed()
        {
            try
            {
                IWebElement successToast = _wait.Until(ExpectedConditions.ElementIsVisible(_successToast));

                Console.WriteLine($"Найдено Toast-уведомление: '{successToast.Text}'");

                return successToast.Displayed;
            }
            catch (WebDriverTimeoutException ex)
            {
                Console.WriteLine($"Не удалось найти Toast-уведомление: {ex.Message}");
                return false;
            }
        }

        public void SelectDate13()
        {
            OpenDateDropdown();

            IWebElement date13Button = _wait.Until(ExpectedConditions.ElementToBeClickable(_date13Option));

            // Кликаем на 13-е число
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", date13Button);
        }

        public void SelectDate14()
        {
            OpenDateDropdown();

            IWebElement date14Button = _wait.Until(ExpectedConditions.ElementToBeClickable(_date14Option));

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", date14Button);
        }

        public void EditReservation()
        {
            By editButtonSelector = By.XPath("//button[contains(text(), 'Edit Reservation')]");

            IWebElement editButton = _wait.Until(ExpectedConditions.ElementToBeClickable(editButtonSelector));

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", editButton);
        }

        public void IncreaseGuestsInModal()
        {
            IWebElement increaseButton = _wait.Until(ExpectedConditions.ElementToBeClickable(_increaseGuestsInModalButton));

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", increaseButton);
        }

        public void ConfirmEdit()
        {
            IWebElement confirmButton = _wait.Until(ExpectedConditions.ElementToBeClickable(_confirmEditButton));

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", confirmButton);
        }

        public bool IsRequiredFieldsWarningDisplayed()
        {
            try
            {
                IWebElement warningElement = _wait.Until(ExpectedConditions.ElementIsVisible(_requiredFieldsWarning));
                return warningElement.Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public string GetRequiredFieldsWarningText()
        {
            try
            {
                IWebElement warningElement = _wait.Until(ExpectedConditions.ElementIsVisible(_requiredFieldsWarning));
                return warningElement.Text;
            }
            catch (WebDriverTimeoutException)
            {
                return string.Empty;
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