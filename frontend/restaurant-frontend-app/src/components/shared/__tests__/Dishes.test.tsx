import { Dish, ExtendedDish } from "@/types";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { Dishes } from "../Dishes";
import { render, screen, fireEvent } from "@testing-library/react";
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

describe("Dishes component", () => {
  const mockDishes: Dish[] = [
    {
      id: "1",
      name: "Pizza",
      price: "10",
      weight: "500",
      imageUrl: "pizza.jpg",
    },
    {
      id: "2",
      name: "Burger",
      price: "8",
      weight: "300",
      imageUrl: "burger.jpg",
    },
    {
      id: "3",
      name: "Pasta",
      price: "12",
      weight: "400",
      imageUrl: "pasta.jpg",
    },
    {
      id: "4",
      name: "Salad",
      price: "6",
      weight: "250",
      imageUrl: "salad.jpg",
    },
    {
      id: "5",
      name: "Extra item",
      price: "5",
      weight: "100",
      imageUrl: "extra.jpg",
    },
  ];

  const mockOneDish: ExtendedDish = {
    id: "1",
    name: "Pizza",
    price: "10",
    weight: "500",
    imageUrl: "pizza.jpg",
    description: "Delicious pizza",
    calories: "300",
    carbohydrates: "40",
    fats: "10",
    proteins: "15",
    vitamins: "A, B, C",
  };

  beforeEach(() => {
    vi.clearAllMocks();
    vi.spyOn(reactRedux, "useSelector").mockImplementation((selector) => {
      if (selector === selectOneDish) return mockOneDish;
      if (selector === selectOneDishLoading) return false;
      return null;
    });
    const mockDispatch = vi.fn().mockResolvedValue(undefined);
    vi.spyOn(appHooks, "useAppDispatch").mockReturnValue(mockDispatch);
    vi.spyOn(dishesThunks, "getOneDish").mockImplementation(vi.fn());
  });

  it("renders component with correct titles and dishes", () => {
    render(<Dishes dishes={mockDishes} title="Popular Dishes" />);
    expect(screen.getByText("Popular Dishes")).toBeInTheDocument();
    const dishNames = mockDishes.slice(0, 4).map((dish) => dish.name);
    dishNames.forEach((name) => {
      expect(screen.getByText(name)).toBeInTheDocument();
    });
    expect(screen.queryByText(mockDishes[4].name)).not.toBeInTheDocument();
  });

  it("limits displayed dishes to 4 even if more are provided", () => {
    render(<Dishes dishes={mockDishes} title="Popular Dishes" />);
    const allDishNames = screen.getAllByTestId("dish-card");
    expect(allDishNames).toHaveLength(4);
    expect(screen.queryByText(mockDishes[4].name)).not.toBeInTheDocument();
  });

  it("opens dialog and fetches dish details when dish card is clicked", async () => {
    vi.mock("../ui", async () => {
      const actual = await vi.importActual("../ui");
      return {
        ...actual,
        DishCard: ({
          name,
          price,
          weight,
          imageUrl,
          onClick,
        }: Omit<Dish, "id"> & { onClick: () => void }) => (
          <div data-testid="dish-card" onClick={onClick}>
            <div>{name}</div>
            <div>{price}</div>
            <div>{weight}</div>
            <img src={imageUrl} alt={name} />
          </div>
        ),
      };
    });

    const mockDispatch = vi.fn().mockResolvedValue(undefined);
    vi.spyOn(appHooks, "useAppDispatch").mockReturnValue(mockDispatch);

    render(<Dishes dishes={mockDishes} title="Popular Dishes" />);

    const dishCards = screen.getAllByTestId("dish-card");
    fireEvent.click(dishCards[0]);
    expect(screen.getByTestId("one-dish-dialog")).toBeInTheDocument();
    expect(mockDispatch).toHaveBeenCalledTimes(1);
    expect(dishesThunks.getOneDish).toHaveBeenCalledWith("1");
  });

  it("shows loader in dialog when dish is loading", () => {
    vi.spyOn(reactRedux, "useSelector").mockImplementation((selector) => {
      if (selector === selectOneDish) return mockOneDish;
      if (selector === selectOneDishLoading) return true;
      return null;
    });

    render(<Dishes dishes={mockDishes} title="Popular Dishes" />);

    const dishCards = screen.getAllByTestId("dish-card");
    fireEvent.click(dishCards[0]);
    expect(screen.getByTestId("loader")).toBeInTheDocument();
    expect(screen.queryByText(mockOneDish.description)).not.toBeInTheDocument();
  });
});
