import { useAppDispatch } from "@/app/hooks";
import { register } from "@/app/thunks/userThunks";
import { RegistrationFields } from "@/types";
import { useState } from "react";
import { useNavigate } from "react-router";

export const useForm = (initialState: RegistrationFields) => {
  const [state, setState] = useState<RegistrationFields>(initialState);
  const [submitted, setSubmitted] = useState(false);
  const navigate = useNavigate();

  const dispatch = useAppDispatch();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setState((prev) => ({ ...prev, [name]: value }));
  };

  const onSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setSubmitted(true);

    try {
      const data = {
        firstName: state.name,
        lastName: state.lastName,
        email: state.email,
        password: state.password,
      };

      const result = await dispatch(register(data)).unwrap();

      console.log("Registration successful:", result);
      navigate("/");
    } catch (error) {
      console.error("Registration failed:", error);
    }
  };

  return { state, handleChange, onSubmit, submitted };
};
