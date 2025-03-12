import { RegistrationFields } from "@/types";
import { useState } from "react";

export const useForm = (initialState: RegistrationFields) => {
  const [state, setState] = useState<RegistrationFields>(initialState);
  const [submitted, setSubmitted] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setState((prev) => ({ ...prev, [name]: value }));
  };

  const onSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    console.log(state);
    setSubmitted(true);
  };

  return { state, handleChange, onSubmit, submitted };
};
