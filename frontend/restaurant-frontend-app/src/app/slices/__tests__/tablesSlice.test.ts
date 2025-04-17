import { describe, it, expect, vi, beforeEach } from "vitest";
import { configureStore, EnhancedStore } from "@reduxjs/toolkit";
import {
  tablesReducer,
  selectTables,
  selectTablesLoading,
} from "../tablesSlice";
import { getTables } from "@/app/thunks/tablesThunk";
import { RichTimeSlot } from "@/types";
import {
  parseTimeFromServer,
  parseDateFromServer,
} from "@/utils/dateTime";

vi.mock("date-fns", async () => {
  const actual = await vi.importActual("date-fns");
  return {
    ...actual,
    isPast: vi.fn().mockReturnValue(true) // Всегда возвращаем true в тестах
  };
});

// Тип для стора
interface RootState {
  tables: {
    tables: {
      tableId: string;
      tableNumber: string;
      availableSlots: RichTimeSlot[];
      date: string;
      capacity: string;
      locationAddress: string;
      locationId: string;
    }[];
    tablesLoading: boolean;
  };
}

describe("tablesSlice", () => {
  let store: EnhancedStore<RootState>;

  beforeEach(() => {
    vi.clearAllMocks();
    store = configureStore({
      reducer: {
        tables: tablesReducer,
      },
    });
  });

  describe("initial state", () => {
    it("should have the correct initial state", () => {
      expect(store.getState().tables.tables).toEqual([]);
      expect(store.getState().tables.tablesLoading).toBe(false);
    });
  });

  describe("selectors", () => {
    it("selectTables selector should return tables", () => {
      const state = {
        tables: {
          tables: [
            {
              tableId: "1",
              tableNumber: "1",
              availableSlots: [
                {
                  id: "10:00 - 11:00",
                  startString: "10:00",
                  endString: "11:00",
                  rangeString: "10:00 - 11:00",
                  startDate: parseTimeFromServer("10:00"),
                  endDate: parseTimeFromServer("11:00"),
                  isPast: false,
                } as RichTimeSlot,
              ],
              date: parseDateFromServer("2025-04-11"),
              capacity: "4",
              locationAddress: "123 Street",
              locationId: "loc1",
            },
          ],
          tablesLoading: false,
        },
      };

      expect(selectTables(state)).toEqual([
        {
          tableId: "1",
          tableNumber: "1",
          availableSlots: [
            {
              id: "10:00 - 11:00",
              startString: "10:00",
              endString: "11:00",
              rangeString: "10:00 - 11:00",
              startDate: parseTimeFromServer("10:00"),
              endDate: parseTimeFromServer("11:00"),
              isPast: false,
            } as RichTimeSlot,
          ],
          date: parseDateFromServer("2025-04-11"),
          capacity: "4",
          locationAddress: "123 Street",
          locationId: "loc1",
        },
      ]);
    });

    it("selectTablesLoading selector should return tablesLoading state", () => {
      const state = {
        tables: {
          tables: [],
          tablesLoading: true,
        },
      };
      expect(selectTablesLoading(state)).toBe(true);
    });
  });

  describe("extraReducers", () => {
    it("should handle getTables.pending", () => {
      store.dispatch({ type: getTables.pending.type });
      expect(store.getState().tables.tablesLoading).toBe(true);
    });

    it("should handle getTables.fulfilled", () => {
      const mockTables = [
        {
          tableId: "1",
          tableNumber: "1",
          availableSlots: [
            { start: "10:00", end: "11:00" },
            { start: "12:00", end: "13:00" },
          ],
          capacity: "4",
          locationAddress: "123 Street",
          locationId: "loc1",
        },
      ];

      store.dispatch({
        type: getTables.fulfilled.type,
        payload: {
          data: mockTables,
          date: "2025-04-11",
        },
      });

      const formattedSlots = [
        {
          id: "10:00 - 11:00",
          startString: "10:00",
          endString: "11:00",
          rangeString: "10:00 - 11:00",
          startDate: parseTimeFromServer("10:00"),
          endDate: parseTimeFromServer("11:00"),
          isPast: true,
        },
        {
          id: "12:00 - 13:00",
          startString: "12:00",
          endString: "13:00",
          rangeString: "12:00 - 13:00",
          startDate: parseTimeFromServer("12:00"),
          endDate: parseTimeFromServer("13:00"),
          isPast: true,
        },
      ];

      expect(store.getState().tables.tablesLoading).toBe(false);
      expect(store.getState().tables.tables).toEqual([
        {
          tableId: "1",
          tableNumber: "1",
          availableSlots: formattedSlots,
          date: parseDateFromServer("2025-04-11"),
          capacity: "4",
          locationAddress: "123 Street",
          locationId: "loc1",
        },
      ]);
    });

    it("should handle getTables.rejected", () => {
      store.dispatch({ type: getTables.rejected.type });
      expect(store.getState().tables.tablesLoading).toBe(false);
    });
  });
});
