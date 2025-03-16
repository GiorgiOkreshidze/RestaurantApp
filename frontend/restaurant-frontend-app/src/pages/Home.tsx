import { selectUser } from "@/app/slices/userSlice";
import { Dishes, Hero, Locations } from "@/components/shared";

export const Home = () => {
  return (
    <>
      <Hero />
      <Dishes />
      <Locations />
    </>
  );
};
