import { Text } from "../ui";
import { Container } from "./container";

import ReviewImage from "../../assets/images/rock.jpg";
import { Review } from "@/types";
import { ReviewsCard } from "../ui/ReviewsCard";

const reviewsData: Review[] = [
  {
    name: "Rock",
    date: "Aug 6, 2024",
    rating: 5,
    review:
      "Absolutely loved this restaurant! The outdoor terrace was perfect for a relaxing evening, and the menu had so many fresh, healthy options. I’m vegetarian, and it’s great to see so many plant-based dishes with authentic Georgian flavors. Definitely coming back soon!",
    image: ReviewImage,
  },
  {
    name: "Rock",
    date: "Aug 6, 2024",
    rating: 3,
    review:
      "Absolutely loved this restaurant! The outdoor terrace was perfect for a relaxing evening, and the menu had so many fresh, healthy options. I’m vegetarian, and it’s great to see so many plant-based dishes with authentic Georgian flavors. Definitely coming back soon!",
    image: ReviewImage,
  },
  {
    name: "Rock",
    date: "Aug 6, 2024",
    rating: 2,
    review:
      "Absolutely loved this restaurant! The outdoor terrace was perfect for a relaxing evening, and the menu had so many fresh, healthy options. I’m vegetarian, and it’s great to see so many plant-based dishes with authentic Georgian flavors. Definitely coming back soon!",
    image: ReviewImage,
  },
  {
    name: "Rock",
    date: "Aug 6, 2024",
    rating: 5,
    review:
      "Absolutely loved this restaurant! The outdoor terrace was perfect for a relaxing evening, and the menu had so many fresh, healthy options. I’m vegetarian, and it’s great to see so many plant-based dishes with authentic Georgian flavors. Definitely coming back soon!",
    image: ReviewImage,
  },
];

export const Reviews = () => {
  return (
    <div>
      <Container>
        <Text variant="h2" className="mb-10">
          Customer Reviews
        </Text>

        {/* Табы: Тут нужно будет импрувнуть эту часть */}
        <div className="flex items-center justify-between mb-6">
          <div className="flex gap-4 items-center">
            <button className="cursor-pointer">
              <Text
                variant="h3"
                className="text-green-200 border-b-2 border-green-200"
              >
                Service
              </Text>
            </button>
            <button className="cursor-pointer">
              <Text variant="h3">Cuisine experience</Text>
            </button>
          </div>
          <div id="sort" className="flex items-center gap-4">
            <Text variant="bodyBold">Sort by:</Text>
          </div>
        </div>

        <div className="flex gap-8">
          {reviewsData.map((review) => (
            <ReviewsCard
              key={review.name}
              name={review.name}
              date={review.date}
              rating={review.rating}
              review={review.review}
              image={review.image}
            />
          ))}
        </div>

        {/* Нужно сделать еще пагинацию */}
      </Container>
    </div>
  );
};
