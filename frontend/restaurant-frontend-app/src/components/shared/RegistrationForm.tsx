import {
  Container,
  PasswordInput,
  RegistrationInput,
} from "@/components/shared";
import { Button } from "@/components/ui";
import { RegistrationFields } from "@/types";
import { useForm } from "@/hooks/useForm";
import { useFormValidation } from "@/hooks/useFormValidation";
import { useEffect } from "react";
import { Link } from "react-router";
import { useSelector } from "react-redux";
import { selectRegisterLoading } from "@/app/slices/userSlice";

const initialState: RegistrationFields = {
  name: "",
  lastName: "",
  email: "",
  password: "",
  confirmPassword: "",
};

export const Registration = () => {
  const { state, handleChange, onSubmit, submitted } = useForm(initialState);
  const errors = useFormValidation(state, submitted);
  const isLoading = useSelector(selectRegisterLoading);

  useEffect(() => {
    console.log(errors);
  }, [errors]);

  return (
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
            error={errors.confirmPassword}
          />
        </div>
        <div className="mt-6">
          <Button
            className="bg-secondary w-full h-[56px] rounded-[8px] border flex items-center justify-center"
            disabled={isLoading}
          >
            {isLoading ? (
              <span className="animate-spin border-2 border-t-transparent border-white w-5 h-5 rounded-full"></span>
            ) : (
              "Create an Account"
            )}
          </Button>

          <p className="font-light text-xs mt-4">
            Already have an account?{" "}
            <Link className="text-blue font-bold " to={"/login"}>
              Login{" "}
            </Link>
            instead
          </p>
        </div>
      </form>
    </div>
  );
};
