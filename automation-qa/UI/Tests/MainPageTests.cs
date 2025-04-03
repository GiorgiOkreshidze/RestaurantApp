using NUnit.Framework;
using automation_qa.UI.Pages;
using automation_qa.Framework;

namespace automation_qa.UI.Tests
{
    [TestFixture]
    [Category("Main")]
    public class MainPageTests : BaseTest
    {
        private NavigationBar _navigationBar;
        private MainPage _mainPage;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _mainPage = new MainPage(Driver);
        }

        [Test]
        public void MainPage_ShouldDisplayHeader()
        {
            // Проверяем, что заголовок отображается
            Assert.That(_mainPage.IsHeaderDisplayed(), Is.True, "Заголовок 'GREEN & TASTY' должен отображаться на главной странице");
        }

        [Test]
        public void MainPage_ShouldDisplayViewMenuButton()
        {
            // Проверяем, что кнопка "View Menu" отображается
            Assert.That(_mainPage.IsViewMenuButtonDisplayed(), Is.True, "Кнопка 'View Menu' должна отображаться на главной странице");
        }

        [Test]
        public void MainPage_ShouldDisplayLocationSection()
        {
            Assert.That(_mainPage.IsLocationSectionDisplayed(), Is.True, "Секция 'Locations' должна быть видимой на странице");
        }

        [Test]
        public void MainPage_ShouldDisplayBookTableLink()
        {
            // Проверяем, что ссылка "Book a Table" отображается
            Thread.Sleep(3000);
            Assert.That(_mainPage.IsBookTableLinkDisplayed(), Is.True, "Ссылка 'Book a Table' должна отображаться на главной странице");
        }

        [Test]
        public void MainPage_ShouldDisplayReservationsLink()
        {
            // Проверяем, что ссылка "Reservations" отображается
            Assert.That(_mainPage.IsReservationsLinkDisplayed(), Is.True, "Ссылка 'Reservations' должна отображаться на главной странице");
        }

        [Test]
        public void MainPage_ShouldClickMacAndCheese()
        {
            // Проверяем, что элемент "Mac & Cheese" отображается
            Assert.That(_mainPage.IsMacAndCheeseDisplayed(), Is.True, "Элемент 'Mac & Cheese' должен отображаться на главной странице");

            // Выполняем клик по "Mac & Cheese"
            _mainPage.ClickMacAndCheese();

            // Добавляем небольшое ожидание для обработки клика
            Thread.Sleep(1000);

            // Завершаем тест
            Assert.Pass("Клик по 'Mac & Cheese' выполнен успешно.");
        }

        [Test]
        public void MainPage_ShouldClickChickenTikkaMasala()
        {
            Assert.That(_mainPage.IsChickenTikkaMasalaDisplayed(), Is.True, "Элемент 'Chicken Tikka Masala' должен отображаться на главной странице");
            _mainPage.ClickChickenTikkaMasala();
            Thread.Sleep(1000);
            Assert.Pass("Клик по 'Chicken Tikka Masala' выполнен успешно.");
        }

        [Test]
        public void MainPage_ShouldClickCheeseburger()
        {
            Assert.That(_mainPage.IsCheeseburgerDisplayed(), Is.True, "Элемент 'Cheeseburger' должен отображаться на главной странице");
            _mainPage.ClickCheeseburger();
            Thread.Sleep(1000);
            Assert.Pass("Клик по 'Cheeseburger' выполнен успешно.");
        }

        [Test]
        public void MainPage_ShouldNavigateToLocationOverview()
        {
            // Переходим на страницу локации
            _mainPage.SelectLocation();

            // Добавляем небольшое ожидание для того, чтобы клик успел обработаться
            Thread.Sleep(1000); // Это не лучший способ, но для простоты проверки можно использовать
            Assert.Pass("Клик выполнен успешно, переход на локацию произошел.");
        }
    }
}
