import { describe, it, expect, vi, beforeEach } from "vitest";
import { configureStore, EnhancedStore } from "@reduxjs/toolkit";
import {
  reservationsReducer,
  selectReservations,
  selectReservationsLoading,
  selectReservationCreatingLoading,
  selectReservationDeletingLoading,
  selectGiveReservationFeedbackLoading,
  initialState,
} from "../reservationsSlice";
import {
  getReservations,
  upsertClientReservation,
  deleteClientReservation,
  upsertWaiterReservation,
  giveReservationFeedback,
} from "@/app/thunks/reservationsThunks";
import { toast } from "react-toastify";
import { Reservation } from "@/types/reservation.types";

vi.mock("react-toastify", () => ({
  toast: {
    error: vi.fn(),
  },
}));

interface RootState {
  reservations: typeof initialState;
}

describe("reservationsSlice", () => {
  let store: EnhancedStore<RootState>;

  beforeEach(() => {
    vi.clearAllMocks();

    store = configureStore({
      reducer: {
        reservations: reservationsReducer,
      },
    });
  });

  describe("initial state", () => {
    it("should have the correct initial state", () => {
      expect(store.getState().reservations).toEqual(initialState);
    });
  });

  describe("thunks", () => {
    describe("getReservations", () => {
      it("should handle pending state", () => {
        const pendingAction = { type: getReservations.pending.type };
        store.dispatch(pendingAction);
        expect(store.getState().reservations.reservationsLoading).toBe(true);
      });

      it("should handle fulfilled state", () => {
        const mockReservations: Reservation[] = [
          {
            id: "1",
            date: "123",
            feedbackId: "123",
            guestsNumber: "2",
            locationAddress: "123",
            locationId: "123",
            preOrder: "1",
            status: "Reserved",
            tableId: "123",
            tableNumber: "123",
            timeFrom: "12:20",
            timeTo: "13:20",
            timeSlot: "12:20 - 13:20",
            userEmail: "123",
            userInfo: "123",
            createdAt: "123",
            editableTill: "123",
          },
          {
            id: "1",
            date: "123",
            feedbackId: "123",
            guestsNumber: "2",
            locationAddress: "123",
            locationId: "123",
            preOrder: "1",
            status: "Reserved",
            tableId: "123",
            tableNumber: "123",
            timeFrom: "12:20",
            timeTo: "13:20",
            timeSlot: "12:20 - 13:20",
            userEmail: "123",
            userInfo: "123",
            createdAt: "123",
            editableTill: "123",
          },
        ];

        const fulfilledAction = getReservations.fulfilled(
          mockReservations,
          "",
          { date: "2023" }
        );

        store.dispatch(fulfilledAction);

        expect(store.getState().reservations.reservationsLoading).toBe(false);
        expect(store.getState().reservations.reservations).toEqual(
          mockReservations
        );
      });

      it("should handle rejected state", () => {
        const rejectedAction = { type: getReservations.rejected.type };
        store.dispatch(rejectedAction);
        expect(store.getState().reservations.reservationsLoading).toBe(false);
      });
    });

    describe("upsertClientReservation", () => {
      it("should handle pending state", () => {
        const pendingAction = { type: upsertClientReservation.pending.type };
        store.dispatch(pendingAction);
        expect(store.getState().reservations.reservationCreatingLoading).toBe(
          true
        );
      });

      it("should handle fulfilled state", () => {
        const mockReservation = {
          id: "1",
          name: "New Reservation",
        };

        const fulfilledAction = {
          type: upsertClientReservation.fulfilled.type,
          payload: mockReservation,
        };

        store.dispatch(fulfilledAction);

        expect(store.getState().reservations.reservationCreatingLoading).toBe(
          false
        );
      });

      it("should handle rejected state and show error toast", () => {
        const errorMessage = "Failed to create reservation";
        const rejectedAction = {
          type: upsertClientReservation.rejected.type,
          payload: { message: errorMessage },
        };

        store.dispatch(rejectedAction);

        expect(store.getState().reservations.reservationCreatingLoading).toBe(
          false
        );
        expect(toast.error).toHaveBeenCalledWith(errorMessage);
      });
    });

    describe("deleteClientReservation", () => {
      it("should handle pending state", () => {
        const pendingAction = { type: deleteClientReservation.pending.type };
        store.dispatch(pendingAction);
        expect(store.getState().reservations.reservationDeletingLoading).toBe(
          true
        );
      });

      it("should handle fulfilled state", () => {
        const fulfilledAction = {
          type: deleteClientReservation.fulfilled.type,
        };
        store.dispatch(fulfilledAction);
        expect(store.getState().reservations.reservationDeletingLoading).toBe(
          false
        );
      });

      it("should handle rejected state", () => {
        const rejectedAction = { type: deleteClientReservation.rejected.type };
        store.dispatch(rejectedAction);
        expect(store.getState().reservations.reservationDeletingLoading).toBe(
          false
        );
      });
    });

    describe("upsertWaiterReservation", () => {
      it("should handle pending state", () => {
        const pendingAction = { type: upsertWaiterReservation.pending.type };
        store.dispatch(pendingAction);
        expect(store.getState().reservations.reservationCreatingLoading).toBe(
          true
        );
      });

      it("should handle fulfilled state", () => {
        const mockReservation = {
          id: "1",
          name: "Waiter Reservation",
        };

        const fulfilledAction = {
          type: upsertWaiterReservation.fulfilled.type,
          payload: mockReservation,
        };

        store.dispatch(fulfilledAction);

        expect(store.getState().reservations.reservationCreatingLoading).toBe(
          false
        );
      });

      it("should handle rejected state and show error toast", () => {
        const errorMessage = "Failed to create waiter reservation";
        const rejectedAction = {
          type: upsertWaiterReservation.rejected.type,
          payload: { message: errorMessage },
        };

        store.dispatch(rejectedAction);

        expect(store.getState().reservations.reservationCreatingLoading).toBe(
          false
        );
        expect(toast.error).toHaveBeenCalledWith(errorMessage);
      });
    });

    describe("giveReservationFeedback", () => {
      it("should handle pending state", () => {
        const pendingAction = { type: giveReservationFeedback.pending.type };
        store.dispatch(pendingAction);
        expect(
          store.getState().reservations.giveReservationFeedbackLoading
        ).toBe(true);
      });

      it("should handle fulfilled state", () => {
        const fulfilledAction = {
          type: giveReservationFeedback.fulfilled.type,
        };
        store.dispatch(fulfilledAction);
        expect(
          store.getState().reservations.giveReservationFeedbackLoading
        ).toBe(false);
      });

      it("should handle rejected state and show error toast", () => {
        const errorMessage = "Failed to give feedback";
        const rejectedAction = {
          type: giveReservationFeedback.rejected.type,
          payload: { message: errorMessage },
        };

        store.dispatch(rejectedAction);

        expect(
          store.getState().reservations.giveReservationFeedbackLoading
        ).toBe(false);
        expect(toast.error).toHaveBeenCalledWith(errorMessage);
      });
    });
  });

  describe("selectors", () => {
    it("should select reservations state correctly", () => {
      const state = store.getState();

      expect(selectReservations(state)).toEqual([]);
      expect(selectReservationsLoading(state)).toBe(false);
      expect(selectReservationCreatingLoading(state)).toBe(false);
      expect(selectReservationDeletingLoading(state)).toBe(false);
      expect(selectGiveReservationFeedbackLoading(state)).toBe(false);
    });

    it("should return updated state from selectors", () => {
      const mockReservations: Reservation[] = [
        {
          id: "1",
          date: "123",
          feedbackId: "123",
          guestsNumber: "2",
          locationAddress: "123",
          locationId: "123",
          preOrder: "1",
          status: "Reserved",
          tableId: "123",
          tableNumber: "123",
          timeFrom: "12:20",
          timeTo: "13:20",
          timeSlot: "12:20 - 13:20",
          userEmail: "123",
          userInfo: "123",
          createdAt: "123",
          editableTill: "123",
        },
        {
          id: "1",
          date: "123",
          feedbackId: "123",
          guestsNumber: "2",
          locationAddress: "123",
          locationId: "123",
          preOrder: "1",
          status: "Reserved",
          tableId: "123",
          tableNumber: "123",
          timeFrom: "12:20",
          timeTo: "13:20",
          timeSlot: "12:20 - 13:20",
          userEmail: "123",
          userInfo: "123",
          createdAt: "123",
          editableTill: "123",
        },
      ];

      store.dispatch({
        type: getReservations.fulfilled.type,
        payload: mockReservations,
      });

      const state = store.getState();

      expect(selectReservations(state)).toEqual(mockReservations);
    });
  });
});
