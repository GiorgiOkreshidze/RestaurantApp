import { UserType } from "@/types/user.types";
import { Label, RadioGroup, RadioGroupItem, Text } from "../ui";
import { buttonVariants } from "../ui/Button";
import { cn } from "@/lib/utils";
import { Dispatch, SetStateAction } from "react";

export const UserPicker = (props: Props) => {
  const { userType, setUserType, ...restProps } = props;

  return (
    <RadioGroup
      value={userType}
      onValueChange={(value: UserType) => setUserType(value)}
      {...restProps}
    >
      {userOptions.map(({ id, title }) => (
        <Label
          key={id}
          htmlFor={id}
          className={cn(
            buttonVariants({ variant: "trigger" }),
            id === userType && "border-primary",
          )}
        >
          <RadioGroupItem value={id} id={id} />
          <Text variant="buttonSecondary">{title}</Text>
        </Label>
      ))}
    </RadioGroup>
  );
};

interface Props {
  className?: string;
  userType: UserType;
  setUserType: Dispatch<SetStateAction<UserType>>;
}

const userOptions = [
  {
    id: UserType.VISITOR,
    title: "Visitor",
  },
  {
    id: UserType.CUSTOMER,
    title: "Existing Customer",
  },
];
