using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace automation_qa.UI.Pages
{
    public class UserProfilePage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Locators for profile navigation
        private readonly By _signInButton = By.XPath("/html/body/div/header/div/section/div[3]/a");
        private readonly By _profileIconButton = By.XPath("/html/body/div/header/div/section/div[3]/button[2]");
        private readonly By _myProfileOption = By.XPath("/html/body/div[2]/div/a/p");

        // Locators for profile page elements
        private readonly By _firstNameInput = By.XPath("/html/body/div/div/div/div[2]/form/div/div/div[2]/div/div[1]/input");
        private readonly By _lastNameInput = By.XPath("/html/body/div/div/div/div[2]/form/div/div/div[2]/div/div[2]/input");
        private readonly By _saveChangesButton = By.XPath("/html/body/div/div/div/div[2]/form/div/div/div[2]/button");
        private readonly By _uploadPhotoButton = By.XPath("/html/body/div/div/div/div[2]/form/div/div/div[1]/div[2]/button");
        private readonly By _changePasswordLink = By.XPath("/html/body/div/div/div/div[1]/button[2]/p");

        // For checking successful save
        private readonly By _successMessage = By.CssSelector(".toast-success, .success-message");

        public UserProfilePage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // Check if sign-in is required
        public bool NeedToSignIn()
        {
            try
            {
                return _driver.FindElements(_signInButton).Count > 0;
            }
            catch
            {
                return false;
            }
        }

        // Navigation to profile page
        public UserProfilePage NavigateToProfile()
        {
            try
            {
                // Check if login is required
                if (NeedToSignIn())
                {
                    Console.WriteLine("User is not logged in. Sign in first before accessing profile.");
                    throw new InvalidOperationException("User is not logged in. Sign in first before accessing profile.");
                }

                // Wait until the profile button is visible and clickable
                _wait.Until(d => {
                    try
                    {
                        var element = d.FindElement(_profileIconButton);
                        return element.Displayed && element.Enabled;
                    }
                    catch
                    {
                        return false;
                    }
                });

                // Click on the profile icon
                _driver.FindElement(_profileIconButton).Click();

                // Wait until the My Profile option becomes visible
                _wait.Until(d => {
                    try
                    {
                        var element = d.FindElement(_myProfileOption);
                        return element.Displayed;
                    }
                    catch
                    {
                        return false;
                    }
                });

                // Click on My Profile
                _driver.FindElement(_myProfileOption).Click();

                // Wait for the profile page to load (check for firstName field)
                _wait.Until(d => {
                    try
                    {
                        return d.FindElement(_firstNameInput).Displayed;
                    }
                    catch
                    {
                        return false;
                    }
                });

                return this;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error navigating to profile: {ex.Message}");
                ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile("profile_navigation_error.png");
                throw;
            }
        }

        // Methods for working with the profile form
        public string GetCurrentFirstName()
        {
            try
            {
                IWebElement firstNameInput = _driver.FindElement(_firstNameInput);
                return firstNameInput.GetAttribute("value");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting first name: {ex.Message}");
                return string.Empty;
            }
        }

        public string GetCurrentLastName()
        {
            try
            {
                IWebElement lastNameInput = _driver.FindElement(_lastNameInput);
                return lastNameInput.GetAttribute("value");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting last name: {ex.Message}");
                return string.Empty;
            }
        }

        public UserProfilePage UpdateFirstName(string firstName)
        {
            try
            {
                IWebElement firstNameInput = _driver.FindElement(_firstNameInput);

                // Completely clear the field using different methods
                firstNameInput.Clear();
                // Additionally use JavaScript for clearing
                IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                js.ExecuteScript("arguments[0].value = '';", firstNameInput);

                // Additionally can clear using key combinations
                firstNameInput.SendKeys(OpenQA.Selenium.Keys.Control + "a");
                firstNameInput.SendKeys(OpenQA.Selenium.Keys.Delete);

                // After complete clearing, enter the new value
                firstNameInput.SendKeys(firstName);
                return this;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating first name: {ex.Message}");
                ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile("update_firstname_error.png");
                throw;
            }
        }

        public UserProfilePage UpdateLastName(string lastName)
        {
            try
            {
                IWebElement lastNameInput = _driver.FindElement(_lastNameInput);

                // Completely clear the field using different methods
                lastNameInput.Clear();
                // Additionally use JavaScript for clearing
                IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                js.ExecuteScript("arguments[0].value = '';", lastNameInput);

                // Additionally can clear using key combinations
                lastNameInput.SendKeys(OpenQA.Selenium.Keys.Control + "a");
                lastNameInput.SendKeys(OpenQA.Selenium.Keys.Delete);

                // After complete clearing, enter the new value
                lastNameInput.SendKeys(lastName);
                return this;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating last name: {ex.Message}");
                ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile("update_lastname_error.png");
                throw;
            }
        }

        public UserProfilePage ClickSaveChanges()
        {
            try
            {
                // Scroll to the Save Changes button
                IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                IWebElement saveButton = _driver.FindElement(_saveChangesButton);
                js.ExecuteScript("arguments[0].scrollIntoView(true);", saveButton);

                // Wait until the button becomes clickable
                _wait.Until(d => {
                    try
                    {
                        var element = d.FindElement(_saveChangesButton);
                        return element.Displayed && element.Enabled;
                    }
                    catch
                    {
                        return false;
                    }
                });

                // Click the button
                saveButton.Click();
                return this;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clicking Save Changes: {ex.Message}");
                ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile("save_changes_error.png");
                throw;
            }
        }

        public UserProfilePage NavigateToChangePassword()
        {
            try
            {
                _driver.FindElement(_changePasswordLink).Click();
                return this;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error navigating to Change Password: {ex.Message}");
                ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile("change_password_nav_error.png");
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
                    catch
                    {
                        // Try alternative locator for the message
                        try
                        {
                            return d.FindElement(By.XPath("//div[contains(text(), 'successfully') or contains(text(), 'Success')]")).Displayed;
                        }
                        catch
                        {
                            return false;
                        }
                    }
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Method for complex profile update
        public bool UpdateProfile(string firstName, string lastName)
        {
            try
            {
                UpdateFirstName(firstName);
                UpdateLastName(lastName);
                ClickSaveChanges();
                return IsSuccessMessageDisplayed();
            }
            catch
            {
                return false;
            }
        }
    }
}
