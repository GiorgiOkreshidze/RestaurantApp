import { apiURLLocal } from "@/utils/constants";
import { http, HttpResponse } from "msw";
import { timeString24hToDateObj } from "@/utils/dateTime";
import { isPast } from "date-fns";

export const timeslotsHandlers = [
  http.get(`${apiURLLocal}/timeslots`, () => {
    return HttpResponse.json(timeslots);
  }),
];

const timeslots = [
  {
    id: "06:30 - 08:00",
    startString: "06:30",
    endString: "08:00",
    rangeString: "06:30 - 08:00",
    startDate: timeString24hToDateObj("06:30"),
    endDate: timeString24hToDateObj("08:00"),
    isPast: isPast(timeString24hToDateObj("06:30")),
  },
  {
    id: "08:15 - 09:45",
    startString: "08:15",
    endString: "09:45",
    rangeString: "08:15 - 09:45",
    startDate: timeString24hToDateObj("08:15"),
    endDate: timeString24hToDateObj("09:45"),
    isPast: isPast(timeString24hToDateObj("08:15")),
  },
  {
    id: "10:00 - 11:30",
    startString: "10:00",
    endString: "11:30",
    rangeString: "10:00 - 11:30",
    startDate: timeString24hToDateObj("10:00"),
    endDate: timeString24hToDateObj("11:30"),
    isPast: isPast(timeString24hToDateObj("10:00")),
  },
  {
    id: "11:45 - 13:15",
    startString: "11:45",
    endString: "13:15",
    rangeString: "11:45 - 13:15",
    startDate: timeString24hToDateObj("11:45"),
    endDate: timeString24hToDateObj("13:15"),
    isPast: isPast(timeString24hToDateObj("11:45")),
  },
  {
    id: "13:30 - 15:00",
    startString: "13:30",
    endString: "15:00",
    rangeString: "13:30 - 15:00",
    startDate: timeString24hToDateObj("13:30"),
    endDate: timeString24hToDateObj("15:00"),
    isPast: isPast(timeString24hToDateObj("13:30")),
  },
  {
    id: "15:15 - 16:45",
    startString: "15:15",
    endString: "16:45",
    rangeString: "15:15 - 16:45",
    startDate: timeString24hToDateObj("15:15"),
    endDate: timeString24hToDateObj("16:45"),
    isPast: isPast(timeString24hToDateObj("15:15")),
  },
  {
    id: "17:00 - 18:30",
    startString: "17:00",
    endString: "18:30",
    rangeString: "17:00 - 18:30",
    startDate: timeString24hToDateObj("17:00"),
    endDate: timeString24hToDateObj("18:30"),
    isPast: isPast(timeString24hToDateObj("17:00")),
  },
];
