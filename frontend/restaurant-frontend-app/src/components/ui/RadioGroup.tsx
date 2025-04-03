import * as RadioGroupPrimitive from "@radix-ui/react-radio-group";
import { cn } from "@/lib/utils";
import { CircleIcon } from "../icons";
import { ComponentPropsWithRef } from "react";

const RadioGroup = (props: RadiGroupProps) => {
  return (
    <RadioGroupPrimitive.Root
      className={cn("grid gap-2", props.className)}
      {...props}
    />
  );
};
RadioGroup.displayName = RadioGroupPrimitive.Root.displayName;

const RadioGroupItem = (props: RadioGroupItemProps) => {
  return (
    <RadioGroupPrimitive.Item
      ref={props.ref}
      className={cn(
        "aspect-square h-[1.5rem] w-[1.5rem] rounded-full border border-primary text-primary shadow focus:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50",
        props.className,
      )}
      {...props}
    >
      <RadioGroupPrimitive.Indicator className="flex items-center justify-center">
        <CircleIcon />
      </RadioGroupPrimitive.Indicator>
    </RadioGroupPrimitive.Item>
  );
};
RadioGroupItem.displayName = RadioGroupPrimitive.Item.displayName;

export { RadioGroup, RadioGroupItem };

type RadiGroupProps = ComponentPropsWithRef<typeof RadioGroupPrimitive.Root>;
type RadioGroupItemProps = ComponentPropsWithRef<
  typeof RadioGroupPrimitive.Item
>;
