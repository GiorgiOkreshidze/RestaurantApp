import {
  Container,
  Dishes,
  LocationHero,
  PageBody,
  Reviews,
} from "@/components/shared";
import { Text } from "@/components/ui";
import { NavLink, useParams } from "react-router";
import { useAppDispatch } from "@/app/hooks";
import { useEffect } from "react";
import {
  getOneLocation,
  getSpecialityDishes,
} from "@/app/thunks/locationsThunks";
import { useSelector } from "react-redux";
import {
  selectFeedbacks,
  selectOneLocation,
  selectSpecialityDishes,
} from "@/app/slices/locationsSlice";

export const Location = () => {
  const { id } = useParams();
  const dispatch = useAppDispatch();
  const specialityDishes = useSelector(selectSpecialityDishes);
  const feedbacks = useSelector(selectFeedbacks);
  const oneLocation = useSelector(selectOneLocation);

  useEffect(() => {
    if (id) {
      dispatch(getOneLocation(id));
    }
  }, [id, dispatch]);

  useEffect(() => {
    if (id) {
      dispatch(getSpecialityDishes(id));
    }
  }, [id, dispatch]);

  return (
    <>
      <Container>
        <div className="mb-8 flex items-center">
          <NavLink to={"/"}>
            <Text variant="caption"> Main Page &gt;</Text>
          </NavLink>
          <NavLink to={`/locations/${id}`}>
            <Text variant="bodyBold" className="ml-2">
              {oneLocation?.address}
            </Text>
          </NavLink>
        </div>
      </Container>

      <LocationHero />

      <PageBody>
        <Dishes title="Specialty Dishes" dishes={specialityDishes} />
      </PageBody>

      <Reviews feedbacks={feedbacks} id={id} />
    </>
  );
};
