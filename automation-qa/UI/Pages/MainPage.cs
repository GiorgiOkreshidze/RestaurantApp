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

        // Локаторы для элементов на главной странице
        private readonly By _headerTitle = By.XPath("//p[@class='fontset-h1 text-green-200']//span[@class='text-primary' and text()='Green']/following-sibling::span[text()=' & Tasty']");
        private readonly By _viewMenuButton = By.XPath("//div[contains(@class, 'content-center') and contains(@class, 'min-h-')]/div/div/button");
        private readonly By _locationSection = By.XPath("//div[contains(@class, 'w-full') and contains(@class, 'mx-auto')]/article[2]/header/p");
        private readonly By _locationCard = By.XPath("//article[2]//div[contains(@class, 'p-6')]//div[contains(@class, 'flex') and contains(@class, 'gap-2.5')]//p");
        private readonly By _bookTableLink = By.XPath("//header//section//div[contains(@class, 'flex')]/a[2]");
        private readonly By _reservationsLink = By.XPath("//article[2]/div/div[2]/div[contains(@class, 'w-full') and contains(@class, 'h-140px')]");
        private readonly By _macAndCheeseLink = By.CssSelector("#root > div.w-full.mx-auto.max-w-\\5B 1440px\\5D .px-\\5B 40px\\5D .pt-\\5B 4rem\\5D .pb-\\5B 2\\.5rem\\5D > article:nth-child(1) > div > div:nth-child(1) > p");
        private readonly By _chickenTikkaMasalaLink = By.XPath("//p[contains(text(), 'Chicken Tikka Masala')]");
        private readonly By _cheeseburgerLink = By.XPath("//p[contains(text(), 'Cheeseburger')]");

        public MainPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        // Проверка, что заголовок главной страницы отображается
        public bool IsHeaderDisplayed()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(_headerTitle)).Displayed;
        }

        // Проверка, что кнопка "View Menu" отображается
        public bool IsViewMenuButtonDisplayed()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(_viewMenuButton)).Displayed;
        }

        public bool IsLocationSectionDisplayed()
        {
            var element = _driver.FindElement(_locationSection);

            // Прокручиваем к элементу, чтобы он стал видимым
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);

            // Ожидаем, пока элемент станет видимым
            return _wait.Until(ExpectedConditions.ElementIsVisible(_locationSection)).Displayed;
        }

        // Проверка, что ссылка "Book a Table" отображается
        public bool IsBookTableLinkDisplayed()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(_bookTableLink)).Displayed;
        }

        public bool IsMacAndCheeseDisplayed()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(_macAndCheeseLink)).Displayed;
        }

        public void ClickMacAndCheese()
        {
            var macAndCheese = _wait.Until(ExpectedConditions.ElementToBeClickable(_macAndCheeseLink));
            macAndCheese.Click();
        }

        public bool IsChickenTikkaMasalaDisplayed()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(_chickenTikkaMasalaLink)).Displayed;
        }

        public void ClickChickenTikkaMasala()
        {
            var chickenTikkaMasala = _wait.Until(ExpectedConditions.ElementToBeClickable(_chickenTikkaMasalaLink));
            chickenTikkaMasala.Click();
        }

        public bool IsCheeseburgerDisplayed()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(_cheeseburgerLink)).Displayed;
        }

        public void ClickCheeseburger()
        {
            var cheeseburger = _wait.Until(ExpectedConditions.ElementToBeClickable(_cheeseburgerLink));
            cheeseburger.Click();
        }

        // Проверка, что ссылка "Reservations" отображается
        public bool IsReservationsLinkDisplayed()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(_reservationsLink)).Displayed;
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

            // Прокручиваем к элементу, чтобы он стал видимым
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", locationCard);

            // Используем JavaScript для клика, чтобы обойти ограничения Selenium
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", locationCard);

            // Возвращаем новую страницу, передав _driver в конструктор LocationOverviewPage
            return new LocationOverviewPage(_driver);
        }
    }
}
