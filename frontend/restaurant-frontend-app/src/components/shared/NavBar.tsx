import { Container, Title, UserMenu } from ".";
import { Link, NavLink } from "react-router";
import { CartIcon, Logo } from "@/components/icons/";
import { Button } from "../ui";
import { useSelector } from "react-redux";
import { selectUser } from "@/app/slices/userSlice";
import { ComponentProps } from "react";
import { buttonVariants } from "../ui/Button";

export const NavBar = () => {
  const user = useSelector(selectUser);

  return (
    <Container>
      <section className="grid justify-center py-[0.75rem] gap-[1rem] lg:grid-cols-[auto_1fr_auto]">
        <div className="flex items-center">
          <Link to="/" className="inline-flex items-center gap-[0.75rem]">
            <Logo className="size-[48px]" />
            <Title variant="navBarLogo" />
          </Link>
        </div>
        <div className="flex flex-col justify-center items-center gap-[1rem] lg:flex-row lg:gap-[2rem]">
          <NavBarLink to="/">Main page</NavBarLink>
          <NavBarLink to="/booking">Book a Table</NavBarLink>
          <NavBarLink to="/reservations">Reservations</NavBarLink>
        </div>
        <div className="flex items-center justify-center self-center lg:justify-self-end">
          {user ? (
            <>
              <Button asChild variant="tertiary" size="sm">
                <Link to="#cart">
                  <CartIcon className="size-[24px]" />
                </Link>
              </Button>
              <UserMenu />
            </>
          ) : (
            <Link
              to="/signin"
              className={buttonVariants({ variant: "secondary", size: "l" })}
            >
              Sign&nbsp;In
            </Link>
          )}
        </div>
      </section>
    </Container>
  );
};

const NavBarLink = ({
  children,
  ...props
}: ComponentProps<"a"> & { to: string }) => {
  return (
    <NavLink
      className="fontset-navigation text-foreground border-b-[2px] border-transparent hover:border-b-foreground [&.active]:text-primary [&.active]:border-b-primary [&.active]:cursor-default"
      {...props}
    >
      {children}
    </NavLink>
  );
};
