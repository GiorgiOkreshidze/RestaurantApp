using NUnit.Framework;
using automation_qa.UI.Pages;

namespace automation_qa.UI.Tests
{
    [TestFixture]
    public class LocationOverviewTests : BaseTest
    {
        private NavigationBar _navigationBar;
        private MainPage _mainPage;
        private LocationOverviewPage _locationPage;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _mainPage = new MainPage(Driver);
            _locationPage = _mainPage.SelectLocation();
        }

        [Test]
        public void LocationOverview_ShouldDisplayRatingAndFeedback()
        {
            Assert.That(_locationPage.IsRatingDisplayed(), Is.True, "");

            Assert.That(_locationPage.IsFeedbackSectionDisplayed(), Is.True, "");
        }

        [Test]
        public void LocationOverview_ShouldFilterFeedbackByService()
        {
            _locationPage.FilterFeedbackByService();

            var feedbackItems = _locationPage.GetFeedbackItems();
            Assert.That(feedbackItems.Count, Is.GreaterThan(0), "П");
        }

        [Test]
        public void LocationOverview_ShouldFilterFeedbackByCuisine()
        {
            _locationPage.FilterFeedbackByCuisine();

            var feedbackItems = _locationPage.GetFeedbackItems();
            Assert.That(feedbackItems.Count, Is.GreaterThan(0), "");
        }

        [Test]
        public void LocationOverview_ShouldSortFeedbackByDate()
        {
            _locationPage.SortFeedbackByDate();

            var feedbackItems = _locationPage.GetFeedbackItems();
            Assert.That(feedbackItems.Count, Is.GreaterThan(0), "");
        }

        [Test]
        public void LocationOverview_ShouldSortFeedbackByRating()
        {
            _locationPage.SortFeedbackByRating();

            var feedbackItems = _locationPage.GetFeedbackItems();
            Assert.That(feedbackItems.Count, Is.GreaterThan(0), "");
        }

        [Test]
        public void LocationOverview_ShouldDisplayFeedbackPagination()
        {
            Assert.That(_locationPage.IsPaginationDisplayed(), Is.True, "");
        }
    }
}
