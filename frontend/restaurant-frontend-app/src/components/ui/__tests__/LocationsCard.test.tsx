import { render, screen, fireEvent } from "@testing-library/react";
import { describe, expect, vi, beforeEach, it } from "vitest";
import { Location } from "@/types/location.types";
import { LocationsCard } from "../LocationsCard";
import * as reactRouter from "react-router";

vi.mock("react-router", () => ({
  useNavigate: () => vi.fn().mockReturnValue(vi.fn()),
}));

const scrollToMock = vi.fn();
Object.defineProperty(window, "scrollTo", {
  value: scrollToMock,
  writable: true,
});

describe("LocationsCard Component", () => {
  const mockLocation: Location = {
    id: "location-123",
    address: "221B Baker Street, London",
    description: "Blabla",
    rating: "5",
    imageUrl: "/location-image.jpg",
    totalCapacity: "42",
    averageOccupancy: "75",
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders location information correctly", () => {
    render(<LocationsCard location={mockLocation} />);

    expect(screen.getByText("221B Baker Street, London")).toBeInTheDocument();
    expect(screen.getByText("Total capacity: 42 tables")).toBeInTheDocument();
    expect(screen.getByText("Average occupancy: 75%")).toBeInTheDocument();

    const imageContainer = screen.getByLabelText(
      "Location card for /location-image.jpg"
    );
    expect(imageContainer).toBeInTheDocument();
    expect(imageContainer.style.backgroundImage).toBe(
      "url(/location-image.jpg)"
    );
  });

  it("navigates to correct location detail page on click", () => {
    const navigateMock = vi.fn();
    vi.spyOn(reactRouter, "useNavigate").mockReturnValue(navigateMock);

    render(<LocationsCard location={mockLocation} />);

    const card = screen
      .getByText("221B Baker Street, London")
      .closest("div.flex.flex-col");
    fireEvent.click(card!);

    expect(navigateMock).toHaveBeenCalledWith("/locations/location-123");

    expect(scrollToMock).toHaveBeenCalledWith(0, 0);
  });

  it("has correct styling and visual properties", () => {
    render(<LocationsCard location={mockLocation} />);

    const container = screen
      .getByText("221B Baker Street, London")
      .closest("div.flex.flex-col");
    expect(container).toHaveClass("flex");
    expect(container).toHaveClass("flex-col");
    expect(container).toHaveClass("rounded-2xl");
    expect(container).toHaveClass(
      "shadow-[0_0_10px_4px_rgba(194,194,194,0.5)]"
    );
    expect(container).toHaveClass("transition-all");
    expect(container).toHaveClass("duration-300");
    expect(container).toHaveClass("hover:scale-105");
    expect(container).toHaveClass("cursor-pointer");
  });

});
