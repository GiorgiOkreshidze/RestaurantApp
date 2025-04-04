import { selectTables, selectTablesLoading } from "@/app/slices/tablesSlice";
import {
  BookingForm,
  PageBody,
  PageBodyHeader,
  PageHero,
  TableCard,
} from "@/components/shared";
import { Spinner, Text } from "@/components/ui";
import { useSelector } from "react-redux";

export const Booking = () => {
  const tables = useSelector(selectTables);
  const tablesLoading = useSelector(selectTablesLoading);

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
        {tablesLoading ? (
          <Spinner />
        ) : (
          <ul className="grid gap-[2rem] lg:grid-cols-2">
            {tables
              ?.filter((table) => table.availableSlots.length > 0)
              .map((table) => (
                <TableCard key={table.tableId} table={table} />
              ))}
          </ul>
        )}
      </PageBody>
    </>
  );
};
