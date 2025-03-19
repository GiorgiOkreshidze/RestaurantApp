import { Container } from "./container";
import { DishCard, Text } from "../ui";
import { Dish } from "@/types";
import { Loader2 } from "lucide-react";

interface Props {
  title: string;
  isLoading: boolean;
  dishes: Dish[];
}

export const Dishes: React.FC<Props> = ({
  isLoading = false,
  title,
  dishes,
}) => {
  return (
    <div>
      <Container className="!py-[64px]">
        <Text variant="h2" className="mb-10">
          {title}
        </Text>
        {isLoading ? (
          <Loader2 className="size-[4rem]" />
        ) : (
          <div className="flex gap-8">
            {dishes.slice(0, 4).map((item, index) => (
              <DishCard
                key={index}
                name={item.name}
                price={item.price}
                weight={item.weight}
                imageUrl={item.imageUrl}
              />
            ))}
          </div>
        )}
      </Container>
    </div>
  );
};
