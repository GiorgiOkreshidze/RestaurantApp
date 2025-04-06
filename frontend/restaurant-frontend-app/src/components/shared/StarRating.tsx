import { cn } from "@/lib/utils";
import { Star } from "../icons";
import { useState } from "react";

export const StarRating = (props: Props) => {
  const { total = 5, selected, onChange } = props;
  const [hovered, setHovered] = useState<number | null>(null);

  return (
    <p className="inline-flex " onMouseLeave={() => setHovered(null)}>
      {Array(total)
        .fill(null)
        .map((_, i) => (
          <Star
            key={i}
            className={cn(
              "size-[2rem] cursor-pointer active:animate-ping",
              i <= (hovered ?? selected - 1)
                ? "fill-orange-400 stroke-orange-400"
                : "fill-transparent stroke-neutral-200",
            )}
            onClick={() => onChange(i + 1)}
            onMouseEnter={() => setHovered(i)}
          />
        ))}
    </p>
  );
};

interface Props {
  selected: number;
  total?: number;
  onChange: (rating: number) => void;
}
