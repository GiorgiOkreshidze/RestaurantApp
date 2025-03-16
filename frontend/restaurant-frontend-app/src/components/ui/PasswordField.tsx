import { ComponentProps, useState } from "react";
import { Button, Input } from ".";
import { EyeIcon, OpenEyeIcon } from "../icons";

export const PasswordField = ({
  ...props
}: ComponentProps<"input"> & {
  isInvalid: boolean;
}) => {
  const [showPassword, setShowPassword] = useState(false);
  const type = showPassword ? "text" : "password";

  return (
    <div className="relative">
      <Input type={type} {...props} />
      <Button
        variant="tertiary"
        className="absolute w-auto px-[1.125rem] h-[calc(100%-2px)] right-[1px] top-[50%] translate-y-[-50%]"
        onClick={() => setShowPassword(!showPassword)}
      >
        {showPassword ? <EyeIcon /> : <OpenEyeIcon />}
      </Button>
    </div>
  );
};
