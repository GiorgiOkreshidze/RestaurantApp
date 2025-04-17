import { describe, it, expect, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { Button } from "../Button";

vi.mock("@/lib/utils", () => ({
  cn: (...inputs: (string | undefined)[]) => inputs.filter(Boolean).join(" "),
}));

describe("Button", () => {
  it("renders correctly with children", () => {
    render(<Button>Click me</Button>);
    expect(
      screen.getByRole("button", { name: /click me/i })
    ).toBeInTheDocument();
  });

  it("renders with icon", () => {
    const mockIcon = <span data-testid="test-icon">Icon</span>;
    render(<Button icon={mockIcon}>With Icon</Button>);
    expect(screen.getByTestId("test-icon")).toBeInTheDocument();
  });

  it("renders as custom element when asChild is true", () => {
    render(
      <Button asChild>
        <a href="#test">Link Button</a>
      </Button>
    );
    expect(screen.getByRole("link")).toBeInTheDocument();
  });

  it("calls onClick handler when clicked", async () => {
    const handleClick = vi.fn();
    render(<Button onClick={handleClick}>Click me</Button>);

    await userEvent.click(screen.getByRole("button"));
    expect(handleClick).toHaveBeenCalledTimes(1);
  });

  it("can be disabled", () => {
    render(<Button disabled>Disabled</Button>);
    expect(screen.getByRole("button")).toBeDisabled();
  });
});
