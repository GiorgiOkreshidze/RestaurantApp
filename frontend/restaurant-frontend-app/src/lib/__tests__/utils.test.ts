import { describe, expect, it } from "vitest";
import { cn } from "../utils";

describe("utils", () => {
  it("should combine classes correctly", () => {
    const result = cn("p-1", "m-1");
    expect(result).toBe("p-1 m-1");
  })
})