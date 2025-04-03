using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace automation_qa.UI.Pages
{
    public class LocationOverviewPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Локаторы для элементов на странице локации
        private readonly By _locationTitle = By.XPath("//h1[contains(text(), '14 Baratashvili Street')]");
        private readonly By _rating = By.XPath("//div[contains(@class, 'rating')]");
        private readonly By _feedbackSection = By.XPath("//h2[contains(text(), 'Customer Feedback')]");
        private readonly By _serviceFilter = By.XPath("//select/option[contains(text(), 'Service')]");
        private readonly By _cuisineFilter = By.XPath("//select/option[contains(text(), 'Cuisine Experience')]");
        private readonly By _sortByDate = By.XPath("//select/option[contains(text(), 'Date')]");
        private readonly By _sortByRating = By.XPath("//select/option[contains(text(), 'Rating')]");
        private readonly By _feedbackItems = By.XPath("//div[contains(@class, 'feedback-item')]");
        private readonly By _pagination = By.XPath("//div[contains(@class, 'pagination')]");

        public LocationOverviewPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        public bool IsLocationTitleDisplayed()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(_locationTitle)).Displayed;
        }

        public bool IsRatingDisplayed()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(_rating)).Displayed;
        }

        public bool IsFeedbackSectionDisplayed()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(_feedbackSection)).Displayed;
        }

        public void FilterFeedbackByService()
        {
            var filter = _wait.Until(ExpectedConditions.ElementToBeClickable(_serviceFilter));
            filter.Click();
        }

        public void FilterFeedbackByCuisine()
        {
            var filter = _wait.Until(ExpectedConditions.ElementToBeClickable(_cuisineFilter));
            filter.Click();
        }

        public void SortFeedbackByDate()
        {
            var sortOption = _wait.Until(ExpectedConditions.ElementToBeClickable(_sortByDate));
            sortOption.Click();
        }

        public void SortFeedbackByRating()
        {
            var sortOption = _wait.Until(ExpectedConditions.ElementToBeClickable(_sortByRating));
            sortOption.Click();
        }

        public List<IWebElement> GetFeedbackItems()
        {
            return _wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(_feedbackItems)).ToList();
        }

        public bool IsPaginationDisplayed()
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(_pagination)).Displayed;
        }
    }
}