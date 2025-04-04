import { RegistrationFields, ValidationErrors } from "@/types";
import { useEffect, useState } from "react";

const existingEmails = ["test@example.com", "user@domain.com", "admin@site.ru"];

export const useFormValidation = (
  state: RegistrationFields,
  submitted: boolean,
) => {
  const [errors, setErrors] = useState<ValidationErrors>({});

  useEffect(() => {
    const nameRegex = /^[A-Za-z'-]{1,50}$/;
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    const passwordRegex =
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*().,?_~])[A-Za-z\d!@#$%^&*().,?_~]{8,16}$/;
    if (submitted) {
      const newErrors: ValidationErrors = {};
      if (!state.name) newErrors.name = "Enter your name.";
      else if (!nameRegex.test(state.name))
        newErrors.name =
          "First name must be up to 50 characters. Only Latin letters, hyphens, and apostrophes are allowed.";

      if (!state.lastName) newErrors.lastName = "Enter your last name.";
      else if (!nameRegex.test(state.lastName))
        newErrors.lastName =
          "Last name must be up to 50 characters. Only Latin letters, hyphens, and apostrophes are allowed.";

      if (!state.email) newErrors.email = "Enter your email.";
      else if (!emailRegex.test(state.email))
        newErrors.email =
          "Invalid email address. Please ensure it follows the format: username@domain.com";
      else if (existingEmails.includes(state.email))
        newErrors.email = "This email is already registered!";

      if (!state.password) newErrors.password = "Enter your password.";
      else if (!passwordRegex.test(state.password))
        newErrors.password = "Invalid password.";

      if (!state.confirmPassword) {
        newErrors.confirmPassword = "Confirm your password.";
      } else if (state.password !== state.confirmPassword) {
        newErrors.confirmPassword = "Passwords do not match.";
      }

      setErrors(newErrors);
    }
  }, [state, submitted]);

  return errors;
};
