import { setupServer } from "msw/node";
import { afterAll, afterEach, beforeAll, describe, expect, it } from "vitest";
import { locationTables } from "../endpoints/location-tables";
import { apiURLLocal, serverRoute } from "@/utils/constants";

const server = setupServer(...locationTables);

describe("location-tables.ts", () => {
  beforeAll(() => server.listen());
  afterEach(() => server.resetHandlers());
  afterAll(() => server.close());

  it("should respond with status 200 and JSON data", async () => {
    const response = await fetch(`${apiURLLocal}/${serverRoute.locationTables}`);
    expect(response.status).toBe(200);
    const data = await response.json();
    expect(data).toBeDefined();
  });
});
