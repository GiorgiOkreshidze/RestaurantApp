import { Container } from ".";
import { Link, NavLink } from "react-router";
import { Logo } from "@/components/icons/";
import { Button, Text } from "../ui";

export const NavBar = () => {
  return (
    <Container className="!py-3">
      <div className="grid grid-cols-[1fr_auto_1fr] items-center">
        <div>
          <Link to="/">
            <div className="flex gap-[12px] max-h-[50px] w-[250px] items-center">
              <Logo className="w-[50px] h-[50px]" />
              <Text variant="h2" className="w-full">
                <span className="text-green-200">Green</span> & Tasty
              </Text>
            </div>
          </Link>
        </div>
        <div className="flex gap-4 ">
          <NavLink
            to="/"
            className="[&.active]:text-green-200 [&.active]:border-b-2 [&.active]:border-green-200"
          >
            <Text variant="h2" className="text-inherit">
              Main page
            </Text>
          </NavLink>

          <NavLink
            to="/booking"
            className="[&.active]:text-green-200 [&.active]:border-b-2 [&.active]:border-green-200"
          >
            <Text variant="h2" className="text-inherit">
              Book a Table
            </Text>
          </NavLink>
        </div>
        <div className="ml-auto">
          <Button asChild variant="secondary" size="l">
            <Link to="/signin">Sign&nbsp;In</Link>
          </Button>
        </div>
      </div>
    </Container>
  );
};
