import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { Input } from "../Input";

describe("Input.tsx", () => {
  it("should render correctly", () => {
    render(<Input/>);
    const inputElem = screen.getByTestId("input");
    expect(inputElem).toBeInTheDocument();
  })

  it("should render correctly with 'isInvalid' true", () => {
    render(<Input isInvalid/>);
    const inputElem = screen.getByTestId("input");
    expect(inputElem).toBeInTheDocument();
  })
})