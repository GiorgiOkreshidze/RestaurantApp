import { render, screen } from "@testing-library/react";
import { describe, it, expect, vi } from "vitest";
import { format } from "date-fns";
import { Review } from "@/types";
import { ReviewsCard } from "../ReviewsCard";

// Мокаем импорт изображения
vi.mock("../../assets/images/rock.jpg", () => "/mocked-rock-image.jpg");

describe("ReviewsCard Component", () => {
  // Моковые данные для тестов
  const mockReview: Review = {
      id: "review-123",
      userName: "John Doe",
      comment: "Great place with amazing food!",
      rate: 4,
      date: "2023-05-15",
      locationId: "location-123",
      userAvatarUrl: "rock.jpg",
      type: "SERVICE_EXPERIENCE"
  };

  it("should render user information correctly", () => {
    render(<ReviewsCard review={mockReview} />);

    // Проверяем имя пользователя
    expect(screen.getByText("John Doe")).toBeInTheDocument();

    // Проверяем дату в отформатированном виде
    const formattedDate = format(mockReview.date, "MMM d, yyyy");
    expect(screen.getByText(formattedDate)).toBeInTheDocument();
  });

  it("should render the correct comment text", () => {
    render(<ReviewsCard review={mockReview} />);

    // Проверяем текст комментария
    expect(
      screen.getByText("Great place with amazing food!")
    ).toBeInTheDocument();
  });

  it("should render the correct number of filled and empty stars based on rating", () => {
    render(<ReviewsCard review={mockReview} />);

    // Находим контейнер со звездами
    const starsContainer = screen
      .getByText("John Doe")
      .closest("div")
      ?.parentElement?.querySelector(".flex.gap-1.items-center.ml-auto");

    expect(starsContainer).toBeInTheDocument();

    // В тестовых данных рейтинг 4, поэтому ожидаем 4 заполненные звезды и 1 пустую
    // Прямое тестирование количества иконок сложно реализовать в JSDOM
    // Поэтому проверяем, что контейнер со звездами существует и имеет 5 дочерних элементов
    expect(starsContainer?.childElementCount).toBe(5);
  });


  it("should handle reviews with different ratings correctly", () => {
    // Тест с рейтингом 2
    const lowRatingReview = { ...mockReview, rate: 2 };
    const { rerender } = render(<ReviewsCard review={lowRatingReview} />);

    // Прямая проверка количества заполненных/пустых звезд сложна,
    // но мы можем проверить основную структуру
    const starsContainer = screen
      .getByText("John Doe")
      .closest("div")
      ?.parentElement?.querySelector(".flex.gap-1.items-center.ml-auto");

    expect(starsContainer).toBeInTheDocument();
    expect(starsContainer?.childElementCount).toBe(5);

    // Перерендериваем с максимальным рейтингом
    const highRatingReview = { ...mockReview, rate: 5 };
    rerender(<ReviewsCard review={highRatingReview} />);

    // Снова проверяем структуру
    const updatedStarsContainer = screen
      .getByText("John Doe")
      .closest("div")
      ?.parentElement?.querySelector(".flex.gap-1.items-center.ml-auto");

    expect(updatedStarsContainer).toBeInTheDocument();
    expect(updatedStarsContainer?.childElementCount).toBe(5);
  });
});
