import { StarIcon, EmptyStar } from "../icons";
import { Text } from "./Text";

interface Props {
  name: string;
  date: string;
  rating: number;
  review: string;
  image: string;
}

export const ReviewsCard: React.FC<Props> = ({
  name,
  date,
  rating,
  review,
  image,
}) => {
  const stars = Array.from({ length: 5 }, (_, i) =>
    i < rating ? (
      <StarIcon className="w-5 h-5" key={i} />
    ) : (
      <EmptyStar className="w-5 h-5" key={i} />
    )
  );

  return (
    <div className="w-[316px] h-[324px] rounded-3xl shadow-[0_0_10px_4px_rgba(194,194,194,0.5)] p-6">
      <div className="flex items-center mb-6 ">
        <div id="avatar" className="mr-3">
          <img
            src={image}
            alt={name}
            className="w-[60px] h-[60px] rounded-full "
          />
        </div>

        <div className="flex flex-col gap-2">
          <Text variant="bodyBold">{name}</Text>
          <Text variant="caption">{date}</Text>
        </div>

        <div className="flex gap-1 items-center ml-auto">{stars}</div>
      </div>

      <div>
        <Text variant="blockTitle">{review}</Text>
      </div>
    </div>
  );
};
