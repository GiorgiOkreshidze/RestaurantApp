import { describe, expect, it } from "vitest";
import {
  formatDateToServer,
  formatDateToUI,
  formatTimeToUI,
  parseDateFromServer,
  parseTimeFromServer,
} from "../dateTime";

describe.only("dateTime.ts", () => {
  describe("parseDateFromServer", () => {
    it("parses valid value", () => {
      const result = parseDateFromServer("2025-01-01");
      const resultDate = new Date(result);
      expect(resultDate.getFullYear()).toBe(2025);
      expect(resultDate.getMonth()).toBe(0);
      expect(resultDate.getDate()).toBe(1);
    });
    it("handles invalid value", () => {
      const result = parseDateFromServer("wrong");
      expect(result).toBe("Invalid Date");
    });
  });

  describe("parseTimeFromServer", () => {
    it("parses valid value", () => {
      const result = parseTimeFromServer("22:16");
      const resultTime = new Date(result);
      expect(resultTime.getHours()).toBe(22);
      expect(resultTime.getMinutes()).toBe(16);
    });
    it("handles invalid value", () => {
      const result = parseTimeFromServer("30:16");
      expect(result).toBe("Invalid Date");
    });
  });

  describe("formatDateToServer", () => {
    it("parses valid value", () => {
      const result = formatDateToServer(new Date("2025-01-01").toString());
      const resultDate = new Date(result);
      expect(resultDate.getFullYear()).toBe(2025);
      expect(resultDate.getMonth()).toBe(0);
      expect(resultDate.getDate()).toBe(1);
    });
    it("handles invalid value", () => {
      expect(() => formatDateToServer("wrong")).toThrow();
    });
  });

  describe("formatDateToUI", () => {
    it("parses valid value", () => {
      const result = formatDateToUI(new Date("2025-01-01").toString());
      expect(result).toBeTypeOf("string");
    });
    it("handles invalid value", () => {
      expect(() => formatDateToUI("wrong")).toThrow();
    });
  });

  describe("formatTimeToUI", () => {
    it("parses valid value", () => {
      const result = formatTimeToUI("22:40");
      expect(result).toBeTypeOf("string");
    });
    it("handles invalid value", () => {
      expect(() => formatTimeToUI("wrong")).toThrow();
    });
  });
});
