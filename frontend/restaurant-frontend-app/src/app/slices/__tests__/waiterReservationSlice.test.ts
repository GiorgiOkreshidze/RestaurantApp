import { describe, it, expect, vi, beforeEach } from "vitest";
import { configureStore, EnhancedStore } from "@reduxjs/toolkit";
import {
  waiterReservationsReducer,
  initialState,
  setFormDateAction,
  setFormTimeAction,
  setFormTableAction,
  selectWaiterReservationsForm,
  WaiterReservationsState,
} from "../waiterReservationsSlice";
import { startOfToday } from "date-fns";
import { getLocationTables } from "@/app/thunks/locationsThunks";
import { getTimeSlots } from "@/app/thunks/bookingThunk";

vi.mock("date-fns", () => ({
  startOfToday: vi.fn().mockReturnValue(new Date("2025-04-12T00:00:00")),
}));

interface RootState {
  waiterReservations: WaiterReservationsState;
}

describe("waiterReservationsSlice", () => {
  let store: EnhancedStore<RootState>;

  beforeEach(() => {
    vi.clearAllMocks();

    store = configureStore({
      reducer: {
        waiterReservations: waiterReservationsReducer,
      },
    });
  });

  describe("initial state", () => {
    it("should have the correct initial state", () => {
      expect(store.getState().waiterReservations).toEqual(initialState);
    });

    it("should have today's date in the initial state", () => {
      expect(store.getState().waiterReservations.form.date).toBe(
        startOfToday().toString()
      );
    });
  });

  describe("reducers", () => {
    it("should handle setFormDateAction", () => {
      const date = "2025-04-15";
      store.dispatch(setFormDateAction(date));
      expect(store.getState().waiterReservations.form.date).toBe(date);
    });

    it("should handle setFormTimeAction", () => {
      const time = "18:00-19:00";
      store.dispatch(setFormTimeAction(time));
      expect(store.getState().waiterReservations.form.time).toBe(time);
    });

    it("should handle setFormTableAction", () => {
      const table = "table-123";
      store.dispatch(setFormTableAction(table));
      expect(store.getState().waiterReservations.form.table).toBe(table);
    });
  });

  describe("extraReducers", () => {
    describe("getTimeSlots", () => {
      it("should update timeList when getTimeSlots fulfilled", () => {
        const mockTimeSlots = [
          {
            rangeString: "12:00-13:00",
            isPast: false,
            startTime: "12:00",
            endTime: "13:00",
          },
          {
            rangeString: "14:00-15:00",
            isPast: false,
            startTime: "14:00",
            endTime: "15:00",
          },
        ];

        const fulfilledAction = {
          type: getTimeSlots.fulfilled.type,
          payload: mockTimeSlots,
        };

        store.dispatch(fulfilledAction);

        expect(store.getState().waiterReservations.form.timeList).toEqual(
          mockTimeSlots
        );
      });
    });

    describe("getLocationTables", () => {
      it("should update tableList when getLocationTables fulfilled", () => {
        const mockTables = [
          { id: "table-1", name: "Table 1", seats: 4 },
          { id: "table-2", name: "Table 2", seats: 2 },
        ];

        const fulfilledAction = {
          type: getLocationTables.fulfilled.type,
          payload: mockTables,
        };

        store.dispatch(fulfilledAction);

        expect(store.getState().waiterReservations.form.tableList).toEqual(
          mockTables
        );
      });
    });
  });

  describe("selectors", () => {
    it("should select waiter reservations form", () => {
      const emptyForm = store.getState().waiterReservations.form;
      const initialFormSelection = selectWaiterReservationsForm(
        store.getState()
      );
      expect(initialFormSelection).toEqual(emptyForm);

      const date = "2025-04-15";
      const time = "18:00-19:00";
      const table = "table-123";

      store.dispatch(setFormDateAction(date));
      store.dispatch(setFormTimeAction(time));
      store.dispatch(setFormTableAction(table));

      const updatedFormSelection = selectWaiterReservationsForm(
        store.getState()
      );
      expect(updatedFormSelection).toEqual({
        ...emptyForm,
        date,
        time,
        table,
      });
    });

    it("should reflect changes in form after thunks are fulfilled", () => {
      const emptyForm = store.getState().waiterReservations.form;

      const mockTimeSlots = [
        {
          rangeString: "12:00-13:00",
          isPast: false,
          startTime: "12:00",
          endTime: "13:00",
        },
      ];

      const mockTables = [{ id: "table-1", name: "Table 1", seats: 4 }];

      store.dispatch({
        type: getTimeSlots.fulfilled.type,
        payload: mockTimeSlots,
      });

      store.dispatch({
        type: getLocationTables.fulfilled.type,
        payload: mockTables,
      });

      const updatedFormSelection = selectWaiterReservationsForm(
        store.getState()
      );
      expect(updatedFormSelection).toEqual({
        ...emptyForm,
        timeList: mockTimeSlots,
        tableList: mockTables,
      });
    });
  });

  describe("combined actions", () => {
    it("should maintain state consistency when multiple actions are dispatched", () => {
      const date = "2025-04-15";
      const time = "18:00-19:00";
      const table = "table-123";

      store.dispatch(setFormDateAction(date));
      store.dispatch(setFormTimeAction(time));
      store.dispatch(setFormTableAction(table));

      const mockTimeSlots = [
        {
          rangeString: "12:00-13:00",
          isPast: false,
          startTime: "12:00",
          endTime: "13:00",
        },
      ];

      const mockTables = [{ id: "table-1", name: "Table 1", seats: 4 }];

      store.dispatch({
        type: getTimeSlots.fulfilled.type,
        payload: mockTimeSlots,
      });

      store.dispatch({
        type: getLocationTables.fulfilled.type,
        payload: mockTables,
      });

      const finalState = store.getState().waiterReservations.form;
      expect(finalState).toEqual({
        date,
        time,
        table,
        timeList: mockTimeSlots,
        tableList: mockTables,
      });

      const selectorResult = selectWaiterReservationsForm(store.getState());
      expect(selectorResult).toEqual(finalState);
    });
  });
});
