import React from "react";
import { Text } from "./Text";
import { Location } from "../icons";

interface Props {
  name: string;
  capacity: number;
  occupancy: number;
  image: string;
}

export const LocationsCard: React.FC<Props> = ({
  name,
  capacity,
  occupancy,
  image,
}) => {
  return (
    <div className="w-[432px] h-[256px] flex flex-col rounded-2xl shadow-[0_0_10px_4px_rgba(194,194,194,0.5)] transition-all duration-300 hover:scale-105 cursor-pointer">
      <div
        className="w-full h-[140px] bg-cover bg-center rounded-t-2xl"
        style={{ backgroundImage: `url(${image})` }}
      ></div>
      <div className="p-6">
        <div className="flex gap-2.5">
          <Location className="stroke-green-200" />
          <Text variant="bodyBold" className="mb-5">
            {name}
          </Text>
        </div>
        <div className="flex items-center justify-between">
          <Text variant="bodyBold">
            Total capacity:&nbsp;&nbsp; {capacity} tables
          </Text>
          <Text variant="bodyBold">
            Average occupancy:&nbsp;&nbsp; {occupancy}%
          </Text>
        </div>
      </div>
    </div>
  );
};
