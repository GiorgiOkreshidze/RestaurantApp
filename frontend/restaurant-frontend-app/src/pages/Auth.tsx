import { RegForm, Container, Title, LoginForm } from "@/components/shared";
import { Logo } from "@/components/icons/";
import { useLocation } from "react-router";

export const Auth = () => {
  const { pathname } = useLocation();

  return (
    <Container className="flex *:basis-full items-center min-h-[100vh]">
      <div className="flex flex-col gap-[32px] md:flex-row-reverse">
        <div className="flex items-center justify-center gap-[12px] md:flex-1/2 md:flex-col-reverse md:gap-[72px]">
          <Logo className="w-[48px] md:w-[min(70%,417px)]" />
          <Title />
        </div>
        <section className="md:flex-1/2 flex flex-col justify-center md:items-center">
          <div className="w-full md:max-w-[496px]">
            {pathname === "/signin" ? <LoginForm /> : <RegForm />}
          </div>
        </section>
      </div>
    </Container>
  );
};
