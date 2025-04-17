import { setupServer } from "msw/node";
import { afterAll, afterEach, beforeAll, describe, expect, it } from "vitest";
import { timeslotsHandlers } from "../endpoints/timeslots";
import { apiURLLocal, serverRoute } from "@/utils/constants";

const server = setupServer(...timeslotsHandlers);

describe("timeslots.ts", () => {
  beforeAll(() => server.listen());
  afterEach(() => server.resetHandlers());
  afterAll(() => server.close());

  it("should respond with status 200 and JSON data", async () => {
    const response = await fetch(`${apiURLLocal}/${serverRoute.timeSlots}`);
    expect(response.status).toBe(200);
    const data = await response.json();
    expect(data).toBeDefined();
  });
});
