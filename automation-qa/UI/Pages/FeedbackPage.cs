using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace automation_qa.UI.Pages
{
    public class FeedbackPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        private readonly By _reservationsLink = By.XPath("//a[contains(text(), 'Reservations') or contains(@href, '/reservations')]");
        private readonly By _leaveFeedbackButton = By.XPath("//*[@id=\"root\"]/div/div/article/footer/button");
        private readonly By _feedbackTextArea = By.XPath("/html/body/div[3]/form/div[2]/div/div[2]/textarea");
        private readonly By _submitButton = By.XPath("/html/body/div[3]/form/div[3]/button");
        private readonly By _successMessage = By.CssSelector(".success-message, .toast-success");

        public FeedbackPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public FeedbackPage NavigateToReservations()
        {
            try
            {
                _wait.Until(d => d.FindElement(_reservationsLink).Displayed);
                _driver.FindElement(_reservationsLink).Click();
                return this;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error navigating to Reservations: {ex.Message}");
                ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile("reservation_nav_error.png");
                throw;
            }
        }

        public FeedbackPage ClickLeaveFeedbackButton()
        {
            try
            {
                _wait.Until(d => d.FindElement(_leaveFeedbackButton).Displayed);
                _driver.FindElement(_leaveFeedbackButton).Click();
                return this;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clicking Leave Feedback button: {ex.Message}");
                ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile("feedback_button_error.png");
                throw;
            }
        }

        public FeedbackPage EnterFeedbackText(string text)
        {
            try
            {
                _wait.Until(d => d.FindElement(_feedbackTextArea).Displayed);
                _driver.FindElement(_feedbackTextArea).Clear();
                _driver.FindElement(_feedbackTextArea).SendKeys(text);
                return this;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error entering feedback text: {ex.Message}");
                ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile("feedback_text_error.png");
                throw;
            }
        }

        public FeedbackPage ClickSubmitButton()
        {
            try
            {
                _wait.Until(d => d.FindElement(_submitButton).Displayed);
                _driver.FindElement(_submitButton).Click();
                return this;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clicking Submit button: {ex.Message}");
                ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile("submit_button_error.png");
                throw;
            }
        }

        public bool IsSuccessMessageDisplayed()
        {
            try
            {
                _wait.Until(d => {
                    try
                    {
                        return d.FindElement(_successMessage).Displayed;
                    }
                    catch (NoSuchElementException)
                    {
                        try
                        {
                            return d.FindElement(By.XPath("//div[contains(@class, 'toast-success') or contains(@class, 'success-message')]")).Displayed;
                        }
                        catch
                        {
                            try
                            {
                                return d.FindElement(By.XPath("//div[contains(text(), 'Thank you') or contains(text(), 'successfully')]")).Displayed;
                            }
                            catch
                            {
                                return false;
                            }
                        }
                    }
                });
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile("success_message_error.png");
                return false;
            }
        }

        public void SubmitFeedback(string feedbackText)
        {
            NavigateToReservations();
            ClickLeaveFeedbackButton();
            EnterFeedbackText(feedbackText);
            ClickSubmitButton();
        }
    }
}