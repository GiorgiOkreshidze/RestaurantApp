import { describe, it, expect, vi, beforeEach } from "vitest";
import { configureStore, EnhancedStore } from "@reduxjs/toolkit";
import { locationsReducer, initialState } from "../locationsSlice";
import {
  getLocations,
  getOneLocation,
  getSpecialityDishes,
  getFeedbacksOfLocation,
  getSelectOptions,
  getLocationTables,
} from "@/app/thunks/locationsThunks";

interface RootState {
  locations: typeof initialState;
}

describe("locationsSlice", () => {
  let store: EnhancedStore<RootState>;

  beforeEach(() => {
    vi.clearAllMocks();

    store = configureStore({
      reducer: {
        locations: locationsReducer,
      },
    });
  });

  describe("initial state", () => {
    it("should have the correct initial state", () => {
      expect(store.getState().locations).toEqual(initialState);
    });
  });

  describe("thunks", () => {
    describe("getLocations", () => {
      it("should handle pending state", () => {
        const pendingAction = { type: getLocations.pending.type };
        store.dispatch(pendingAction);
        expect(store.getState().locations.locationsLoading).toBe(true);
      });

      it("should handle fulfilled state", () => {
        const mockLocations = [
          { id: "1", name: "Location 1" },
          { id: "2", name: "Location 2" },
        ];
        const fulfilledAction = {
          type: getLocations.fulfilled.type,
          payload: mockLocations,
        };
        store.dispatch(fulfilledAction);

        expect(store.getState().locations.locationsLoading).toBe(false);
        expect(store.getState().locations.locations).toEqual(mockLocations);
      });

      it("should handle rejected state", () => {
        const rejectedAction = { type: getLocations.rejected.type };
        store.dispatch(rejectedAction);
        expect(store.getState().locations.locationsLoading).toBe(false);
      });
    });

    describe("getOneLocation", () => {
      it("should handle pending state", () => {
        const pendingAction = { type: getOneLocation.pending.type };
        store.dispatch(pendingAction);
        expect(store.getState().locations.locationsLoading).toBe(true);
      });

      it("should handle fulfilled state", () => {
        const mockLocation = { id: "1", name: "Location 1" };
        const fulfilledAction = {
          type: getOneLocation.fulfilled.type,
          payload: mockLocation,
        };
        store.dispatch(fulfilledAction);

        expect(store.getState().locations.locationsLoading).toBe(false);
        expect(store.getState().locations.oneLocation).toEqual(mockLocation);
      });

      it("should handle rejected state", () => {
        const rejectedAction = { type: getOneLocation.rejected.type };
        store.dispatch(rejectedAction);
        expect(store.getState().locations.locationsLoading).toBe(false);
      });
    });

    describe("getSpecialityDishes", () => {
      it("should handle pending state", () => {
        const pendingAction = { type: getSpecialityDishes.pending.type };
        store.dispatch(pendingAction);
        expect(store.getState().locations.locationsLoading).toBe(true);
      });

      it("should handle fulfilled state", () => {
        const mockDishes = [
          { id: "1", name: "Dish 1" },
          { id: "2", name: "Dish 2" },
        ];
        const fulfilledAction = {
          type: getSpecialityDishes.fulfilled.type,
          payload: mockDishes,
        };
        store.dispatch(fulfilledAction);

        expect(store.getState().locations.locationsLoading).toBe(false);
        expect(store.getState().locations.specialityDishes).toEqual(mockDishes);
      });

      it("should handle rejected state", () => {
        const rejectedAction = { type: getSpecialityDishes.rejected.type };
        store.dispatch(rejectedAction);
        expect(store.getState().locations.locationsLoading).toBe(false);
      });
    });

    describe("getFeedbacksOfLocation", () => {
      it("should handle pending state", () => {
        const pendingAction = { type: getFeedbacksOfLocation.pending.type };
        store.dispatch(pendingAction);
        expect(store.getState().locations.feedbacksLoading).toBe(true);
      });

      it("should handle fulfilled state", () => {
        const mockFeedbacks = {
          content: [
            { id: "1", rating: 5, comment: "Great!" },
            { id: "2", rating: 4, comment: "Good!" },
          ],
        };
        const fulfilledAction = {
          type: getFeedbacksOfLocation.fulfilled.type,
          payload: mockFeedbacks,
        };
        store.dispatch(fulfilledAction);

        expect(store.getState().locations.feedbacksLoading).toBe(false);
        expect(store.getState().locations.feedbacks).toEqual(
          mockFeedbacks.content
        );
      });

      it("should handle rejected state", () => {
        const rejectedAction = { type: getFeedbacksOfLocation.rejected.type };
        store.dispatch(rejectedAction);
        expect(store.getState().locations.feedbacksLoading).toBe(false);
      });
    });

    describe("getSelectOptions", () => {
      it("should handle pending state", () => {
        const pendingAction = { type: getSelectOptions.pending.type };
        store.dispatch(pendingAction);
        expect(store.getState().locations.selectOptionsLoading).toBe(true);
      });

      it("should handle fulfilled state", () => {
        const mockOptions = [
          { value: "1", label: "Option 1" },
          { value: "2", label: "Option 2" },
        ];
        const fulfilledAction = {
          type: getSelectOptions.fulfilled.type,
          payload: mockOptions,
        };
        store.dispatch(fulfilledAction);

        expect(store.getState().locations.selectOptionsLoading).toBe(false);
        expect(store.getState().locations.selectOptions).toEqual(mockOptions);
      });

      it("should handle rejected state", () => {
        const rejectedAction = { type: getSelectOptions.rejected.type };
        store.dispatch(rejectedAction);
        expect(store.getState().locations.selectOptionsLoading).toBe(false);
      });
    });

    describe("getLocationTables", () => {
      it("should handle pending state", () => {
        const pendingAction = { type: getLocationTables.pending.type };
        store.dispatch(pendingAction);
        expect(store.getState().locations.locationTablesLoading).toBe(true);
      });

      it("should handle fulfilled state", () => {
        const mockTables = [
          { id: "1", name: "Table 1" },
          { id: "2", name: "Table 2" },
        ];
        const fulfilledAction = {
          type: getLocationTables.fulfilled.type,
          payload: mockTables,
        };
        store.dispatch(fulfilledAction);

        expect(store.getState().locations.locationTablesLoading).toBe(false);
        expect(store.getState().locations.locationTables).toEqual(mockTables);
      });

      it("should handle rejected state", () => {
        const rejectedAction = { type: getLocationTables.rejected.type };
        store.dispatch(rejectedAction);
        expect(store.getState().locations.locationTablesLoading).toBe(false);
      });
    });
  });

  describe("selectors", () => {
    it("should select locations", () => {
      const mockLocations = [{ id: "1", name: "Location 1" }];
      store.dispatch({
        type: getLocations.fulfilled.type,
        payload: mockLocations,
      });

      const state = store.getState();
      expect(state.locations.locations).toEqual(mockLocations);
    });

    it("should select oneLocation", () => {
      const mockLocation = { id: "1", name: "Location 1" };
      store.dispatch({
        type: getOneLocation.fulfilled.type,
        payload: mockLocation,
      });

      const state = store.getState();
      expect(state.locations.oneLocation).toEqual(mockLocation);
    });

    it("should select specialityDishes", () => {
      const mockDishes = [{ id: "1", name: "Dish 1" }];
      store.dispatch({
        type: getSpecialityDishes.fulfilled.type,
        payload: mockDishes,
      });

      const state = store.getState();
      expect(state.locations.specialityDishes).toEqual(mockDishes);
    });

    it("should select feedbacks", () => {
      const mockFeedbacks = {
        content: [{ id: "1", rating: 5, comment: "Great!" }],
      };
      store.dispatch({
        type: getFeedbacksOfLocation.fulfilled.type,
        payload: mockFeedbacks,
      });

      const state = store.getState();
      expect(state.locations.feedbacks).toEqual(mockFeedbacks.content);
    });

    it("should select selectOptions", () => {
      const mockOptions = [{ value: "1", label: "Option 1" }];
      store.dispatch({
        type: getSelectOptions.fulfilled.type,
        payload: mockOptions,
      });

      const state = store.getState();
      expect(state.locations.selectOptions).toEqual(mockOptions);
    });

    it("should select locationTables", () => {
      const mockTables = [{ id: "1", name: "Table 1" }];
      store.dispatch({
        type: getLocationTables.fulfilled.type,
        payload: mockTables,
      });

      const state = store.getState();
      expect(state.locations.locationTables).toEqual(mockTables);
    });

    it("should select loading states", () => {
      store.dispatch({ type: getLocations.pending.type });
      store.dispatch({ type: getFeedbacksOfLocation.pending.type });
      store.dispatch({ type: getSelectOptions.pending.type });
      store.dispatch({ type: getLocationTables.pending.type });

      const state = store.getState();
      expect(state.locations.locationsLoading).toBe(true);
      expect(state.locations.feedbacksLoading).toBe(true);
      expect(state.locations.selectOptionsLoading).toBe(true);
      expect(state.locations.locationTablesLoading).toBe(true);
    });
  });
});
