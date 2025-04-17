import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { ProtectedRoute } from "../ProtectedRoute";

vi.mock("react-redux", () => ({
  useSelector: vi.fn(),
}));

import { useSelector } from "react-redux";
import { MemoryRouter } from "react-router";

describe("ProtectedRoute.tsx", () => {
  it("should render children when user is truthy", () => {
    vi.mocked(useSelector).mockReturnValue({});
    render(
      <ProtectedRoute>
        <h1>Hello world</h1>
      </ProtectedRoute>,
    );
    expect(screen.getByText("Hello world")).toBeInTheDocument();
  });

  it("should not render children when user is falsy", () => {
    vi.mocked(useSelector).mockReturnValue(null);
    render(
      <MemoryRouter>
        <ProtectedRoute>
          <h1>Hello world</h1>
        </ProtectedRoute>
      </MemoryRouter>,
    );
    expect(screen.queryByText("Hello world")).not.toBeInTheDocument();
  });
});
