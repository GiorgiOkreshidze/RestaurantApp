import { selectTables } from "@/app/slices/bookingSlice";
import {
  TableCard,
  BookingForm,
  PageBody,
  PageBodyHeader,
  PageHero,
} from "@/components/shared";
import { Text } from "@/components/ui";
import { useBookingForm } from "@/hooks/useBookingForm";
import { useSelector } from "react-redux";

export const Booking = () => {
  const tables = useSelector(selectTables);
  const { date } = useBookingForm();

  return (
    <>
      <PageHero variant="dark" className="flex flex-col justify-center">
        <Text variant="h2" className="text-primary">
          Green & Tasty Restaurants
        </Text>
        <Text variant="h1" tag="h1" className="text-primary mt-[1.375rem]">
          Book a Table
        </Text>
        <BookingForm className="mt-[2.5rem]" />
      </PageHero>
      <PageBody>
        <PageBodyHeader>
          <Text variant="bodyBold">{tables.length} tables available</Text>
        </PageBodyHeader>
        <ul className="grid gap-[2rem] lg:grid-cols-2">
          {tables?.map((table, i) => (
            <TableCard key={i} date={date} table={table} />
          ))}
        </ul>
      </PageBody>
    </>
  );
};

// const tables = [
//   {
//     availableSlots: [
//       {
//         start: "06:30",
//         end: "08:00",
//       },
//       {
//         start: "08:15",
//         end: "09:45",
//       },
//       {
//         start: "10:00",
//         end: "11:30",
//       },
//       {
//         start: "11:45",
//         end: "13:15",
//       },
//       {
//         start: "13:30",
//         end: "15:00",
//       },
//       {
//         start: "15:15",
//         end: "16:45",
//       },
//       {
//         start: "17:00",
//         end: "18:30",
//       },
//     ],
//     capacity: "4",
//     locationAddress: "9 Abashidze Street",
//     locationId: "3a88c365-970b-4a7a-a206-bc5282b9b25f",
//     tableId: "d7561080-cc9b-497d-ad38-88f264efbbfc",
//     tableNumber: "2",
//   },
//   {
//     availableSlots: [
//       {
//         start: "06:30",
//         end: "08:00",
//       },
//       {
//         start: "08:15",
//         end: "09:45",
//       },
//       {
//         start: "10:00",
//         end: "11:30",
//       },
//       {
//         start: "11:45",
//         end: "13:15",
//       },
//       {
//         start: "13:30",
//         end: "15:00",
//       },
//       {
//         start: "15:15",
//         end: "16:45",
//       },
//       {
//         start: "17:00",
//         end: "18:30",
//       },
//     ],
//     capacity: "6",
//     locationAddress: "9 Abashidze Street",
//     locationId: "3a88c365-970b-4a7a-a206-bc5282b9b25f",
//     tableId: "42f7291d-0997-4e74-8740-880533094881",
//     tableNumber: "1",
//   },
//   {
//     availableSlots: [
//       {
//         start: "06:30",
//         end: "08:00",
//       },
//       {
//         start: "08:15",
//         end: "09:45",
//       },
//       {
//         start: "10:00",
//         end: "11:30",
//       },
//       {
//         start: "11:45",
//         end: "13:15",
//       },
//       {
//         start: "13:30",
//         end: "15:00",
//       },
//       {
//         start: "15:15",
//         end: "16:45",
//       },
//       {
//         start: "17:00",
//         end: "18:30",
//       },
//     ],
//     capacity: "10",
//     locationAddress: "9 Abashidze Street",
//     locationId: "3a88c365-970b-4a7a-a206-bc5282b9b25f",
//     tableId: "604acc18-d0f6-4d0d-af5c-b14a9aeaa559",
//     tableNumber: "4",
//   },
//   {
//     availableSlots: [
//       {
//         start: "06:30",
//         end: "08:00",
//       },
//       {
//         start: "08:15",
//         end: "09:45",
//       },
//       {
//         start: "10:00",
//         end: "11:30",
//       },
//       {
//         start: "11:45",
//         end: "13:15",
//       },
//       {
//         start: "13:30",
//         end: "15:00",
//       },
//       {
//         start: "15:15",
//         end: "16:45",
//       },
//       {
//         start: "17:00",
//         end: "18:30",
//       },
//     ],
//     capacity: "10",
//     locationAddress: "9 Abashidze Street",
//     locationId: "3a88c365-970b-4a7a-a206-bc5282b9b25f",
//     tableId: "146bc291-2926-4a0b-90a2-a54975d2cbba",
//     tableNumber: "3",
//   },
// ];
