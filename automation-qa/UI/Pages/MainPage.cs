using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace automation_qa.UI.Pages
{
    public class MainPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        private readonly By _headerTitle = By.XPath("//p[@class='fontset-h1 text-green-200']//span[@class='text-primary' and text()='Green']/following-sibling::span[text()=' & Tasty']");
        private readonly By _viewMenuButton = By.XPath("/html/body/div/div[1]/div/div/a");
        private readonly By _locationSection = By.XPath("//div[contains(@class, 'w-full') and contains(@class, 'mx-auto')]/article[2]/header/p");
        private readonly By _locationCard = By.XPath("//article[2]//div[contains(@class, 'p-6')]//div[contains(@class, 'flex') and contains(@class, 'gap-2.5')]//p");
        private readonly By _locationCards = By.CssSelector("#root > div.w-full.mx-auto.max-w-\\[1440px\\].px-\\[40px\\].pt-\\[4rem\\].pb-\\[2\\.5rem\\] > article:nth-child(2) > div > div");
        private readonly By _bookTableLink = By.XPath("//header//section//div[contains(@class, 'flex')]/a[2]");
        private readonly By _reservationsLink = By.XPath("//article[2]/div/div[2]/div[contains(@class, 'w-full') and contains(@class, 'h-140px')]");
        private readonly By _reservationsNavLink = By.XPath("//a[text()='Reservations']");
        private readonly By _dishCards = By.XPath("//*[@id='root']/div[2]/article[1]/div/div");
        private readonly By _menuTitle = By.XPath("/html/body/div/div/div[1]/div/h1");

        public MainPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        public bool IsHeaderDisplayed()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(_headerTitle)).Displayed;
        }

        public bool IsViewMenuButtonDisplayed()
        {
            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                return wait.Until(
                    SeleniumExtras.WaitHelpers.ExpectedConditions
                        .ElementIsVisible(By.CssSelector("#root a > button"))
                ).Displayed;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return false;
            }
        }

        public bool IsViewMenuButtonClickable()
        {
            try
            {
                var button = _wait.Until(ExpectedConditions.ElementToBeClickable(_viewMenuButton));
                return button != null && button.Displayed && button.Enabled;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public MenuPage NavigateToMenuPage()
        {
            var viewMenuButton = _wait.Until(ExpectedConditions.ElementToBeClickable(_viewMenuButton));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", viewMenuButton);
            viewMenuButton.Click();
            System.Threading.Thread.Sleep(5000);

            if (!_driver.Url.Contains("/menu"))
            {
                throw new Exception($"Failed to navigate to MenuPage. Current URL: {_driver.Url}");
            }
            return new MenuPage(_driver);
        }

        public bool IsLocationSectionDisplayed()
        {
            var element = _driver.FindElement(_locationSection);

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);

            return _wait.Until(ExpectedConditions.ElementIsVisible(_locationSection)).Displayed;
        }

        public bool IsBookTableLinkDisplayed()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(_bookTableLink)).Displayed;
        }

        public bool IsReservationsLinkDisplayed()
        {
            try
            {
                return _wait.Until(ExpectedConditions.ElementIsVisible(_bookTableLink)).Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public BookingPage ClickBookTableLink()
        {
            var bookTableLink = _wait.Until(ExpectedConditions.ElementToBeClickable(_bookTableLink));
            bookTableLink.Click();
            return new BookingPage(_driver);
        }

        public LocationOverviewPage SelectLocation()
        {
            var locationCard = _wait.Until(ExpectedConditions.ElementToBeClickable(_locationCard));

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", locationCard);

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", locationCard);

            return new LocationOverviewPage(_driver);
        }

        public bool AreDishCardsDisplayed()
        {
            try
            {
                var cards = _driver.FindElements(_dishCards);
                // Прокручиваем до первой карточки, если она есть
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

        // Метод для получения количества плашек
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

        public bool AreLocationCardsDisplayed()
        {
            try
            {
                // Сначала прокручиваем до раздела с локациями
                var locationSection = _driver.FindElement(_locationSection);
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", locationSection);

                // Даем время для загрузки элементов после прокрутки
                Thread.Sleep(1000);

                // Затем ищем карточки локаций
                var cards = _driver.FindElements(_locationCards);

                Console.WriteLine($"Found {cards.Count} location cards");

                return cards.Count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking location cards: {ex.Message}");
                return false;
            }
        }

        // Добавьте этот метод в класс MainPage
        public int GetLocationCardsCount()
        {
            try
            {
                return _driver.FindElements(_locationCards).Count;
            }
            catch
            {
                return 0;
            }
        }

        public bool IsReservationsNavLinkDisplayed()
        {
            try
            {
                return _wait.Until(ExpectedConditions.ElementIsVisible(_reservationsNavLink)).Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public void ClickViewMenuButton()
        {
            // Используем абсолютный XPath для кнопки View Menu
            By viewMenuButtonSelector = By.XPath("/html/body/div/div[1]/div/div/a");

            var button = _wait.Until(ExpectedConditions.ElementToBeClickable(viewMenuButtonSelector));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", button);

            // Не будем ждать конкретный URL, просто даем время для загрузки страницы
            Thread.Sleep(2000);
        }
    }
}
