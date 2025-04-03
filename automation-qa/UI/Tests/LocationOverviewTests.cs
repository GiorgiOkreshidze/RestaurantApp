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
            // Проверяем, что рейтинг отображается
            Assert.That(_locationPage.IsRatingDisplayed(), Is.True, "Рейтинг локации должен отображаться");

            // Проверяем, что секция отзывов отображается
            Assert.That(_locationPage.IsFeedbackSectionDisplayed(), Is.True, "Секция отзывов должна отображаться");
        }

        [Test]
        public void LocationOverview_ShouldFilterFeedbackByService()
        {
            // Фильтруем отзывы по сервису
            _locationPage.FilterFeedbackByService();

            // Проверяем, что отзывы отображаются после фильтрации
            var feedbackItems = _locationPage.GetFeedbackItems();
            Assert.That(feedbackItems.Count, Is.GreaterThan(0), "После фильтрации по сервису должны отображаться отзывы");
        }

        [Test]
        public void LocationOverview_ShouldFilterFeedbackByCuisine()
        {
            // Фильтруем отзывы по кулинарному опыту
            _locationPage.FilterFeedbackByCuisine();

            // Проверяем, что отзывы отображаются после фильтрации
            var feedbackItems = _locationPage.GetFeedbackItems();
            Assert.That(feedbackItems.Count, Is.GreaterThan(0), "После фильтрации по кулинарному опыту должны отображаться отзывы");
        }

        [Test]
        public void LocationOverview_ShouldSortFeedbackByDate()
        {
            // Сортируем отзывы по дате
            _locationPage.SortFeedbackByDate();

            // Проверяем, что отзывы отображаются после сортировки
            var feedbackItems = _locationPage.GetFeedbackItems();
            Assert.That(feedbackItems.Count, Is.GreaterThan(0), "После сортировки по дате должны отображаться отзывы");
        }

        [Test]
        public void LocationOverview_ShouldSortFeedbackByRating()
        {
            // Сортируем отзывы по рейтингу
            _locationPage.SortFeedbackByRating();

            // Проверяем, что отзывы отображаются после сортировки
            var feedbackItems = _locationPage.GetFeedbackItems();
            Assert.That(feedbackItems.Count, Is.GreaterThan(0), "После сортировки по рейтингу должны отображаться отзывы");
        }

        [Test]
        public void LocationOverview_ShouldDisplayFeedbackPagination()
        {
            // Проверяем, что пагинация отображается
            Assert.That(_locationPage.IsPaginationDisplayed(), Is.True, "Пагинация отзывов должна отображаться на странице");
        }
    }
}
