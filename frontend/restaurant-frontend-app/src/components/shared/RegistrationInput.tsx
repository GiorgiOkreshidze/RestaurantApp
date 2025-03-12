import React from "react";
import { Input, Label } from "../ui";

interface Props {
  label: string;
  secondLabel?: string;
  name: string;
  placeholder: string;
  value: string;
  error?: string;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
}

export const RegistrationInput: React.FC<Props> = ({
  label,
  secondLabel,
  name,
  placeholder,
  value,
  error,
  onChange,
}) => {
  return (
    <div className="w-full mb-6">
      <Label htmlFor={name} className="font-medium mb-1 leading-[24px]">
        {label}
      </Label>
      <Input
        id={name}
        name={name}
        value={value}
        placeholder={placeholder}
        className={`h-[56px] w-full bg-primary ${error ? "border-red" : ""} `}
        onChange={onChange}
      />
      <Label
        htmlFor={name}
        className={`text-xs font-light mt-2 ${
          error ? "text-red" : "text-neutral"
        } `}
      >
        {error ? error : secondLabel}
      </Label>
    </div>
  );
};
