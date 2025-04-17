// AllDishes.test.tsx

import { render, screen, fireEvent } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { AllDishes } from "../AllDishes";
import { Dish } from "@/types";
import * as reactRedux from "react-redux";
import * as appHooks from "@/app/hooks";
import * as dishesThunks from "@/app/thunks/dishesThunks";
import { selectOneDish, selectOneDishLoading } from "@/app/slices/dishesSlice";

vi.mock("react-redux", async () => {
  const actual = await vi.importActual("react-redux");
  return {
    ...actual,
    useSelector: vi.fn(),
    useDispatch: vi.fn(),
  };
});

vi.mock("@/app/hooks", () => ({
  useAppDispatch: vi.fn(),
}));

vi.mock("@/app/thunks/dishesThunks", () => ({
  getOneDish: vi.fn(),
}));

describe("AllDishes component", () => {
  const mockDishes: Dish[] = [
    {
      id: "1",
      name: "Pizza",
      price: "10",
      weight: "500g",
      imageUrl: "pizza.jpg",
      state: "available",
    },
    {
      id: "2",
      name: "Burger",
      price: "8",
      weight: "300g",
      imageUrl: "burger.jpg",
      state: "available",
    },
    {
      id: "3",
      name: "Pasta",
      price: "12",
      weight: "400g",
      imageUrl: "pasta.jpg",
      state: "available",
    },
  ];

  beforeEach(() => {
    vi.clearAllMocks();
    vi.spyOn(reactRedux, "useSelector").mockImplementation((selector) => {
      if (selector === selectOneDish) return null;
      if (selector === selectOneDishLoading) return false;
      return null;
    });
    const mockDispatch = vi.fn().mockResolvedValue(undefined);
    vi.spyOn(appHooks, "useAppDispatch").mockReturnValue(mockDispatch);
    vi.spyOn(dishesThunks, "getOneDish").mockImplementation(vi.fn());
  });

  it("renders Loader when loading is true", () => {
    render(<AllDishes dishes={[]} loading={true} />);
    expect(screen.getByTestId("loader")).toBeInTheDocument();
  });

  it('renders "No dishes found" when dishes array is empty', () => {
    render(<AllDishes dishes={[]} loading={false} />);
    expect(screen.getByText("No dishes found")).toBeInTheDocument();
    expect(
      screen.getByText("Try changing your filters or check back later.")
    ).toBeInTheDocument();
  });

  it("renders list of DishCards", () => {
    render(<AllDishes dishes={mockDishes} loading={false} />);

    mockDishes.forEach((dish) => {
      expect(screen.getByText(dish.name)).toBeInTheDocument();
    });
  });

  it("dispatches getOneDish and opens dialog when DishCard clicked", () => {
    const mockDispatch = vi.fn().mockResolvedValue(undefined);
    vi.spyOn(appHooks, "useAppDispatch").mockReturnValue(mockDispatch);
    vi.spyOn(dishesThunks, "getOneDish").mockReturnValue({
      type: "GET_ONE_DISH",
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } as any);

    render(<AllDishes dishes={mockDishes} loading={false} />);

    const dishCards = screen.getAllByTestId("dish-card");
    fireEvent.click(dishCards[0]);

    expect(mockDispatch).toHaveBeenCalled();
    expect(dishesThunks.getOneDish).toHaveBeenCalledWith("1");

    expect(screen.getByTestId("one-dish-dialog")).toBeInTheDocument();
  });

  it("shows loader in dialog when dish details are loading", () => {
    vi.spyOn(reactRedux, "useSelector").mockImplementation((selector) => {
      if (selector === selectOneDish) return null;
      if (selector === selectOneDishLoading) return true;
      return null;
    });

    render(<AllDishes dishes={mockDishes} loading={false} />);

    const dishCards = screen.getAllByTestId("dish-card");
    fireEvent.click(dishCards[0]);

    expect(screen.getByTestId("loader")).toBeInTheDocument();
  });
});
