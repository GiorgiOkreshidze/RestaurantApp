import { apiURL } from "@/utils/constants";
import { parseTimeFromServer } from "@/utils/dateTime";
import { isPast } from "date-fns";
import { http, HttpResponse } from "msw";

export const handlers = [
  http.get(`${apiURL}/timeslots`, () => {
    return HttpResponse.json([
      {
        id: "06:30 - 08:00",
        startString: "06:30",
        endString: "08:00",
        rangeString: "06:30 - 08:00",
        startDate: parseTimeFromServer("06:30"),
        endDate: parseTimeFromServer("08:00"),
        isPast: isPast(parseTimeFromServer("06:30")),
      },
      {
        id: "08:15 - 09:45",
        startString: "08:15",
        endString: "09:45",
        rangeString: "08:15 - 09:45",
        startDate: parseTimeFromServer("08:15"),
        endDate: parseTimeFromServer("09:45"),
        isPast: isPast(parseTimeFromServer("08:15")),
      },
      {
        id: "10:00 - 11:30",
        startString: "10:00",
        endString: "11:30",
        rangeString: "10:00 - 11:30",
        startDate: parseTimeFromServer("10:00"),
        endDate: parseTimeFromServer("11:30"),
        isPast: isPast(parseTimeFromServer("10:00")),
      },
      {
        id: "11:45 - 13:15",
        startString: "11:45",
        endString: "13:15",
        rangeString: "11:45 - 13:15",
        startDate: parseTimeFromServer("11:45"),
        endDate: parseTimeFromServer("13:15"),
        isPast: isPast(parseTimeFromServer("11:45")),
      },
      {
        id: "13:30 - 15:00",
        startString: "13:30",
        endString: "15:00",
        rangeString: "13:30 - 15:00",
        startDate: parseTimeFromServer("13:30"),
        endDate: parseTimeFromServer("15:00"),
        isPast: isPast(parseTimeFromServer("13:30")),
      },
      {
        id: "15:15 - 16:45",
        startString: "15:15",
        endString: "16:45",
        rangeString: "15:15 - 16:45",
        startDate: parseTimeFromServer("15:15"),
        endDate: parseTimeFromServer("16:45"),
        isPast: isPast(parseTimeFromServer("15:15")),
      },
      {
        id: "17:00 - 18:30",
        startString: "17:00",
        endString: "18:30",
        rangeString: "17:00 - 18:30",
        startDate: parseTimeFromServer("17:00"),
        endDate: parseTimeFromServer("18:30"),
        isPast: isPast(parseTimeFromServer("17:00")),
      },
    ]);
  }),

  http.get(`${apiURL}/location-tables`, () => {
    return HttpResponse.json([
      {
        tableId: "04ba5b37-8fbd-4f5f-8354-0b75078a790a",
        tableNumber: "1",
        capacity: "4",
        locationId: "8c4fc44e-c1a5-42eb-9912-55aeb5111a99",
        locationAddress: "48 Rustaveli Avenue",
      },
      {
        tableId: "dc80f954-df0c-457d-a938-26671f0dad47",
        tableNumber: "2",
        capacity: "6",
        locationId: "8c4fc44e-c1a5-42eb-9912-55aeb5111a99",
        locationAddress: "48 Rustaveli Avenue",
      },
      {
        tableId: "075312b0-6267-4dc6-ae96-28597231964e",
        tableNumber: "3",
        capacity: "2",
        locationId: "8c4fc44e-c1a5-42eb-9912-55aeb5111a99",
        locationAddress: "48 Rustaveli Avenue",
      },
      {
        tableId: "84405cfd-c7f5-4f67-9eef-496485065b1f",
        tableNumber: "4",
        capacity: "10",
        locationId: "8c4fc44e-c1a5-42eb-9912-55aeb5111a99",
        locationAddress: "48 Rustaveli Avenue",
      },
      {
        tableId: "c25f24b6-f305-4cd7-a6b6-54f126794c78",
        tableNumber: "1",
        capacity: "2",
        locationId: "e1fcb3b4-bf68-4bcb-b9ba-eac917dafac7",
        locationAddress: "14 Baratashvili Street",
      },
      {
        tableId: "9f9e2205-dc13-438b-802e-5824ad1889eb",
        tableNumber: "2",
        capacity: "4",
        locationId: "e1fcb3b4-bf68-4bcb-b9ba-eac917dafac7",
        locationAddress: "14 Baratashvili Street",
      },
      {
        tableId: "07095e7b-5cc9-40d2-a1d8-65aecdf5a2d9",
        tableNumber: "3",
        capacity: "6",
        locationId: "e1fcb3b4-bf68-4bcb-b9ba-eac917dafac7",
        locationAddress: "14 Baratashvili Street",
      },
      {
        tableId: "b8801e52-ba42-463e-b5cd-68c4604e223e",
        tableNumber: "4",
        capacity: "10",
        locationId: "e1fcb3b4-bf68-4bcb-b9ba-eac917dafac7",
        locationAddress: "14 Baratashvili Street",
      },
      {
        tableId: "42f7291d-0997-4e74-8740-880533094881",
        tableNumber: "1",
        capacity: "6",
        locationId: "3a88c365-970b-4a7a-a206-bc5282b9b25f",
        locationAddress: "9 Abashidze Street",
      },
      {
        tableId: "d7561080-cc9b-497d-ad38-88f264efbbfc",
        tableNumber: "2",
        capacity: "4",
        locationId: "3a88c365-970b-4a7a-a206-bc5282b9b25f",
        locationAddress: "9 Abashidze Street",
      },
      {
        tableId: "146bc291-2926-4a0b-90a2-a54975d2cbba",
        tableNumber: "3",
        capacity: "10",
        locationId: "3a88c365-970b-4a7a-a206-bc5282b9b25f",
        locationAddress: "9 Abashidze Street",
      },
      {
        tableId: "604acc18-d0f6-4d0d-af5c-b14a9aeaa559",
        tableNumber: "4",
        capacity: "10",
        locationId: "3a88c365-970b-4a7a-a206-bc5282b9b25f",
        locationAddress: "9 Abashidze Street",
      },
    ]);
  }),
];
