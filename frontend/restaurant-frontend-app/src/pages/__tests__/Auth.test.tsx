import { describe, it, expect, vi } from "vitest";
import { render, screen } from "@testing-library/react";

// Мокаем зависимости
vi.mock("react-router", () => ({
  useLocation: vi.fn(),
}));

vi.mock("@/components/shared", () => ({
  RegForm: () => <div data-testid="reg-form">Registration Form</div>,
  Container: ({ children, className }) => (
    <div data-testid="container" className={className}>
      {children}
    </div>
  ),
  BrandTitle: ({ variant }) => (
    <div data-testid="brand-title" data-variant={variant}>
      Brand Title
    </div>
  ),
  LoginForm: () => <div data-testid="login-form">Login Form</div>,
}));

vi.mock("@/components/icons/", () => ({
  Logo: ({ className }) => (
    <div data-testid="logo" className={className}>
      Logo
    </div>
  ),
}));

import { useLocation } from "react-router";
import { Auth } from "../Auth";

describe("Auth Component", () => {
  it("renders LoginForm when pathname is /signin", () => {
    // Устанавливаем путь /signin
    vi.mocked(useLocation).mockReturnValue({
        pathname: "/signin",
        state: null,
        search: "",
        hash: "",
        key: ""
    });

    render(<Auth />);

    // Проверяем наличие общих элементов
    expect(screen.getByTestId("container")).toBeInTheDocument();
    expect(screen.getByTestId("logo")).toBeInTheDocument();
    expect(screen.getByTestId("brand-title")).toBeInTheDocument();

    // Проверяем, что отображается LoginForm
    expect(screen.getByTestId("login-form")).toBeInTheDocument();
    expect(screen.queryByTestId("reg-form")).not.toBeInTheDocument();
  });

  it("renders RegForm when pathname is not /signin", () => {
    // Устанавливаем путь /signup
    vi.mocked(useLocation).mockReturnValue({
        pathname: "/signup",
        state: null,
        search: "",
        hash: "",
        key: ""
    });

    render(<Auth />);

    // Проверяем наличие общих элементов
    expect(screen.getByTestId("container")).toBeInTheDocument();
    expect(screen.getByTestId("logo")).toBeInTheDocument();
    expect(screen.getByTestId("brand-title")).toBeInTheDocument();

    // Проверяем, что отображается RegForm
    expect(screen.getByTestId("reg-form")).toBeInTheDocument();
    expect(screen.queryByTestId("login-form")).not.toBeInTheDocument();
  });

  it("passes correct variant to BrandTitle", () => {
    vi.mocked(useLocation).mockReturnValue({
        pathname: "/signin",
        state: null,
        search: "",
        hash: "",
        key: ""
    });

    render(<Auth />);

    const brandTitle = screen.getByTestId("brand-title");
    expect(brandTitle).toHaveAttribute("data-variant", "huge");
  });

  it("applies correct classes to Logo", () => {
    vi.mocked(useLocation).mockReturnValue({
        pathname: "/signin",
        state: null,
        search: "",
        hash: "",
        key: ""
    });

    render(<Auth />);

    const logo = screen.getByTestId("logo");
    expect(logo).toHaveClass("w-[48px]");
    expect(logo).toHaveClass("md:w-[min(70%,417px)]");
  });
});
