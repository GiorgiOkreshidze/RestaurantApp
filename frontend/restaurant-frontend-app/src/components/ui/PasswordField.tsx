import { ComponentProps, Dispatch, SetStateAction, useState } from "react";
import { Button, Input } from ".";
import { EyeIcon, OpenEyeIcon } from "../icons";

export const PasswordField = ({
  showPassword: showPasswordProps,
  setShowPassword: setShowPasswordProps,
  ...props
}: ComponentProps<"input"> & {
  isInvalid: boolean;
  showPassword?: boolean;
  setShowPassword?: Dispatch<SetStateAction<boolean>>;
}) => {
  const [showPasswordLocal, setShowPasswordLocal] = useState(false);
  const showPassword = showPasswordProps ?? showPasswordLocal;
  const setShowPassword = setShowPasswordProps ?? setShowPasswordLocal;
  const type = showPassword ? "text" : "password";

  return (
    <div className="relative">
      <Input className="w-full" type={type} {...props} />
      <Button
        variant="tertiary"
        className="absolute w-auto px-[1.125rem] h-[calc(100%-2px)] right-[1px] top-[50%] translate-y-[-50%] hover:bg-neutral-100"
        onClick={() => setShowPassword(!showPassword)}
      >
        {showPassword ? <EyeIcon /> : <OpenEyeIcon />}
      </Button>
    </div>
  );
};
