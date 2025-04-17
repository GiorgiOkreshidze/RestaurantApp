import { act, renderHook } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { useFilterState } from "../useFiltersState";

describe("useFilterState", () => {
  describe("state", () => {
    it("should initialize with default values", () => {
      const { result } = renderHook(() => useFilterState());
      expect(result.current.filters).toEqual({
        activeCategory: "",
        sortBy: "",
      });
    });
  });

  describe("handleCategoryToggle", () => {
    it("should set activeCategory, that is not active", () => {
      const { result } = renderHook(() => useFilterState());
      expect(result.current.filters.activeCategory).toBe("");
      act(() => result.current.handleCategoryToggle("Appetizers"));
      expect(result.current.filters.activeCategory).toBe("Appetizers");
    });
    it("should reset category, if it is active", () => {
      const { result } = renderHook(() => useFilterState());
      expect(result.current.filters.activeCategory).toBe("");
      act(() => result.current.handleCategoryToggle("Appetizers"));
      expect(result.current.filters.activeCategory).toBe("Appetizers");
      act(() => result.current.handleCategoryToggle("Appetizers"));
      expect(result.current.filters.activeCategory).toBe("");
    });
  });

  describe("isCategoryActive", () => {
    it("should return true, if passed category is active", () => {
      const { result } = renderHook(() => useFilterState());
      expect(result.current.filters.activeCategory).toBe("");
      expect(result.current.isCategoryActive("")).toBe(true);
      expect(result.current.isCategoryActive("Appetizers")).toBe(false);
      act(() => result.current.handleCategoryToggle("Appetizers"));
      expect(result.current.isCategoryActive("")).toBe(false);
      expect(result.current.isCategoryActive("Appetizers")).toBe(true);
    });
  });

  describe("setSort", () => {
    it("should set specified sort mode", () => {
      const { result } = renderHook(() => useFilterState());
      expect(result.current.filters.sortBy).toBe("");
      act(() => result.current.setSort("id"));
      expect(result.current.filters.sortBy).toBe("id");
    });
  });
});
