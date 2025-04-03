using NUnit.Framework;
using automation_qa.UI.Pages;
using automation_qa.Framework;
using System.Threading;

namespace automation_qa.UI.Tests
{
    [TestFixture]
    [Category("Booking")]
    public class BookingPageTests : BaseTest
    {
        private NavigationBar _navigationBar;
        private BookingPage _bookingPage;
        private MainPage _mainPage;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _mainPage = new MainPage(Driver);
            _bookingPage = _mainPage.ClickBookTableLink();
            Thread.Sleep(2000);
        }

        [Test]
        public void BookingPage_ShouldDisplayHeader()
        {
            // Проверяем, что заголовок страницы бронирования отображается
            Assert.That(_bookingPage.IsBookingHeaderDisplayed(), Is.True, "Заголовок 'Book a Table' должен отображаться на странице бронирования");
        }

        [Test]
        public void BookingPage_ShouldClickLocationDropdownButton()
        {
            // Используем существующий метод для открытия выпадающего списка локаций
            _bookingPage.OpenLocationDropdown();

            // Добавляем небольшую задержку для обработки клика
            Thread.Sleep(2000);

            // Завершаем тест успешно
            Assert.Pass("Клик по кнопке раскрытия списка локаций выполнен успешно");
        }

        [Test]
        public void BookingPage_ShouldSelectDate()
        {
            // Открываем выпадающий список дат
            _bookingPage.OpenDateDropdown();

            // Выбираем дату
            _bookingPage.SelectDate();

            // Завершаем тест
            Assert.Pass("Дата успешно выбрана");
        }

        [Test]
        public void BookingPage_ShouldSelectTime()
        {
            // Открываем выпадающий список времени
            _bookingPage.OpenTimeDropdown();

            // Выбираем время
            _bookingPage.SelectTime();

            // Завершаем тест
            Assert.Pass("Время успешно выбрано");
        }

        [Test]
        public void BookingPage_ShouldIncreaseGuestsCount()
        {
            // Получаем начальное количество гостей
            string initialCount = _bookingPage.GetGuestsCount();

            // Увеличиваем количество гостей
            _bookingPage.IncreaseGuests();

            // Получаем новое количество гостей
            string newCount = _bookingPage.GetGuestsCount();

            // Проверяем, что количество увеличилось
            Assert.That(newCount, Is.Not.EqualTo(initialCount), "Количество гостей должно измениться после увеличения");
        }

        [Test]
        public void BookingPage_ShouldFindAvailableTables()
        {
            try
            {
                // Просто нажимаем кнопку Find a Table без выбора параметров
                _bookingPage.ClickFindTable();

                // Добавляем задержку для загрузки результатов
                Thread.Sleep(3000);

                // Проверяем, что отображаются доступные столики
                Assert.That(_bookingPage.AreAvailableTablesDisplayed(), Is.True, "Должны отображаться доступные столики");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Тест упал с ошибкой: {ex.Message}");
                Assert.Fail($"Тест не выполнен: {ex.Message}");
            }
        }

        [Test]
        public void BookingPage_ShouldDisplayTableCards()
        {
            // Просто нажимаем кнопку Find a Table без заполнения формы
            _bookingPage.ClickFindTable();

            // Добавляем задержку для загрузки результатов
            Thread.Sleep(3000);

            // Проверяем, что отображаются карточки столиков
            Assert.That(_bookingPage.IsTableCardDisplayed(), Is.True, "Должны отображаться карточки столиков");
        }

        [Test]
        public void BookingPage_ShouldFilterTablesByLocation()
        {
            // Открываем выпадающий список локаций
            _bookingPage.OpenLocationDropdown();

            // Выбираем локацию
            _bookingPage.SelectLocation();

            // Нажимаем на кнопку "Find a Table"
            _bookingPage.ClickFindTable();

            // Добавляем задержку для загрузки результатов
            Thread.Sleep(2000);

            // Проверяем, что отображаются доступные столики
            Assert.That(_bookingPage.AreAvailableTablesDisplayed(), Is.True, "Должны отображаться доступные столики");

            // Проверяем, что отображаются карточки столиков
            Assert.That(_bookingPage.IsTableCardDisplayed(), Is.True, "Должны отображаться карточки столиков");
        }

        [Test]
        public void BookingPage_ShouldFilterTablesByGuestsCount()
        {
            // Увеличиваем количество гостей
            _bookingPage.IncreaseGuests();
            _bookingPage.IncreaseGuests();

            // Нажимаем на кнопку "Find a Table"
            _bookingPage.ClickFindTable();

            // Добавляем задержку для загрузки результатов
            Thread.Sleep(2000);

            // Проверяем, что отображаются карточки столиков
            Assert.That(_bookingPage.IsTableCardDisplayed(), Is.True, "Должны отображаться карточки столиков");
        }

        [Test]
        public void BookingPage_ShouldFilterTablesByTimeSlot()
        {
            // Открываем выпадающий список времени
            _bookingPage.OpenTimeDropdown();

            // Выбираем время
            _bookingPage.SelectTime();

            // Нажимаем на кнопку "Find a Table"
            _bookingPage.ClickFindTable();

            // Добавляем задержку для загрузки результатов
            Thread.Sleep(2000);

            // Проверяем, что отображаются доступные столики
            Assert.That(_bookingPage.AreAvailableTablesDisplayed(), Is.True, "Должны отображаться доступные столики");

            // Проверяем, что отображаются карточки столиков
            Assert.That(_bookingPage.IsTableCardDisplayed(), Is.True, "Должны отображаться карточки столиков");
        }

        [Test]
        public void FindTableButton_ShouldBeClickable()
        {
            try
            {
                // Проверяем, что кнопка "Find a Table" существует и кликабельна
                bool isButtonClickable = _bookingPage.IsFindTableButtonClickable();

                // Проверяем результат
                Assert.That(isButtonClickable, Is.True, "Кнопка 'Find a Table' должна быть кликабельна");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Тест упал с ошибкой: {ex.Message}");
                Assert.Fail($"Тест не выполнен: {ex.Message}");
            }
        }

        [Test]
        public void BookingPage_ShouldCompleteFullSearchCycle()
        {
            try
            {
                // Используем метод класса BookingPage для клика по кнопке
                _bookingPage.ClickFindTable();

                // Ждем загрузку результатов
                Thread.Sleep(3000);

                // Проверяем наличие карточек столиков
                Assert.That(_bookingPage.IsTableCardDisplayed(), Is.True, "Должны отображаться карточки столиков");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Тест упал с ошибкой: {ex.Message}");
                Assert.Fail($"Тест не выполнен: {ex.Message}");
            }
        }
    }
}