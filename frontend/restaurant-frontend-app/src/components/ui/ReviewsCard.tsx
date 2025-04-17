import { Review } from "@/types";
import { StarIcon, EmptyStar } from "../icons";
import { Text } from "./Text";
import rockImg from "../../assets/images/rock.jpg";
import { format } from "date-fns";

interface Props {
  review: Review;
}

export const ReviewsCard: React.FC<Props> = ({ review }) => {
  const stars = Array.from({ length: 5 }, (_, i) =>
    i < review.rate ? (
      <StarIcon className="w-5 h-5" key={i} />
    ) : (
      <EmptyStar className="w-5 h-5" key={i} />
    )
  );

  return (
    <div className="w-full h-[324px] rounded-3xl shadow-[0_0_10px_4px_rgba(194,194,194,0.5)] p-6">
      <div className="flex items-center mb-6 ">
        <div id="avatar" className="mr-3 w-[60px]">
          <img
            src={review.userAvatarUrl || rockImg}
            alt={review.userName}
            className="w-full rounded-full "
          />
        </div>

        <div className="flex flex-col gap-2">
          <Text variant="bodyBold">{review.userName}</Text>
          <Text variant="caption" className="text-neutral-400">
            {format(review.date, "MMM d, yyyy")}
          </Text>
        </div>

        <div className="flex gap-1 items-center ml-auto">{stars}</div>
      </div>

      <div>
        <Text variant="blockTitle">{review.comment}</Text>
      </div>
    </div>
  );
};
