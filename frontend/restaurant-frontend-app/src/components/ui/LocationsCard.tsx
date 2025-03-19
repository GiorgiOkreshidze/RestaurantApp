import React from "react";
import { Text } from "./Text";
import { LocationIcon } from "../icons";
import locationImg from "../../assets/images/location.jpg";
import { useNavigate } from "react-router";
import { useAppDispatch } from "@/app/hooks";
import { setOneLocation } from "@/app/slices/locationsSlice";
import { Location } from "@/types";

interface Props {
  location: Location;
}

export const LocationsCard: React.FC<Props> = ({ location }) => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const onClick = () => {
    dispatch(setOneLocation(location));
    navigate(`/locations/${location.id}`);
    window.scrollTo(0, 0);
  };

  return (
    <div
      className="flex flex-col rounded-2xl shadow-[0_0_10px_4px_rgba(194,194,194,0.5)] transition-all duration-300 hover:scale-105 cursor-pointer"
      onClick={onClick}
    >
      <div
        className="w-full h-[140px] bg-cover bg-center rounded-t-2xl"
        style={{ backgroundImage: `url(${locationImg})` }}
        aria-label={`Location card for ${location.imageUrl}`}
      ></div>
      <div className="p-6">
        <div className="flex gap-2.5">
          <LocationIcon className="stroke-green-200" />
          <Text variant="bodyBold" className="mb-5">
            {location.address}
          </Text>
        </div>
        <div className="flex items-center justify-between">
          <Text variant="bodyBold">
            Total capacity: {location.totalCapacity} tables
          </Text>
          <Text variant="bodyBold">
            Average occupancy: {location.averageOccupancy}%
          </Text>
        </div>
      </div>
    </div>
  );
};
