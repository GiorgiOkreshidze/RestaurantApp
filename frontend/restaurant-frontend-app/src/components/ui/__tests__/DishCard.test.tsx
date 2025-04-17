import { render, screen, fireEvent } from "@testing-library/react";
import { describe, test, expect, vi } from "vitest";
import { Preorder } from "@/types/preorder.types";
import { DishCard } from "../DishCard";

// Моковые данные для тестов
const mockDishData = {
  id: "dish-123",
  name: "Pasta Carbonara",
  price: "12.99",
  weight: "350g",
  imageUrl: "/dish.png",
};

const mockActivePreorder: Preorder = {
  id: "preorder-123",
  status: "new",
  dishes: [],
  number: 1,
};

const mockActivePreorderWithDish: Preorder = {
  id: "preorder-123",
  status: "new",
  dishes: [{ id: "dish-123", count: 2 }],
  number: 1,
};

describe("DishCard Component", () => {
  test("renders basic dish information correctly", () => {
    render(<DishCard {...mockDishData} />);

    expect(screen.getByText("Pasta Carbonara")).toBeInTheDocument();
    expect(screen.getByText("12.99 $")).toBeInTheDocument();
    expect(screen.getByText("350g")).toBeInTheDocument();
    expect(screen.getByAltText("Pasta Carbonara")).toBeInTheDocument();
    expect(screen.getByTestId("dish-card")).toBeInTheDocument();
  });

  test("onClick handler is called when dish card is clicked", () => {
    const mockOnClick = vi.fn();
    render(<DishCard {...mockDishData} onClick={mockOnClick} />);

    fireEvent.click(screen.getByTestId("dish-card"));
    expect(mockOnClick).toHaveBeenCalledTimes(1);
  });

  test('renders "On Stop" badge when dish is disabled', () => {
    render(<DishCard {...mockDishData} state="On Stop" />);

    expect(screen.getByText("On Stop")).toBeInTheDocument();
    expect(screen.getByTestId("dish-card")).toHaveClass("opacity-50");
    expect(screen.getByTestId("dish-card")).toHaveClass("cursor-not-allowed");
    expect(screen.getByTestId("dish-card")).toHaveClass("pointer-events-none");
  });

  test("does not render pre-order button when activePreorder is not provided", () => {
    render(<DishCard {...mockDishData} />);

    expect(screen.queryByText("Pre-order")).not.toBeInTheDocument();
    expect(screen.queryByText("In Cart")).not.toBeInTheDocument();
  });

  test('renders "Pre-order" button when activePreorder is provided but dish is not in preorder', () => {
    render(
      <DishCard
        {...mockDishData}
        activePreorder={mockActivePreorder}
        onPreorderClick={vi.fn()}
      />
    );

    expect(screen.getByText("Pre-order")).toBeInTheDocument();
  });

  test('renders "In Cart" button when dish is in active preorder', () => {
    render(
      <DishCard
        {...mockDishData}
        activePreorder={mockActivePreorderWithDish}
        onPreorderClick={vi.fn()}
      />
    );

    expect(screen.getByText("In Cart")).toBeInTheDocument();
  });

  test("onPreorderClick handler is called with correct parameters", () => {
    const mockOnPreorderClick = vi.fn();
    render(
      <DishCard
        {...mockDishData}
        activePreorder={mockActivePreorder}
        onPreorderClick={mockOnPreorderClick}
      />
    );

    fireEvent.click(screen.getByText("Pre-order"));
    expect(mockOnPreorderClick).toHaveBeenCalledTimes(1);
    expect(mockOnPreorderClick).toHaveBeenCalledWith(false, "dish-123");
  });

  test("onPreorderClick handler is called with correct parameters when dish is in preorder", () => {
    const mockOnPreorderClick = vi.fn();
    render(
      <DishCard
        {...mockDishData}
        activePreorder={mockActivePreorderWithDish}
        onPreorderClick={mockOnPreorderClick}
      />
    );

    fireEvent.click(screen.getByText("In Cart"));
    expect(mockOnPreorderClick).toHaveBeenCalledTimes(1);
    expect(mockOnPreorderClick).toHaveBeenCalledWith(true, "dish-123");
  });

  test("click on preorder button does not trigger the main onClick handler", () => {
    const mockOnClick = vi.fn();
    const mockOnPreorderClick = vi.fn();

    render(
      <DishCard
        {...mockDishData}
        onClick={mockOnClick}
        activePreorder={mockActivePreorder}
        onPreorderClick={mockOnPreorderClick}
      />
    );

    fireEvent.click(screen.getByText("Pre-order"));
    expect(mockOnPreorderClick).toHaveBeenCalledTimes(1);
    expect(mockOnClick).not.toHaveBeenCalled();
  });

  test('preorder button is disabled when dish is "On Stop"', () => {
    render(
      <DishCard
        {...mockDishData}
        state="On Stop"
        activePreorder={mockActivePreorder}
        onPreorderClick={vi.fn()}
      />
    );

    expect(screen.getByText("Pre-order")).toBeDisabled();
  });

  test("does not call onPreorderClick when it is not provided", () => {
    render(<DishCard {...mockDishData} activePreorder={mockActivePreorder} />);

    fireEvent.click(screen.getByText("Pre-order"));
    // Test passes if no error is thrown
  });
});
