import HeroImg from "../../assets/images/hero.jpg";
import { Title } from "./Title";
import { Button, Text } from "../ui";
import { Container } from "./container";

export const Hero = () => {
  return (
    <div
      className="py-10 bg-center bg-cover bg-no-repeat"
      style={{ backgroundImage: `url(${HeroImg})` }}
    >
      <Container className="p-0">
        <div className="max-w-[340px]">
          <Title variant="navBarLogo" className="text-green-200 !text-5xl" />
          <Text variant="body" className=" text-neutral-0 mt-6 mb-3">
            A network of restaurants in Tbilisi, Georgia, offering fresh,
            locally sourced dishes with a focus on health and sustainability.
          </Text>
          <Text variant="body" className=" text-neutral-0 mb-10">
            Our diverse menu includes vegetarian and vegan options, crafted to
            highlight the rich flavors of Georgian cuisine with a modern twist.
          </Text>

          <Button className="w-full">View Menu</Button>
        </div>
      </Container>
    </div>
  );
};
