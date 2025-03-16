import React from "react";
import { Text } from "./Text";

interface Props {
  name: string;
  cost: string;
  weight: string;
  image: string;
}

export const DishCard: React.FC<Props> = ({ name, cost, weight, image }) => {
  return (
    <div className="w-[316px] h-[304px] p-6 rounded-3xl shadow-[0_0_10px_4px_rgba(194,194,194,0.5)] flex flex-col items-center transition-all duration-300 hover:scale-105 cursor-pointer">
      <img src={image} alt={name} className="w-[196px] h-[196px]" />
      <Text variant="bodyBold" className="w-full mt-4">
        {name}
      </Text>
      <div className="flex items-center justify-between w-full mb-1">
        <Text variant="caption" className="text-neutral-900">
          {cost} $
        </Text>
        <Text variant="caption" className="text-neutral-900">
          {weight} g
        </Text>
      </div>
    </div>
  );
};
