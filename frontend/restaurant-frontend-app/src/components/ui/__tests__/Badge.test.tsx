import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { Badge } from "../Badge";

describe("Badge.tsx", () => {
  it("should render correctly with 'Reserved' status", () => {
    render(<Badge status="Reserved" />);
    const badgeElement = screen.getByTestId("badge");
    expect(badgeElement).toBeInTheDocument();
  });

  it("should render correctly with 'Cancelled' status", () => {
    render(<Badge status="Cancelled" />);
    const badgeElement = screen.getByTestId("badge");
    expect(badgeElement).toBeInTheDocument();
  });
  
  it("should render correctly with 'In Progress' status", () => {
    render(<Badge status="In Progress" />);
    const badgeElement = screen.getByTestId("badge");
    expect(badgeElement).toBeInTheDocument();
  });

  it("should render correctly with 'Finished' status", () => {
    render(<Badge status="Finished" />);
    const badgeElement = screen.getByTestId("badge");
    expect(badgeElement).toBeInTheDocument();
  });

  it("should render correctly with 'On Stop' status", () => {
    render(<Badge status="On Stop" />);
    const badgeElement = screen.getByTestId("badge");
    expect(badgeElement).toBeInTheDocument();
  });

  it("should render correctly with 'asChild'", () => {
    render(
      <Badge status="Reserved" asChild>
        <p>Reserved</p>
      </Badge>,
    );
    const badgeElement = screen.getByTestId("badge");
    expect(badgeElement).toBeInTheDocument();
  });
});
