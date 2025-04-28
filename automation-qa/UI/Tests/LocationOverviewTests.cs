using NUnit.Framework;
using automation_qa.UI.Pages;

namespace automation_qa.UI.Tests
{
    [TestFixture]
    [Category("Location")]
    public class LocationOverviewTests : BaseTest
    {
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
        [Category("Smoke")]
        [Category("Regression")]
        public void LocationOverview_ShouldDisplayTitle()
        {
            bool isTitleDisplayed = _locationPage.IsLocationTitleDisplayed();
            Assert.That(isTitleDisplayed, Is.True, "Title '14 Baratashvili Street' should be displayed after navigating to LocationOverviewPage");
        }

        [Test]
        [Category("Regression")]
        public void LocationOverview_ShouldDisplayReviewStars()
        {
            Assert.That(_locationPage.AreReviewStarsDisplayed(), Is.True, "Stars in customer reviews should be displayed");
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public void LocationOverview_ShouldDisplaySpecialityDishesSection()
        {
            Assert.That(_locationPage.IsSpecialityDishesSectionDisplayed(), Is.True, "Speciality dishes section should be displayed");
        }

        [Test]
        [Category("Regression")]
        public void LocationOverview_ShouldDisplayDishCards()
        {
            Assert.That(_locationPage.AreDishCardsDisplayed(), Is.True, "Dish cards should be displayed on the location page");
            Assert.That(_locationPage.GetDishCardsCount(), Is.EqualTo(2), "There should be exactly 2 dish cards displayed");
        }

        [Test]
        [Category("Regression")]
        public void LocationOverview_ShouldDisplayFeedbackSection()
        {
            Assert.That(_locationPage.IsFeedbackSectionDisplayed(), Is.True, "Customer reviews section should be displayed");
        }

        [Test]
        [Category("Regression")]
        public void LocationOverview_ShouldBeAbleToClickCuisineExperienceFilter()
        {
            Assert.That(_locationPage.IsCuisineExperienceFilterClickable(), Is.True, "Cuisine experience filter should be clickable");
            _locationPage.ClickCuisineExperienceFilter();
            Assert.That(_locationPage.IsCuisineExperienceFilterClickable(), Is.True, "Cuisine experience filter should remain clickable after clicking");
        }

        [Test]
        [Category("Regression")]
        public void LocationOverview_ShouldBeAbleToClickMainPageLinkAndNavigate()
        {
            Console.WriteLine($"Page HTML before clicking Main page link: {Driver.PageSource}");
            Assert.That(_locationPage.IsMainPageLinkClickable(), Is.True, "Main page link should be clickable");
            var mainPage = _locationPage.ClickMainPageLink();
            Assert.That(mainPage.IsHeaderDisplayed(), Is.True, "Should navigate to Main page and display header");
        }
    }
}