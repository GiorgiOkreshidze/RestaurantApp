using System;
using System.Globalization;

namespace Function.Actions.Validators;

public static class ReservationValidator
{
    public static void ValidateGuestsNumber(string guestsNumber)
    {
        if (int.TryParse(guestsNumber, out var guests) && guests > 10)
        {
            throw new ArgumentException("The maximum number of guests allowed is 10");
        }
    }

    public static DateTime ParseDate(string date)
    {
        if (string.IsNullOrEmpty(date))
        {
            throw new ArgumentException("Date is required.");
        }

        if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            throw new ArgumentException("Invalid date format. Use yyyy-MM-dd.");
        }

        return parsedDate;
    }

    public static TimeSpan ParseTime(string time, string fieldName)
    {
        if (string.IsNullOrEmpty(time))
        {
            throw new ArgumentException($"{fieldName} is required.");
        }

        if (!TimeSpan.TryParseExact(time, "hh\\:mm", CultureInfo.InvariantCulture, out var parsedTime))
        {
            throw new ArgumentException($"Invalid {fieldName} format. Use hh:mm.");
        }

        return parsedTime;
    }

    public static void ValidateFutureDateTime(DateTime reservationDateTime)
    {
        var currentUtcTime = DateTime.UtcNow;
        if (reservationDateTime < currentUtcTime)
        {
            throw new ArgumentException("Reservation date and time must be in the future.");
        }
    }
}