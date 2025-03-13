import React, { useEffect, useState } from "react";
import { Input, Label } from "../ui";
import { EyeIcon, OpenEyeIcon } from "../icons";

interface Props {
  label: string;
  name: string;
  placeholder: string;
  value: string;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  isConfirm?: boolean;
  confirmValue?: string;
  error?: string;
}

interface PasswordRequirements {
  uppercase: boolean;
  lowercase: boolean;
  number: boolean;
  special: boolean;
  length: boolean;
}

export const PasswordInput: React.FC<Props> = ({
  label,
  name,
  value,
  placeholder,
  onChange,
  isConfirm,
  error,
  confirmValue,
}) => {
  const [requirements, setRequirements] = useState<PasswordRequirements>({
    uppercase: false,
    lowercase: false,
    number: false,
    special: false,
    length: false,
  });
  const [strong, setStrong] = useState<string>("");
  const [isConfirmValid, setIsConfirmValid] = useState<boolean>(false);
  const [showPassword, setShowPassword] = useState(false);

  useEffect(() => {
    const fulfilledRequirements =
      Object.values(requirements).filter(Boolean).length;

    if (fulfilledRequirements >= 5) {
      setStrong("strong");
    } else if (fulfilledRequirements >= 3) {
      setStrong("medium");
    } else {
      setStrong("weak");
    }
  }, [requirements]);

  useEffect(() => {
    if (!isConfirm) {
      const hasUppercase = /\p{Lu}/u.test(value);
      const hasLowercase = /\p{Ll}/u.test(value);
      const hasNumber = /\d/.test(value);
      const hasSpecial = /[!@#$%^&*()_+\-=[\]{};':"\\|,.<>/?`~]/.test(value);
      const validLength = value.length >= 8 && value.length <= 16;

      setRequirements({
        uppercase: hasUppercase,
        lowercase: hasLowercase,
        number: hasNumber,
        special: hasSpecial,
        length: validLength,
      });
    } else if (confirmValue) {
      setIsConfirmValid(value === confirmValue);
    }
  }, [value, isConfirm, confirmValue]);

  const getLabelStyle = (isValid: boolean) => {
    const baseStyle =
      "text-xs font-light before:content-[''] before:block before:w-[8px] before:h-[8px] before:rounded-full mt-1 flex items-center";
    const validColor = "text-green before:bg-green";
    const invalidColor = "text-red before:bg-red";
    const neutralColor = "text-neutral before:bg-neutral";

    if (value === "") return `${baseStyle} ${neutralColor}`;

    return `${baseStyle} ${isValid ? validColor : invalidColor}`;
  };

  const getConfirmStyle = () => {
    const baseStyle =
      "text-xs font-light before:content-[''] before:block before:w-[8px] before:h-[8px] before:rounded-full flex items-center";
    const validColor = "text-green before:bg-green";
    const invalidColor = "text-red before:bg-red";
    const neutralColor = !error
      ? "text-neutral before:bg-neutral"
      : "text-red before:bg-red";

    if (value === "") return `${baseStyle} ${neutralColor}`;

    return `${baseStyle} ${isConfirmValid ? validColor : invalidColor}`;
  };

  return (
    <div className="w-full mb-6">
      <div className="flex justify-between items-center">
        <Label htmlFor={name} className="font-medium mb-1 leading-[24px]">
          {label}
        </Label>
        {!isConfirm && (
          <p
            className={`text-xs font-light mt-1 flex items-center transition-opacity duration-200 
            ${value !== "" ? "opacity-100" : "opacity-0 hidden"}
            ${
              strong === "strong"
                ? "text-green before:bg-green"
                : strong === "medium"
                ? "text-yellow before:bg-yellow"
                : "text-red before:bg-red"
            }
              before:content-[''] before:block before:w-2 before:h-2 before:rounded-full before:mr-1`}
          >
            {strong === "strong"
              ? "Strong"
              : strong === "medium"
              ? "Medium"
              : "Weak"}
          </p>
        )}
      </div>
      <div className="relative">
        <Input
          id={name}
          type={showPassword ? "text" : "password"}
          name={name}
          value={value}
          placeholder={placeholder}
          onChange={onChange}
          className="h-[56px] w-full bg-primary "
        />
        <button
          type="button"
          onClick={() => setShowPassword((prev) => !prev)}
          className="absolute top-1/2 right-3 -translate-y-1/2 cursor-pointer"
        >
          {showPassword ? <OpenEyeIcon /> : <EyeIcon />}{" "}
          {/* Переключение иконки */}
        </button>
      </div>
      {isConfirm ? (
        <Label htmlFor={name} className={getConfirmStyle()}>
          Confirm password must match new password
        </Label>
      ) : (
        <>
          <Label
            htmlFor={name}
            className={getLabelStyle(requirements.uppercase)}
          >
            At least one uppercase letter required
          </Label>
          <Label
            htmlFor={name}
            className={getLabelStyle(requirements.lowercase)}
          >
            At least one lowercase letter required
          </Label>
          <Label htmlFor={name} className={getLabelStyle(requirements.number)}>
            At least one number required
          </Label>
          <Label htmlFor={name} className={getLabelStyle(requirements.special)}>
            At least one special character required
          </Label>
          <Label htmlFor={name} className={getLabelStyle(requirements.length)}>
            Password must be 8-16 characters long
          </Label>
        </>
      )}
    </div>
  );
};
