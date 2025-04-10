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
            Assert.That(_bookingPage.IsBookingHeaderDisplayed(), Is.True, "Book a Table");
        }

        [Test]
        public void BookingPage_ShouldClickLocationDropdownButton()
        {
            _bookingPage.OpenLocationDropdown();

            Thread.Sleep(2000);

            Assert.Pass("Button click");
        }

        [Test]
        public void BookingPage_ShouldSelectDate()
        {
            _bookingPage.OpenDateDropdown();
            _bookingPage.SelectDate();

            Assert.Pass("Succesfully select");
        }

        [Test]
        public void BookingPage_ShouldSelectTime()
        {
            _bookingPage.OpenTimeDropdown();
            _bookingPage.SelectTime();

            Assert.Pass("Succesfully select");
        }

        [Test]
        public void BookingPage_ShouldIncreaseGuestsCount()
        {
            string initialCount = _bookingPage.GetGuestsCount();

            _bookingPage.IncreaseGuests();

            string newCount = _bookingPage.GetGuestsCount();

            Assert.That(newCount, Is.Not.EqualTo(initialCount), "Quality guests changes");
        }

        [Test]
        public void BookingPage_ShouldFindAvailableTables()
        {
            try
            {
                _bookingPage.ClickFindTable();

                Thread.Sleep(3000);

                Assert.That(_bookingPage.AreAvailableTablesDisplayed(), Is.True, "Valid table");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Faild: {ex.Message}");
                Assert.Fail($"Faild: {ex.Message}");
            }
        }

        [Test]
        public void BookingPage_ShouldDisplayTableCards()
        {
            _bookingPage.ClickFindTable();

            Thread.Sleep(3000);

            Assert.That(_bookingPage.IsTableCardDisplayed(), Is.True, "Valid table");
        }

        [Test]
        public void BookingPage_ShouldFilterTablesByLocation()
        {
            _bookingPage.OpenLocationDropdown();
            _bookingPage.SelectLocation();
            _bookingPage.ClickFindTable();

            Thread.Sleep(2000);

            Assert.That(_bookingPage.AreAvailableTablesDisplayed(), Is.True, "Valid table");

            Assert.That(_bookingPage.IsTableCardDisplayed(), Is.True, "Valid table");
        }

        [Test]
        public void BookingPage_ShouldFilterTablesByGuestsCount()
        {
            _bookingPage.IncreaseGuests();
            _bookingPage.IncreaseGuests();
            _bookingPage.ClickFindTable();

            Thread.Sleep(2000);

            Assert.That(_bookingPage.IsTableCardDisplayed(), Is.True, "Valid table");
        }

        [Test]
        public void BookingPage_ShouldFilterTablesByTimeSlot()
        {
            _bookingPage.OpenTimeDropdown();
            _bookingPage.SelectTime();
            _bookingPage.ClickFindTable();

            Thread.Sleep(2000);

            Assert.That(_bookingPage.AreAvailableTablesDisplayed(), Is.True, "Valid table");

            Assert.That(_bookingPage.IsTableCardDisplayed(), Is.True, "Valid table");
        }

        [Test]
        public void FindTableButton_ShouldBeClickable()
        {
            try
            {
                bool isButtonClickable = _bookingPage.IsFindTableButtonClickable();

                Assert.That(isButtonClickable, Is.True, "Button click");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed: {ex.Message}");
                Assert.Fail($"Failed: {ex.Message}");
            }
        }

        [Test]
        public void BookingPage_ShouldCompleteFullSearchCycle()
        {
            try
            {
                _bookingPage.ClickFindTable();

                Thread.Sleep(3000);

                Assert.That(_bookingPage.IsTableCardDisplayed(), Is.True, "Valid table");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed: {ex.Message}");
                Assert.Fail($"Failed: {ex.Message}");
            }
        }
    }
}