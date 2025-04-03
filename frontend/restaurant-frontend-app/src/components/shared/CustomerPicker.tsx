import { Input, Popover, PopoverContent, PopoverTrigger, Text } from "../ui";
import { UserDataResponse } from "@/types";
import { useRef, useState } from "react";

export const CustomerPicker = (props: Props) => {
  const { customerList, customerId, setCustomerId } = props;
  const [isOpen, setIsOpen] = useState(false);
  const [value, setValue] = useState("");
  const inputRef = useRef<HTMLInputElement>(null);

  return (
    <div className="bg-neutral-0 border border-primary p-[1.5rem] rounded">
      <Text variant="bodyBold">Customer's Name</Text>
      <Popover open={isOpen} onOpenChange={setIsOpen} modal={true}>
        <PopoverTrigger onClick={(e) => e.preventDefault()} className="w-full">
          <Input
            value={value}
            onChange={(e) => {
              setValue(e.target.value);
              setIsOpen(Boolean(e.target.value));
              setCustomerId("");
            }}
            ref={inputRef}
            placeholder="Enter customer's name"
            onBlur={() => {
              if (isOpen) return;
              !customerId && setValue("");
            }}
          />
        </PopoverTrigger>
        <Text variant="caption" className="mt-[4px]">
          e.g. Jonson Doe
        </Text>
        <PopoverContent
          className="flex p-0 flex-col max-h-[250px] w-[var(--radix-popover-trigger-width)] overflow-hidden"
          side="bottom"
          onOpenAutoFocus={(e) => e.preventDefault()}
          onCloseAutoFocus={(e) => e.preventDefault()}
        >
          <ul className="overflow-auto">
            {customerList
              ?.filter((customer) =>
                `${customer.firstName} ${customer.lastName}`
                  .toLowerCase()
                  .includes(value.toLowerCase()),
              )
              .map((customer) => (
                <li
                  key={customer.id}
                  value={customer.id}
                  className="fontset-bodyBold hover:bg-green-100 outline-hidden py-[0.25rem] px-[0.5rem] focus:bg-green-100 cursor-pointer data-[disabled]:pointer-events-none data-[disabled]:opacity-50"
                  onClick={(e) => {
                    setCustomerId(customer.id);
                    setIsOpen(false);
                    setValue(`${customer.firstName} ${customer.lastName}`);
                  }}
                >
                  {customer.firstName} {customer.lastName}
                </li>
              ))}
          </ul>
        </PopoverContent>
      </Popover>
    </div>
  );
};

interface Props {
  customerList: UserDataResponse[] | null;
  customerId: string;
  setCustomerId: React.Dispatch<React.SetStateAction<string>>;
}
