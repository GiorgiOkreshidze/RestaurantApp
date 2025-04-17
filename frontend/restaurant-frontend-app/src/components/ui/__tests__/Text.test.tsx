import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { Text } from "../Text";

describe("Text.tsx", () => {
  it("should render correctly", () => {
    render(<Text>Hello world</Text>);
    const textElem = screen.getByText("Hello world");
    expect(textElem).toBeInTheDocument();
  })
})