import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import {
  ChevronDownIcon,
  CircleIcon,
  ClockIcon,
  EmptyStar,
  EyeIcon,
  LocationIcon,
  Logo,
  LogOutIcon,
  MinusIcon,
  OpenEyeIcon,
  PeopleIcon,
  PlusIcon,
  SearchMagnifierIcon,
  Star,
  StarIcon,
  UserCircledIcon,
  UserIcon,
  BinIcon,
  CalendarIcon,
  CartIcon,
} from "../";

describe("icons", () => {
  it("ChevronDownIcon.tsx should render correctly", () => {
    render(<ChevronDownIcon />);
    const iconElem = screen.getByTestId("ChevronDownIcon");
    expect(iconElem).toBeInTheDocument();
  });

  it("CircleIcon.tsx should render correctly", () => {
    render(<CircleIcon />);
    const iconElem = screen.getByTestId("CircleIcon");
    expect(iconElem).toBeInTheDocument();
  });

  it("ClockIcon.tsx should render correctly", () => {
    render(<ClockIcon />);
    const iconElem = screen.getByTestId("ClockIcon");
    expect(iconElem).toBeInTheDocument();
  });

  it("EmptyStar.tsx should render correctly", () => {
    render(<EmptyStar />);
    const iconElem = screen.getByTestId("EmptyStar");
    expect(iconElem).toBeInTheDocument();
  });

  it("EyeIcon.tsx should render correctly", () => {
    render(<EyeIcon />);
    const iconElem = screen.getByTestId("EyeIcon");
    expect(iconElem).toBeInTheDocument();
  });

  it("LocationIcon.tsx should render correctly", () => {
    render(<LocationIcon />);
    const iconElem = screen.getByTestId("LocationIcon");
    expect(iconElem).toBeInTheDocument();
  });

  it("Logo.tsx should render correctly", () => {
    render(<Logo />);
    const iconElem = screen.getByTestId("Logo");
    expect(iconElem).toBeInTheDocument();
  });

  it("Logo.tsx should render correctly", () => {
    render(<Logo variant="white" />);
    const iconElem = screen.getByTestId("Logo");
    expect(iconElem).toBeInTheDocument();
  });

  it("LogOutIcon.tsx should render correctly", () => {
    render(<LogOutIcon />);
    const iconElem = screen.getByTestId("LogOutIcon");
    expect(iconElem).toBeInTheDocument();
  });

  it("MinusIcon.tsx should render correctly", () => {
    render(<MinusIcon />);
    const iconElem = screen.getByTestId("MinusIcon");
    expect(iconElem).toBeInTheDocument();
  });

  it("OpenEyeIcon.tsx should render correctly", () => {
    render(<OpenEyeIcon />);
    const iconElem = screen.getByTestId("OpenEyeIcon");
    expect(iconElem).toBeInTheDocument();
  });

  it("PeopleIcon.tsx should render correctly", () => {
    render(<PeopleIcon />);
    const iconElem = screen.getByTestId("PeopleIcon");
    expect(iconElem).toBeInTheDocument();
  });

  it("PlusIcon.tsx should render correctly", () => {
    render(<PlusIcon />);
    const iconElem = screen.getByTestId("PlusIcon");
    expect(iconElem).toBeInTheDocument();
  });

  it("SearchMagnifierIcon.tsx should render correctly", () => {
    render(<SearchMagnifierIcon />);
    const iconElem = screen.getByTestId("SearchMagnifierIcon");
    expect(iconElem).toBeInTheDocument();
  });

  it("Star.tsx should render correctly", () => {
    render(<Star />);
    const iconElem = screen.getByTestId("Star");
    expect(iconElem).toBeInTheDocument();
  });

  it("StarIcon.tsx should render correctly", () => {
    render(<StarIcon />);
    const iconElem = screen.getByTestId("StarIcon");
    expect(iconElem).toBeInTheDocument();
  });

  it("UserCircledIcon.tsx should render correctly", () => {
    render(<UserCircledIcon />);
    const iconElem = screen.getByTestId("UserCircledIcon");
    expect(iconElem).toBeInTheDocument();
  });

  it("UserIcon.tsx should render correctly", () => {
    render(<UserIcon />);
    const iconElem = screen.getByTestId("UserIcon");
    expect(iconElem).toBeInTheDocument();
  });

  it("BinIcon.tsx should render correctly", () => {
    render(<BinIcon />);
    const iconElem = screen.getByTestId("BinIcon");
    expect(iconElem).toBeInTheDocument();
  });

  it("CalendarIcon.tsx should render correctly", () => {
    render(<CalendarIcon />);
    const iconElem = screen.getByTestId("CalendarIcon");
    expect(iconElem).toBeInTheDocument();
  });

  it("CartIcon.tsx should render correctly", () => {
    render(<CartIcon />);
    const iconElem = screen.getByTestId("CartIcon");
    expect(iconElem).toBeInTheDocument();
  });
});
