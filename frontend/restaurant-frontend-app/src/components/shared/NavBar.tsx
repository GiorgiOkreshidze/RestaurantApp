import { Container } from ".";
import { Link, NavLink } from "react-router";
import { Logo } from "@/components/icons/";
import { Button, Text } from "../ui";

export const NavBar = () => {
  return (
    <Container>
      <div className="grid grid-cols-[1fr_auto_1fr] items-center">
        <div>
          <Link to="/">
            <div className="flex gap-[12px] max-h-[50px] w-[250px] items-center">
              <Logo className="w-[50px] h-[50px]" />
              <Text variant="h1" className="w-full">
                <span className="text-green-200">Green</span> & Tasty
              </Text>
            </div>
          </Link>
        </div>
        <div className="flex gap-4 ">
          <NavLink to="/" className="[&.active]:text-green-200">
            <Text variant="h2" className="text-inherit">
              Main page
            </Text>
          </NavLink>

          <NavLink to="/booking" className="[&.active]:text-green-200">
            <Text variant="h2" className="text-inherit">
              Book a Table
            </Text>
          </NavLink>
        </div>
        <div className="w-[80px] ml-auto">
          <Button variant="outline">
            <Link to="/signin">
              <Text variant="buttonPrimary" className="text-inherit">
                Sign In
              </Text>
            </Link>
          </Button>
        </div>
      </div>
    </Container>
  );
};
