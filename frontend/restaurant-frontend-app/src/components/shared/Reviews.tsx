import { Review, SortOptionType } from "@/types";
import { useEffect, useMemo, useState } from "react";
import { Text } from "../ui";
import { Container } from "./Container";
import { ReviewsCard } from "@/components/ui/ReviewsCard";
import { useAppDispatch } from "@/app/hooks";
import { getFeedbacksOfLocation } from "@/app/thunks/locationsThunks";
import { SortingOptions } from "./SortingOptions";
import { CustomPagination } from "./CustomPagination";
import { useSelector } from "react-redux";
import { selectFeedbacksLoading } from "@/app/slices/locationsSlice";
import { Loader } from "./Loader";

interface Props {
  feedbacks: Review[];
  id?: string;
}

export const Reviews: React.FC<Props> = ({ feedbacks, id }) => {
  const dispatch = useAppDispatch();
  const loading = useSelector(selectFeedbacksLoading);
  const [activeType, setActiveType] = useState<
    "SERVICE_QUALITY" | "CUISINE_EXPERIENCE"
  >("SERVICE_QUALITY");
  const [sortBy, setSortBy] = useState("rating,desc");
  const [page, setPage] = useState(1);

  useEffect(() => {
    if (id) {
      dispatch(getFeedbacksOfLocation({ id, type: activeType, sort: sortBy }));
    }
  }, [dispatch, id, activeType, sortBy]);

  const sortOptions: SortOptionType[] = useMemo(
    () => [
      { id: "rating,desc", label: "Top rated first" },
      { id: "rating,asc", label: "Low rated first" },
      { id: "date,desc", label: "Newest first" },
      { id: "date,asc", label: "Oldest first" },
    ],
    []
  );

  const renderContent = () => {
    if (loading) {
      return (
        <div className="block h-[324px] mx-auto">
          <Loader />;
        </div>
      );
    }

    if (feedbacks.length === 0) {
      return (
        <div className="h-[324px] flex flex-col items-center justify-center">
          <Text variant="h3" className="mb-2">
            No feedbacks found
          </Text>
          <Text variant="bodyBold">
            Try changing your filters or check back later.
          </Text>
        </div>
      );
    }

    return (
      <div className="grid gap-6 grid-cols-[repeat(auto-fill,minmax(316px,1fr))]">
        {feedbacks.map((review) => (
          <ReviewsCard key={review.id} review={review} />
        ))}
      </div>
    );
  };

  return (
    <div>
      <Container>
        <Text variant="h2" className="mb-10">
          Customer Reviews
        </Text>

        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-6 gap-4 sm:gap-0">
          <div className="flex gap-4 items-center">
            <button
              className="cursor-pointer"
              onClick={() => setActiveType("SERVICE_QUALITY")}
            >
              <Text
                variant="h3"
                className={
                  activeType === "SERVICE_QUALITY"
                    ? "text-green-200 border-b-2 border-green-200"
                    : "border-b-2 border-transparent"
                }
              >
                Service
              </Text>
            </button>
            <button
              className="cursor-pointer"
              onClick={() => setActiveType("CUISINE_EXPERIENCE")}
            >
              <Text
                variant="h3"
                className={
                  activeType === "CUISINE_EXPERIENCE"
                    ? "text-green-200 border-b-2 border-green-200"
                    : "border-b-2 border-transparent"
                }
              >
                Cuisine experience
              </Text>
            </button>
          </div>

          <SortingOptions
            options={sortOptions}
            value={sortBy}
            onChange={setSortBy}
          />
        </div>

        {renderContent()}

        <CustomPagination
          currentPage={page}
          totalPages={Math.round(feedbacks.length / 4)}
          onPageChange={setPage}
        />
      </Container>
    </div>
  );
};
