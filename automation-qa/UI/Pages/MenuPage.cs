using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace automation_qa.UI.Pages
{
    public class MenuPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        private readonly By _menuTitle = By.XPath("/html/body/div/div/div[1]/div/h1");
        private readonly By _appetizersFilter = By.XPath("//button[contains(text(), 'Appetizers')]");
        private readonly By _mainCoursesFilter = By.XPath("//button[contains(text(), 'Main Courses')]");
        private readonly By _dessertsFilter = By.XPath("//button[contains(text(), 'Desserts')]");
        private readonly By _specificButton = By.XPath("/html/body/div/div/div[2]/div[1]/div[1]/button[1]");

        public MenuPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
        }

        public bool IsMenuPageDisplayed()
        {
            try
            {
                var title = _wait.Until(ExpectedConditions.ElementIsVisible(_menuTitle));
                return title.Displayed && title.Text.Contains("Menu");
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public bool IsMenuPageUrl()
        {
            try
            {
                return _wait.Until(driver => driver.Url.Contains("menu"));
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public bool AreFiltersDisplayed()
        {
            try
            {
                var appetizers = _wait.Until(ExpectedConditions.ElementIsVisible(_appetizersFilter));
                var mainCourses = _wait.Until(ExpectedConditions.ElementIsVisible(_mainCoursesFilter));
                var desserts = _wait.Until(ExpectedConditions.ElementIsVisible(_dessertsFilter));
                return appetizers.Displayed && mainCourses.Displayed && desserts.Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public bool IsSpecificButtonClickable()
        {
            try
            {
                var button = _wait.Until(ExpectedConditions.ElementToBeClickable(_specificButton));
                return button != null && button.Displayed && button.Enabled;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public void ClickSpecificButton()
        {
            var button = _wait.Until(ExpectedConditions.ElementToBeClickable(_appetizersFilter));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", button);
            button.Click();
        }

        public bool IsMainCoursesButtonClickable()
        {
            try
            {
                var button = _wait.Until(ExpectedConditions.ElementToBeClickable(_mainCoursesFilter));
                return button != null && button.Displayed && button.Enabled;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public bool IsDessertsButtonClickable()
        {
            try
            {
                var button = _wait.Until(ExpectedConditions.ElementToBeClickable(_dessertsFilter));
                return button != null && button.Displayed && button.Enabled;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public void ClickMainCoursesButton()
        {
            var button = _wait.Until(ExpectedConditions.ElementToBeClickable(_mainCoursesFilter));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", button);
            button.Click();
        }

        public void ClickDessertsButton()
        {
            var button = _wait.Until(ExpectedConditions.ElementToBeClickable(_dessertsFilter));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", button);
            button.Click();
        }
    }
}