import {
  Container,
  PasswordInput,
  RegistrationInput,
} from "@/components/shared";
import { Button } from "@/components/ui";
import { RegistrationFields } from "@/types";
import { useForm } from "@/hooks/useForm";
import { useFormValidation } from "@/hooks/useFormValidation";

const initialState: RegistrationFields = {
  name: "",
  lastName: "",
  email: "",
  password: "",
  confirmPassword: "",
};

const Registration = () => {
  const { state, handleChange, onSubmit, submitted } = useForm(initialState);
  const errors = useFormValidation(state, submitted);

  return (
    <Container>
      <div className="flex w-full">
        <div className="w-1/2 px-[84px] py-[34px]">
          <h3 className="uppercase text-sm block">LET'S GET YOU STARTED</h3>
          <h2 className="text-2xl block mb-[64px]">Create an Account</h2>
          <form onSubmit={onSubmit}>
            <div className="flex gap-4">
              <div className="w-full">
                <RegistrationInput
                  name="name"
                  value={state.name}
                  label="First Name"
                  secondLabel="e.g. Jonson"
                  placeholder="Enter your First Name"
                  onChange={handleChange}
                  error={errors.name}
                />
              </div>
              <div className="w-full">
                <RegistrationInput
                  name="lastName"
                  value={state.lastName}
                  label="Last Name"
                  secondLabel="e.g. Doe"
                  placeholder="Enter your Last Name"
                  onChange={handleChange}
                  error={errors.lastName}
                />
              </div>
            </div>
            <div>
              <RegistrationInput
                name="email"
                value={state.email}
                label="Email"
                secondLabel="e.g. username@domain.com"
                placeholder="Enter your Email"
                onChange={handleChange}
                error={errors.email}
              />
            </div>
            <div>
              <PasswordInput
                label="Password"
                name="password"
                value={state.password}
                placeholder="Enter your Password"
                onChange={handleChange}
              />
            </div>
            <div>
              <PasswordInput
                label="Confirm New Password"
                name="confirmPassword"
                placeholder="Confirm New Password"
                value={state.confirmPassword}
                onChange={handleChange}
                isConfirm
                confirmValue={state.password}
              />
            </div>
            <div className="mt-6">
              <Button className="bg-secondary w-full h-[56px] rounded-[8px] border">
                Create an Account
              </Button>
              <p className="font-light text-xs mt-4">
                Already have an account?{" "}
                <a href="/login" className="text-blue font-bold ">
                  Login{" "}
                </a>
                instead
              </p>
            </div>
          </form>
        </div>
        <div className="w-1/2 p-4">
          <div className="w-full">
            <img src="src/assets/images/registrationImage.svg" alt="" />
          </div>
        </div>
      </div>
    </Container>
  );
};

export default Registration;
