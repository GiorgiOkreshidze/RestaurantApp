import React, { useEffect, useState } from "react";
import { Input, Label } from "../ui";

interface Props {
  label: string;
  name: string;
  placeholder: string;
  value: string;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  isConfirm?: boolean;
  confirmValue?: string;
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
  confirmValue,
}) => {
  const [requirements, setRequirements] = useState<PasswordRequirements>({
    uppercase: false,
    lowercase: false,
    number: false,
    special: false,
    length: false,
  });

  const [isConfirmValid, setIsConfirmValid] = useState<boolean>(false);

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
    const validColor = "text-green-500 before:bg-green-500";
    const invalidColor = "text-red-500 before:bg-red-500";
    const neutralColor = "text-neutral before:bg-neutral";

    if (value === "") return `${baseStyle} ${neutralColor}`;

    return `${baseStyle} ${isValid ? validColor : invalidColor}`;
  };

  const getConfirmStyle = () => {
    const baseStyle =
      "text-xs font-light before:content-[''] before:block before:w-[8px] before:h-[8px] before:rounded-full flex items-center";
    const validColor = "text-green before:bg-green";
    const invalidColor = "text-red before:bg-red";
    const neutralColor = "text-neutral before:bg-neutral";

    if (value === "") return `${baseStyle} ${neutralColor}`;

    return `${baseStyle} ${isConfirmValid ? validColor : invalidColor}`;
  };

  return (
    <div className="w-full mb-6">
      <Label htmlFor={name} className="font-medium mb-1 leading-[24px]">
        {label}
      </Label>
      <Input
        id={name}
        name={name}
        type="password"
        value={value}
        placeholder={placeholder}
        onChange={onChange}
        className="h-[56px] w-full bg-primary"
      />
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
