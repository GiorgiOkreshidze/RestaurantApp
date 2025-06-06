import { Container, BrandTitle } from ".";
import { LocationIcon, StarIcon } from "../icons";
import { Button, Text } from "../ui";

import { useSelector } from "react-redux";
import { selectOneLocation } from "@/app/slices/locationsSlice";
import { useNavigate } from "react-router";
import { useAppDispatch } from "@/app/hooks";
import { setLocationAction } from "@/app/slices/bookingSlice";

export const LocationHero = () => {
  const oneLocation = useSelector(selectOneLocation);
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const bookTable = () => {
    dispatch(setLocationAction(oneLocation?.id));
    navigate("/booking");
  };

  return (
    <Container>
      <div className="flex gap-20 justify-between ">
        <div className="min-w-[340px] flex flex-col">
          <BrandTitle
            variant="navBarLogo"
            className="text-green-200 !text-5xl mb-6"
          />

          <div className="flex items-center justify-between mb-6">
            <div className="flex gap-2.5">
              <LocationIcon className="stroke-green-200" />
              <Text variant="bodyBold" className="">
                {oneLocation?.address}
              </Text>
            </div>
            <div className="flex gap-1 items-center">
              <Text>{oneLocation?.rating}</Text>
              <StarIcon />
            </div>
          </div>

          <div>
            <Text variant="blockTitle" className="mb-3">
              {oneLocation?.description}
            </Text>
          </div>

          <Button className="!mt-auto" onClick={bookTable}>
            Book a Table
          </Button>
        </div>
        <div
          className="w-full h-[500px] rounded-3xl bg-cover bg-center"
          style={{ backgroundImage: `url(${oneLocation?.imageUrl})` }}
        ></div>
      </div>
    </Container>
  );
};
