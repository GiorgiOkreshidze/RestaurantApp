import { describe, it, expect, vi, beforeEach } from "vitest";
import { configureStore, EnhancedStore } from "@reduxjs/toolkit";
import {
  bookingReducer,
  initialState,
  setLocationAction,
  setDateAction,
  setTimeAction,
  increaseGuestsAction,
  decreaseGuestsAction,
} from "../bookingSlice";
import { startOfTomorrow } from "date-fns";
import { getTimeSlots } from "@/app/thunks/bookingThunk";

// Mock date-fns
vi.mock("date-fns", () => ({
  startOfTomorrow: vi.fn().mockReturnValue(new Date("2025-04-12T00:00:00")),
}));

// Define type for our store
interface RootState {
  booking: typeof initialState;
}

describe("bookingSlice", () => {
  let store: EnhancedStore<RootState>;

  beforeEach(() => {
    vi.clearAllMocks();

    store = configureStore({
      reducer: {
        booking: bookingReducer,
      },
    });
  });

  describe("initial state", () => {
    it("should have the correct initial state", () => {
      expect(store.getState().booking).toEqual(initialState);
    });
  });

  describe("reducers", () => {
    it("should handle setLocationAction", () => {
      const locationId = "location-123";
      store.dispatch(setLocationAction(locationId));
      expect(store.getState().booking.locationId).toBe(locationId);
    });

    it("should handle setDateAction", () => {
      const date = "2025-04-15T12:00:00";
      store.dispatch(setDateAction(date));
      expect(store.getState().booking.date).toBe(date);
    });

    it("should handle setTimeAction", () => {
      const time = "18:00-19:00";
      store.dispatch(setTimeAction(time));
      expect(store.getState().booking.time).toBe(time);
    });

    it("should handle increaseGuestsAction", () => {
      // Initial value is 2
      store.dispatch(increaseGuestsAction());
      expect(store.getState().booking.guests).toBe(3);
    });

    it("should not increase guests above 10", () => {
      // Set to 10 first
      for (let i = 0; i < 8; i++) {
        store.dispatch(increaseGuestsAction());
      }
      expect(store.getState().booking.guests).toBe(10);

      // Try to increase again
      store.dispatch(increaseGuestsAction());
      expect(store.getState().booking.guests).toBe(10);
    });

    it("should handle decreaseGuestsAction", () => {
      // Increase to 3 first
      store.dispatch(increaseGuestsAction());
      expect(store.getState().booking.guests).toBe(3);

      // Now decrease
      store.dispatch(decreaseGuestsAction());
      expect(store.getState().booking.guests).toBe(2);
    });

    it("should not decrease guests below 1", () => {
      // Decrease to 1
      store.dispatch(decreaseGuestsAction());
      expect(store.getState().booking.guests).toBe(1);

      // Try to decrease again
      store.dispatch(decreaseGuestsAction());
      expect(store.getState().booking.guests).toBe(1);
    });
  });

  describe("thunks", () => {
    describe("getTimeSlots", () => {
      it("should handle pending state", () => {
        const pendingAction = { type: getTimeSlots.pending.type };
        store.dispatch(pendingAction);
        expect(store.getState().booking.timeSlotsLoading).toBe(true);
      });

      it("should handle fulfilled state with available slots", () => {
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

        expect(store.getState().booking.timeSlotsLoading).toBe(false);
        expect(store.getState().booking.timeSlots).toEqual(mockTimeSlots);
        expect(store.getState().booking.time).toBe("12:00-13:00");
        expect(store.getState().booking.date).toBe(new Date().toString());
      });

      it("should handle fulfilled state with only past slots", () => {
        const mockTimeSlots = [
          {
            rangeString: "12:00-13:00",
            isPast: true,
            startTime: "12:00",
            endTime: "13:00",
          },
          {
            rangeString: "14:00-15:00",
            isPast: true,
            startTime: "14:00",
            endTime: "15:00",
          },
        ];

        const fulfilledAction = {
          type: getTimeSlots.fulfilled.type,
          payload: mockTimeSlots,
        };

        store.dispatch(fulfilledAction);

        expect(store.getState().booking.timeSlotsLoading).toBe(false);
        expect(store.getState().booking.timeSlots).toEqual(mockTimeSlots);
        expect(store.getState().booking.time).toBe("12:00-13:00");
        expect(store.getState().booking.date).toBe(
          startOfTomorrow().toString()
        );
      });

      it("should handle rejected state", () => {
        const rejectedAction = { type: getTimeSlots.rejected.type };
        store.dispatch(rejectedAction);
        expect(store.getState().booking.timeSlotsLoading).toBe(false);
      });
    });
  });

  describe("selectors", () => {
    it("should select booking", () => {
      // Change some state values
      const locationId = "location-123";
      const date = "2025-04-15T12:00:00";
      const time = "18:00-19:00";

      store.dispatch(setLocationAction(locationId));
      store.dispatch(setDateAction(date));
      store.dispatch(setTimeAction(time));

      const state = store.getState();
      expect(state.booking).toEqual({
        ...initialState,
        locationId,
        date,
        time,
      });
    });
  });
});
