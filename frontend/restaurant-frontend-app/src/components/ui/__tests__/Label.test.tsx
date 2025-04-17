import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { Label } from "../Label";

describe("Label.tsx", () => {
  it("should render correctly", () => {
    render(<Label/>);
    const labelElem = screen.getByTestId("label");
    expect(labelElem).toBeInTheDocument();
  })
})