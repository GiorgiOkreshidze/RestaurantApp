import { selectUser } from "@/app/slices/userSlice";
import { useSelector } from "react-redux";
import { Text } from "../ui";
import { Logo } from "../icons";
import { Container } from "./Container";

export const PageHeading = () => {
  const user = useSelector(selectUser);

  return (
    <section className="bg-[url('@/assets/images/page-title-bg.png')] bg-center bg-cover bg-white/30 bg-blend-overlay">
      <Container className="flex flex-col items-center gap-[1rem] py-[1.125rem] px-[2.5rem] md:flex-row md:justify-between">
        <Text
          variant="h2"
          className="text-[clamp(1rem,5vw,1.5rem)] text-neutral-0 text-center"
        >
          Hello, {user?.firstName || ""} {user?.lastName || ""}{" "}
          {user?.role ? `(${user.role})` : ""}
        </Text>
        <Logo variant="white" className="size-[68px]" />
      </Container>
    </section>
  );
};
