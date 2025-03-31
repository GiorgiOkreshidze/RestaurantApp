using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace automation_qa.UI.Pages
{
    public class NavigationBar
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Локаторы элементов
        private readonly By _signInButton = By.CssSelector("a[href='/signin']");
        private readonly By _loginLink = By.CssSelector("[data-testid='login-link']");
        private readonly By _createAccountLink = By.CssSelector("a[href='/signup']");
        private readonly By _logoutLink = By.LinkText("Sign Out");

        public NavigationBar(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void ClickSignInButton()
        {
            _driver.FindElement(_signInButton).Click();
        }

        public void GoToLoginPage()
        {
            // Кликаем на кнопку Sign In, которая уже ведет на страницу логина
            ClickSignInButton();
        }

        public void GoToRegistrationPage()
        {
            // Нажимаем на кнопку Sign In для перехода на страницу входа
            _driver.FindElement(_signInButton).Click();

            // Ждем загрузки страницы входа
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            // Находим ссылку Create an Account и нажимаем на неё
            wait.Until(driver => driver.FindElement(_createAccountLink)).Click();
        }

        public void Logout()
        {
            try
            {
                // На многих сайтах нужно сначала открыть меню пользователя
                // Здесь может потребоваться дополнительный клик на меню профиля перед выходом
                _driver.FindElement(_logoutLink).Click();
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Ссылка для выхода не найдена. Возможно, пользователь не вошел в систему.");
            }
        }

        public bool IsUserLoggedIn()
        {
            try
            {
                // Проверяем наличие кнопки выхода или другого элемента, 
                // который есть только для авторизованных пользователей
                return _driver.FindElement(_logoutLink).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
