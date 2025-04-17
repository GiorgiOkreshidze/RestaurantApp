import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

vi.mock("react-redux", () => ({
  useSelector: vi.fn(),
}));

import { useSelector } from "react-redux";
import { PublicRoute } from "../PublicRoute";
import { MemoryRouter } from "react-router";

describe("PublicRoute.tsx", () => {
  it("should render children if 'user' is null", () => {
    vi.mocked(useSelector).mockReturnValue(null);
    render(
      <MemoryRouter>
        <PublicRoute>
          <h1>Hello world</h1>
        </PublicRoute>
      </MemoryRouter>,
    );
    expect(screen.getByText("Hello world")).toBeInTheDocument();
  });

  it("should not render children if 'user' is truthy", () => {
    vi.mocked(useSelector).mockReturnValue({});
    render(
      <MemoryRouter>
        <PublicRoute>
          <h1>Hello world</h1>
        </PublicRoute>
      </MemoryRouter>,
    );
    expect(screen.queryByText("Hello world")).not.toBeInTheDocument();
  });
});
