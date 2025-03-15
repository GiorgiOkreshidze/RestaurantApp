import { Container, Title } from "@/components/shared";
import LoginForm from "@/components/shared/LoginForm";
import { Logo } from "@/components/svg";

export const Login = () => {
  return (
    <Container>
      <div className="flex items-center gap-x-[32px]">
        <LoginForm className="w-1/2 py-[180px]" />
        <section className="flex flex-col items-center w-1/2 py-[120px]">
          <Title />
          <Logo className="mt-[70px] max-w-[477px]" />
        </section>
      </div>
    </Container>
  );
};
