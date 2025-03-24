import { Container, Dishes, LocationHero, Reviews } from "@/components/shared";
import { Text } from "@/components/ui";
import { NavLink, useParams } from "react-router";
import { useAppDispatch } from "@/app/hooks";
import { useEffect } from "react";
import { getSpecialityDishes } from "@/app/thunks/locationsThunks";
import { useSelector } from "react-redux";
import {
  selectOneLocation,
  selectSpecialityDishes,
} from "@/app/slices/locationsSlice";

export const Location = () => {
  const { id } = useParams();
  const dispatch = useAppDispatch();
  const specialityDishes = useSelector(selectSpecialityDishes);
  const oneLocation = useSelector(selectOneLocation);

  useEffect(() => {
    if (id) {
      dispatch(getSpecialityDishes(id));
    }
  }, [id, dispatch]);

  return (
    <>
      {/* Breadcrumbs, пока что захардкоженный */}
      <Container>
        <div className="mb-8 flex items-center">
          <NavLink to={"/"}>
            <Text variant="caption"> Main Page &gt;</Text>
          </NavLink>
          <NavLink to={`locations/${id}`}>
            <Text variant="bodyBold" className="ml-2">
              {oneLocation?.address}
            </Text>
          </NavLink>
        </div>
      </Container>

      <LocationHero />

      <Dishes title="Specialty Dishes" dishes={specialityDishes} />

      <Reviews />
    </>
  );
};
