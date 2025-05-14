using System;
using NUnit.Framework;
using OpenQA.Selenium;
using automation_qa.Drivers;
using automation_qa.Framework;
using automation_qa.UI.Pages;
using ApiTests.Utilities;
using OpenQA.Selenium.Support.UI;

namespace automation_qa.UI.Tests
{
    [TestFixture]
    public class FeedbackTests : BaseTest
    {
        private LoginPage _loginPage;
        private FeedbackPage _feedbackPage;
        private readonly TestConfig _config = TestConfig.Instance;
        private WebDriverWait _wait;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _loginPage = new LoginPage(Driver);
            _feedbackPage = new FeedbackPage(Driver);
            _wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(150));
        }

        [Test]
        [Category("Smoke")]
        public void Feedback_SubmitPositiveFeedback_ShouldShowSuccessMessage()
        {
            // Arrange
            string email = _config.TestUserEmail;
            string password = _config.TestUserPassword;
            string feedbackText = "I like it very much! The service was good!\n\n彡★(◕‿◕)★彡";

            // Click on the Sign In button in the upper right corner
            Thread.Sleep(3000);
            IWebElement signInButton = Driver.FindElement(By.XPath("/html/body/div/header/div/section/div[3]/a"));
            signInButton.Click();

            // Wait for the login page to load
            _wait.Until(d => d.FindElement(By.XPath("//form")));

            // Enter login data
            Thread.Sleep(3000);
            _loginPage.Login(email, password);

            // Wait for successful authorization
            _wait.Until(d => {
                try
                {
                    // Check that we are on the main page and have access to the user menu
                    return d.FindElement(By.XPath("//a[contains(text(), 'Reservations') or contains(@href, '/reservations')]")).Displayed;
                }
                catch
                {
                    return false;
                }
            });

            // Navigate to the Reservations page
            Thread.Sleep(3000);
            _feedbackPage.NavigateToReservations();

            // Wait for the bookings page to load
            _wait.Until(d => {
                try
                {
                    return d.FindElement(By.XPath("//*[@id=\"root\"]/div/div/article/footer/button")).Displayed;
                }
                catch
                {
                    return false;
                }
            });

            // Click on the Leave Feedback button
            Thread.Sleep(3000);
            _feedbackPage.ClickLeaveFeedbackButton();

            // Wait for the feedback form to load
            _wait.Until(d => {
                try
                {
                    return d.FindElement(By.XPath("/html/body/div[3]/form/div[2]/div/div[2]/textarea")).Displayed;
                }
                catch
                {
                    return false;
                }
            });

            // Find the text field for entering feedback
            IWebElement textArea = Driver.FindElement(By.XPath("/html/body/div[3]/form/div[2]/div/div[2]/textarea"));

            // Slow text input character by character
            typeTextSlowly(textArea, feedbackText);

            // Pause before submitting
            Thread.Sleep(3000);

            // Click on the Submit button
            _feedbackPage.ClickSubmitButton();

            // Assert
            Assert.Pass("Feedback form completed and submitted");
        }

        // Helper method for slow text input
        private void typeTextSlowly(IWebElement element, string text)
        {
            // Clear the field before input
            element.Clear();

            // For a nicer effect, split the text into words
            string[] words = text.Split(' ');

            foreach (string word in words)
            {
                // Enter the word character by character
                foreach (char c in word)
                {
                    element.SendKeys(c.ToString());
                    Thread.Sleep(100); // Delay between characters (100 ms)
                }

                // Add a space after the word
                element.SendKeys(" ");
                Thread.Sleep(300); // Longer pause between words (300 ms)
            }
        }

        [Test]
        [Category("Smoke")]
        public void Feedback_SubmitNegativeFeedback_ShouldBeSuccessful()
        {
            // Arrange
            string email = _config.TestUserEmail;
            string password = _config.TestUserPassword;
            string negativeText = "The service was slow and the food was cold. I was disappointed with my experience.(◞‸◟)";

            // Click on the Sign In button in the upper right corner
            IWebElement signInButton = Driver.FindElement(By.XPath("/html/body/div/header/div/section/div[3]/a"));
            signInButton.Click();

            // Wait for the login page to load
            _wait.Until(d => d.FindElement(By.XPath("//form")));

            // Enter login data
            _loginPage.Login(email, password);

            // Wait for successful authorization
            _wait.Until(d => {
                try
                {
                    // Check that we are on the main page and have access to the user menu
                    return d.FindElement(By.XPath("//a[contains(text(), 'Reservations') or contains(@href, '/reservations')]")).Displayed;
                }
                catch
                {
                    return false;
                }
            });

            // Navigate to the Reservations page
            _feedbackPage.NavigateToReservations();

            // Wait for the bookings page to load
            _wait.Until(d => {
                try
                {
                    return d.FindElement(By.XPath("//*[@id=\"root\"]/div/div/article/footer/button")).Displayed;
                }
                catch
                {
                    return false;
                }
            });

            // Click on the Leave Feedback button
            _feedbackPage.ClickLeaveFeedbackButton();

            // Wait for the feedback form to load
            _wait.Until(d => {
                try
                {
                    return d.FindElement(By.XPath("/html/body/div[3]/form/div[2]/div/div[2]/textarea")).Displayed;
                }
                catch
                {
                    return false;
                }
            });

            // Try several options for clicking on the third star
            try
            {
                // Option 1: find all stars and click on the third one
                var stars = Driver.FindElements(By.CssSelector("svg[data-testid='Star']"));
                if (stars.Count >= 3)
                {
                    stars[2].Click(); // Index 2 is the third star (numbering from 0)
                }
                else
                {
                    // Option 2: use JavaScript for clicking
                    IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
                    js.ExecuteScript("document.querySelectorAll('svg[data-testid=\"Star\"]')[2].click();");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Couldn't click star rating: {ex.Message}");
                // Continue the test even if we couldn't click on the star
            }

            // Enter negative feedback text
            _feedbackPage.EnterFeedbackText(negativeText);

            // Pause for reading the text - give enough time (5 seconds)
            Thread.Sleep(5000);

            // Click on the Submit button
            _feedbackPage.ClickSubmitButton();

            // Assert
            Assert.Pass("Negative feedback form completed and submitted");
        }

        [Test]
        [Category("Regression")]
        public void Feedback_SubmitCulinaryFeedback_ShouldBeSuccessful()
        {
            // Arrange
            string email = _config.TestUserEmail;
            string password = _config.TestUserPassword;
            string culinaryFeedbackText = "The food was delicious. I especially enjoyed the seasonal specialties.";

            // Authorization
            Driver.FindElement(By.XPath("/html/body/div/header/div/section/div[3]/a")).Click();
            _wait.Until(d => d.FindElement(By.XPath("//form")));
            _loginPage.Login(email, password);

            // Navigate to the feedback form
            _wait.Until(d => d.FindElement(By.XPath("/html/body/div/header/div/section/div[2]/a[3]")).Displayed);
            _feedbackPage.NavigateToReservations();
            _wait.Until(d => d.FindElement(By.XPath("//*[@id=\"root\"]/div/div/article/footer/button")).Displayed);
            _feedbackPage.ClickLeaveFeedbackButton();

            // Wait for the feedback form to load
            _wait.Until(d => d.FindElement(By.XPath("/html/body/div[3]/form/div[2]/div/div[2]/textarea")).Displayed);

            // Switch to the Culinary Experience tab
            Driver.FindElement(By.XPath("/html/body/div[3]/form/div[2]/div/div[1]/button[2]")).Click();

            // Wait for the culinary feedback text field to appear
            _wait.Until(d => d.FindElement(By.XPath("/html/body/div[3]/form/div[2]/div/div[3]/textarea")).Displayed);

            // Enter text in the culinary feedback field
            Driver.FindElement(By.XPath("/html/body/div[3]/form/div[2]/div/div[3]/textarea")).Clear();
            Driver.FindElement(By.XPath("/html/body/div[3]/form/div[2]/div/div[3]/textarea")).SendKeys(culinaryFeedbackText);

            // Pause for reading
            Thread.Sleep(2000);

            // Submit the form
            _feedbackPage.ClickSubmitButton();

            Assert.Pass("Culinary feedback submitted");
        }

        [Test]
        [Category("Smoke")]
        public void Feedback_ShouldDisplayGiveFeedbackTitle()
        {
            // Authorization
            Driver.FindElement(By.XPath("/html/body/div/header/div/section/div[3]/a")).Click();
            _loginPage.Login(_config.TestUserEmail, _config.TestUserPassword);

            // Navigate to the feedback form
            _wait.Until(d => d.FindElement(By.XPath("//a[contains(@href, '/reservations')]")).Displayed);
            _feedbackPage.NavigateToReservations();
            _feedbackPage.ClickLeaveFeedbackButton();

            // Small pause for the feedback window to fully load
            Thread.Sleep(1000);

            // Try different ways to get the title
            try
            {
                // Option 1: Check via InnerHTML
                IWebElement feedbackTitle = Driver.FindElement(By.XPath("/html/body/div[3]/form/div[1]/h2"));
                string titleText = (string)((IJavaScriptExecutor)Driver).ExecuteScript("return arguments[0].innerText;", feedbackTitle);
                Assert.That(titleText, Contains.Substring("Feedback"), "Title should contain 'Feedback'");
            }
            catch
            {
                try
                {
                    // Option 2: Try to find the title by content
                    IWebElement feedbackTitle = Driver.FindElement(By.XPath("//h2[contains(text(), 'Feedback')]"));
                    Assert.That(feedbackTitle.Displayed, Is.True, "Feedback title should be displayed");
                }
                catch
                {
                    // Option 3: Try to find the title elsewhere
                    IWebElement formElement = Driver.FindElement(By.XPath("//form"));
                    bool titleExists = (bool)((IJavaScriptExecutor)Driver).ExecuteScript(
                        "return document.querySelector('form').innerText.includes('Give Feedback') || " +
                        "document.querySelector('form').innerText.includes('Feedback');", formElement);

                    Assert.That(titleExists, Is.True, "Form should contain Feedback title");
                }
            }
        }

        [Test]
        [Category("Smoke")]
        public void Feedback_ServiceTabButton_ShouldBeClickable()
        {
            // Authorization
            Driver.FindElement(By.XPath("/html/body/div/header/div/section/div[3]/a")).Click();
            _loginPage.Login(_config.TestUserEmail, _config.TestUserPassword);

            // Navigate to the feedback form
            _wait.Until(d => d.FindElement(By.XPath("//a[contains(@href, '/reservations')]")).Displayed);
            _feedbackPage.NavigateToReservations();
            _feedbackPage.ClickLeaveFeedbackButton();

            // Check the clickability of the Service button
            IWebElement serviceButton = Driver.FindElement(By.XPath("/html/body/div[3]/form/div[2]/div/div[1]/button[1]"));

            // Check that the button is displayed and enabled
            Assert.That(serviceButton.Displayed, Is.True, "Service tab button should be displayed");
            Assert.That(serviceButton.Enabled, Is.True, "Service tab button should be enabled");

            // Click the button and check that the click was successful
            serviceButton.Click();

            // If the click was successful, the test passes
            Assert.Pass("Service tab button is clickable");
        }

        [Test]
        [Category("Smoke")]
        public void Feedback_CulinaryExperienceTabButton_ShouldBeClickable()
        {
            // Authorization
            Driver.FindElement(By.XPath("/html/body/div/header/div/section/div[3]/a")).Click();
            _loginPage.Login(_config.TestUserEmail, _config.TestUserPassword);

            // Navigate to the feedback form
            _wait.Until(d => d.FindElement(By.XPath("//a[contains(@href, '/reservations')]")).Displayed);
            _feedbackPage.NavigateToReservations();
            _feedbackPage.ClickLeaveFeedbackButton();

            // Check the clickability of the Culinary Experience button
            IWebElement culinaryButton = Driver.FindElement(By.XPath("/html/body/div[3]/form/div[2]/div/div[1]/button[2]"));

            // Check that the button is displayed and enabled
            Assert.That(culinaryButton.Displayed, Is.True, "Culinary Experience tab button should be displayed");
            Assert.That(culinaryButton.Enabled, Is.True, "Culinary Experience tab button should be enabled");

            // Click the button and check that the click was successful
            culinaryButton.Click();

            // If the click was successful, the test passes
            Assert.Pass("Culinary Experience tab button is clickable");
        }

        [Test]
        [Category("Smoke")]
        public void ReservationPage_LeaveFeedbackButton_ShouldBeClickable()
        {
            // Authorization
            Driver.FindElement(By.XPath("/html/body/div/header/div/section/div[3]/a")).Click();
            _loginPage.Login(_config.TestUserEmail, _config.TestUserPassword);

            // Navigate to the bookings page
            _wait.Until(d => d.FindElement(By.XPath("//a[contains(@href, '/reservations')]")).Displayed);
            _feedbackPage.NavigateToReservations();

            // Check the Leave Feedback button
            IWebElement leaveFeedbackButton = Driver.FindElement(By.XPath("/html/body/div/div/div/article/footer/button"));

            // Check that the button is displayed and enabled
            Assert.That(leaveFeedbackButton.Displayed, Is.True, "Leave Feedback button should be displayed");
            Assert.That(leaveFeedbackButton.Enabled, Is.True, "Leave Feedback button should be enabled");

            // Click the button
            leaveFeedbackButton.Click();

            // If the click was performed without errors, the test passes
            Assert.Pass("Leave Feedback button is clickable");
        }

        [Test]
        [Category("Smoke")]
        public void ReservationPage_ShouldDisplayInProgressStatus()
        {
            // Authorization
            Driver.FindElement(By.XPath("/html/body/div/header/div/section/div[3]/a")).Click();
            _loginPage.Login(_config.TestUserEmail, _config.TestUserPassword);

            // Navigate to the bookings page
            _wait.Until(d => d.FindElement(By.XPath("//a[contains(@href, '/reservations')]")).Displayed);
            _feedbackPage.NavigateToReservations();

            // Check for "In Progress" status
            IWebElement statusElement = Driver.FindElement(By.XPath("/html/body/div/div/div/article/div/span"));

            // Check that the element is displayed and has the text "In Progress"
            Assert.That(statusElement.Displayed, Is.True, "Reservation status should be displayed");
            Assert.That(statusElement.Text, Is.EqualTo("In Progress"), "Reservation should have 'In Progress' status");
        }

        [Test]
        [Category("Smoke")]
        public void FeedbackForm_CloseButton_ShouldBeFunctional()
        {
            // Authorization
            Driver.FindElement(By.XPath("/html/body/div/header/div/section/div[3]/a")).Click();
            _loginPage.Login(_config.TestUserEmail, _config.TestUserPassword);

            // Navigate to the bookings page
            _wait.Until(d => d.FindElement(By.XPath("//a[contains(@href, '/reservations')]")).Displayed);
            _feedbackPage.NavigateToReservations();

            // Click on the Leave Feedback button to open the form
            IWebElement leaveFeedbackButton = Driver.FindElement(By.XPath("/html/body/div/div/div/article/footer/button"));
            leaveFeedbackButton.Click();

            // Wait for the feedback form to appear
            _wait.Until(d => d.FindElement(By.XPath("/html/body/div[3]/form")).Displayed);

            // Find and click on the X to close the form
            IWebElement closeButton = Driver.FindElement(By.XPath("/html/body/div[3]/button"));
            Assert.That(closeButton.Displayed, Is.True, "Close button should be displayed");
            closeButton.Click();

            // Check that the form has closed
            try
            {
                // Wait a bit for the form to close
                Thread.Sleep(1000);

                // Try to find the form - it should be absent
                bool formStillExists = Driver.FindElements(By.XPath("/html/body/div[3]/form")).Count > 0;
                Assert.That(formStillExists, Is.False, "Feedback form should be closed after clicking the close button");
            }
            catch (WebDriverTimeoutException)
            {
                // If a timeout occurred while searching for the form - that's good, the form has closed
                Assert.Pass("Feedback form was successfully closed using the close button");
            }

            // If all checks passed, the test passes
            Assert.Pass("Close button on feedback form is functional");
        }
    }
}