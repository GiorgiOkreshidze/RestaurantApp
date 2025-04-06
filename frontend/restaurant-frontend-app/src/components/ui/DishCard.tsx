import React from "react";
import { Text } from "./Text";
import { Badge } from ".";

interface Props {
  name: string;
  price: string;
  weight: string;
  imageUrl: string;
  state?: string;
  onClick?: () => void;
}

export const DishCard: React.FC<Props> = ({
  name,
  price,
  weight,
  imageUrl,
  state,
  onClick,
}) => {
  const isDisabled = state === "On Stop";

  return (
    <div
      className={` p-6 rounded-3xl shadow-[0_0_10px_4px_rgba(194,194,194,0.5)] flex flex-col items-center transition-all duration-300 cursor-pointer relative 
      ${
        isDisabled
          ? "opacity-50 cursor-not-allowed pointer-events-none hover:scale-100"
          : "hover:scale-105"
      }`}
      onClick={onClick}
    >
      {isDisabled && (
        <div className="absolute top-2 right-2">
          <Badge status={state} className="text-nowrap">
            {state}
          </Badge>
        </div>
      )}

      <img src={imageUrl} alt={name} className="w-[196px] h-[196px]" />
      <Text variant="bodyBold" className="w-full mt-4">
        {name}
      </Text>
      <div className="flex items-center justify-between w-full mb-1">
        <Text variant="caption" className="text-neutral-900">
          {price} $
        </Text>
        <Text variant="caption" className="text-neutral-900">
          {weight}
        </Text>
      </div>
    </div>
  );
};
