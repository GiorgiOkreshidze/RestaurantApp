import { ReactNode } from "react";
import { Container } from "./Container";

export const InfoBar = (props: Props) => {
  return (
    <div className="bg-primary-light">
      <Container className="flex items-center justify-center text-center fontset-body py-[0.75rem] gap-[1rem]">
        {props.children}
      </Container>
    </div>
  );
};

interface Props {
  children: ReactNode;
}
