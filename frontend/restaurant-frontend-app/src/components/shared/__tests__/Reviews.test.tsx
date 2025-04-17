import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import * as reactRedux from "react-redux";
import * as appHooks from "@/app/hooks";
import * as locationsThunks from "@/app/thunks/locationsThunks";
import { Review } from "@/types";
import { Reviews } from "../Reviews";

vi.mock("react-redux");
vi.mock("@/app/hooks");
vi.mock("@/app/thunks/locationsThunks");
vi.mock("@/components/ui/ReviewsCard", () => ({
  ReviewsCard: ({ review }: { review: Review }) => (
    <div data-testid="review-card">{review.userName}</div>
  ),
}));

describe("Reviews Component", () => {
  const mockFeedbacks: Review[] = [
    {
      id: "1",
      userName: "John Doe",
      comment: "Great service!",
      rate: 5,
      date: "2023-01-15",
      locationId: "loc1",
      userAvatarUrl: "",
      type: "SERVICE_QUALITY",
    },
    {
      id: "2",
      userName: "Jane Smith",
      comment: "Excellent food!",
      rate: 4,
      date: "2023-02-20",
      locationId: "loc1",
      userAvatarUrl: "",
      type: "CUISINE_EXPERIENCE",
    },
  ];

  beforeEach(() => {
    vi.spyOn(reactRedux, "useSelector").mockReturnValue(false); 

    const dispatchMock = vi.fn();
    vi.spyOn(appHooks, "useAppDispatch").mockReturnValue(dispatchMock);

    const getFeedbacksMock = vi.fn();
    vi.spyOn(locationsThunks, "getFeedbacksOfLocation").mockImplementation(
      getFeedbacksMock
    );
  });

  it("should render the component title correctly", () => {
    render(<Reviews feedbacks={mockFeedbacks} id="loc1" />);
    expect(screen.getByText("Customer Reviews")).toBeInTheDocument();
  });

  it("should render review cards when feedbacks are provided", () => {
    render(<Reviews feedbacks={mockFeedbacks} id="loc1" />);

    expect(screen.getByText("John Doe")).toBeInTheDocument();
    expect(screen.getByText("Jane Smith")).toBeInTheDocument();
    expect(screen.getAllByTestId("review-card")).toHaveLength(2);
  });

  it('should display "No feedbacks found" message when no feedbacks are available', () => {
    render(<Reviews feedbacks={[]} id="loc1" />);

    expect(screen.getByText("No feedbacks found")).toBeInTheDocument();
    expect(
      screen.getByText("Try changing your filters or check back later.")
    ).toBeInTheDocument();
  });

  it("should show loader when loading is true", () => {
    vi.spyOn(reactRedux, "useSelector").mockReturnValue(true);

    render(<Reviews feedbacks={mockFeedbacks} id="loc1" />);

    expect(screen.getByTestId("loader")).toBeInTheDocument();
  });

  it("should switch between service types when tabs are clicked", async () => {
    const getFeedbacksMock = vi.fn();
    vi.spyOn(locationsThunks, "getFeedbacksOfLocation").mockImplementation(
      getFeedbacksMock
    );

    render(<Reviews feedbacks={mockFeedbacks} id="loc1" />);

    expect(screen.getByText("Service").className).toContain("text-green-200");

    fireEvent.click(screen.getByText("Cuisine experience"));

    await waitFor(() => {
      expect(locationsThunks.getFeedbacksOfLocation).toHaveBeenCalledWith({
        id: "loc1",
        type: "CUISINE_EXPERIENCE",
        sort: "rating,desc",
      });
    });

    expect(screen.getByText("Cuisine experience").className).toContain(
      "text-green-200"
    );
  });

  it("should render pagination with correct number of pages", () => {
    render(<Reviews feedbacks={Array(12).fill(mockFeedbacks[0])} id="loc1" />);

    const pagination = screen.getByTestId("pagination");
    expect(pagination).toBeInTheDocument();

    expect(screen.getByText("3")).toBeInTheDocument();
  });

  it("should fetch feedbacks on component mount", () => {
    render(<Reviews feedbacks={mockFeedbacks} id="loc1" />);

    expect(locationsThunks.getFeedbacksOfLocation).toHaveBeenCalledWith({
      id: "loc1",
      type: "SERVICE_QUALITY",
      sort: "rating,desc",
    });
  });

  it("should not fetch feedbacks when id is not provided", () => {
    render(<Reviews feedbacks={mockFeedbacks} />);

    expect(locationsThunks.getFeedbacksOfLocation).not.toHaveBeenCalled();
  });
});
