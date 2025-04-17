import { render, screen, fireEvent } from "@testing-library/react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import * as reactRedux from "react-redux";
import * as reactRouter from "react-router";
import * as bookingSlice from "@/app/slices/bookingSlice";
import * as appHooks from "@/app/hooks";
import { LocationHero } from "../LocationHero";
import { Location } from "@/types/location.types";

vi.mock("react-redux");
vi.mock("react-router");
vi.mock("@/app/hooks");
vi.mock("@/app/slices/bookingSlice");

describe("LocationHero Component", () => {
  const mockLocation: Location = {
    id: "location-123",
    address: "123 Main Street, City",
    rating: "4.8",
    description: "A nice restaurant with great food",
    imageUrl: "/location-image.jpg",
    totalCapacity: "42",
    averageOccupancy: "75",
  };

  beforeEach(() => {
    vi.spyOn(reactRedux, "useSelector").mockReturnValue(mockLocation);

    const navigateMock = vi.fn();
    vi.spyOn(reactRouter, "useNavigate").mockReturnValue(navigateMock);

    const dispatchMock = vi.fn();
    vi.spyOn(appHooks, "useAppDispatch").mockReturnValue(dispatchMock);

    const setLocationActionMock = vi.fn().mockReturnValue({
      type: "booking/setLocation",
      payload: "location-123",
    });
    vi.spyOn(bookingSlice, "setLocationAction").mockImplementation(
      setLocationActionMock
    );
  });

  it("should render location information correctly", () => {
    render(<LocationHero />);

    expect(screen.getByText("123 Main Street, City")).toBeInTheDocument();
    expect(screen.getByText("4.8")).toBeInTheDocument();
    expect(
      screen.getByText("A nice restaurant with great food")
    ).toBeInTheDocument();

    expect(screen.getByText("Book a Table")).toBeInTheDocument();
  });

  // it("should display location image as background", () => {
  //   render(<LocationHero />);

  //   const imageElement = screen
  //     .getByText("A nice restaurant with great food")
  //     .closest("div.flex.gap-20")
  //     ?.querySelector("div.w-full.h-\\[500px\\]");

  //   expect(imageElement).toBeDefined();
  //   expect((imageElement as HTMLElement)?.style.backgroundImage).toBe(
  //     "url(/location-image.jpg)"
  //   );
  // });

  it('should navigate to booking page when "Book a Table" button is clicked', () => {
    const navigateMock = reactRouter.useNavigate();
    const dispatchMock = appHooks.useAppDispatch();

    render(<LocationHero />);

    const bookButton = screen.getByText("Book a Table");
    fireEvent.click(bookButton);

    expect(bookingSlice.setLocationAction).toHaveBeenCalledWith("location-123");

    expect(dispatchMock).toHaveBeenCalled();

    expect(navigateMock).toHaveBeenCalledWith("/booking");
  });

  it("should display brand title", () => {
    render(<LocationHero />);

    const brandTitleElement = document.querySelector(
      ".text-green-200.\\!text-5xl.mb-6"
    );
    expect(brandTitleElement).not.toBeNull();
  });

  it("should render rating with star icon", () => {
    render(<LocationHero />);

    const ratingContainer = screen.getByText("4.8").closest("div.flex.gap-1");

    expect(ratingContainer).toBeInTheDocument();

    expect(ratingContainer?.children.length).toBeGreaterThan(1);
  });
});
