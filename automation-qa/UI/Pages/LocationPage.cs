using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace automation_qa.UI.Pages
{
    public class LocationOverviewPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        private readonly By _locationTitle = By.XPath("/html/body/div/div[1]/div/a[2]/p");
        private readonly By _reviewStars = By.XPath("/html/body/div/div[4]/div/div[2]/div[1]/div[1]/div[3]");
        private readonly By _specialityDishesSection = By.XPath("/html/body/div/div[3]/article/header/p");
        private readonly By _dishCards = By.XPath("//article[1]/div/div");
        private readonly By _feedbackSection = By.XPath("/html/body/div/div[4]/div/p");
        private readonly By _cuisineExperienceFilter = By.XPath("/html/body/div/div[4]/div/div[1]/div[1]/button[2]/p");
        private readonly By _mainPageLink = By.XPath("/html/body/div/div[1]/div/a[1]");

        public LocationOverviewPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
        }

        public bool IsLocationTitleDisplayed()
        {
            try
            {
                var title = _wait.Until(ExpectedConditions.ElementIsVisible(_locationTitle));
                return title.Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public bool AreReviewStarsDisplayed()
        {
            try
            {
                var stars = _wait.Until(ExpectedConditions.ElementIsVisible(_reviewStars));
                return stars.Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public bool IsSpecialityDishesSectionDisplayed()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(_specialityDishesSection)).Displayed;
        }

        public bool AreDishCardsDisplayed()
        {
            try
            {
                var cards = _driver.FindElements(_dishCards);
                if (cards.Count > 0)
                {
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", cards[0]);
                }
                return cards.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        public int GetDishCardsCount()
        {
            try
            {
                return _driver.FindElements(_dishCards).Count;
            }
            catch
            {
                return 0;
            }
        }

        public bool IsFeedbackSectionDisplayed()
        {
            try
            {
                var section = _wait.Until(ExpectedConditions.ElementIsVisible(_feedbackSection));
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", section);
                return section.Displayed;
            }
            catch
            {
                return false;
            }
        }

        public void ClickCuisineExperienceFilter()
        {
            var filter = _wait.Until(ExpectedConditions.ElementToBeClickable(_cuisineExperienceFilter));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", filter);
            filter.Click();
        }

        public bool IsCuisineExperienceFilterClickable()
        {
            try
            {
                var filter = _wait.Until(ExpectedConditions.ElementToBeClickable(_cuisineExperienceFilter));
                return filter != null && filter.Displayed && filter.Enabled;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public MainPage ClickMainPageLink()
        {
            var mainPageLink = _wait.Until(ExpectedConditions.ElementToBeClickable(_mainPageLink));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", mainPageLink);
            mainPageLink.Click();
            return new MainPage(_driver);
        }

        public bool IsMainPageLinkClickable()
        {
            try
            {
                var mainPageLink = _wait.Until(ExpectedConditions.ElementToBeClickable(_mainPageLink));
                var linkText = mainPageLink.FindElement(By.XPath("./p"));
                Console.WriteLine($"Main page link text: {linkText.Text}");
                return mainPageLink != null && mainPageLink.Displayed && mainPageLink.Enabled && linkText.Text.Contains("Main page", StringComparison.OrdinalIgnoreCase);
            }
            catch (WebDriverTimeoutException ex)
            {
                Console.WriteLine($"Failed to find Main page link: {ex.Message}");
                return false;
            }
        }
    }
}