import {
  DropdownMenu,
  DropdownMenuSeparator,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/";
import { UserCircledIcon, UserIcon } from "../icons";
import { Text } from "../ui";
import { Link } from "react-router";

export const UserMenu = () => {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger className="cursor-pointer">
        <UserCircledIcon width={24} height={24} />
      </DropdownMenuTrigger>
      <DropdownMenuContent
        align="end"
        className="bg-neutral-0 p-[1.5rem] rounded border-none shadow-card"
      >
        <DropdownMenuItem className="flex-col items-start">
          <Link to="#first" className="cursor-pointer">
            <Text variant="bodyBold">Johnson Doe (Customer)</Text>
            <Text variant="caption">johnsondoe@nomail.com</Text>
          </Link>
        </DropdownMenuItem>
        <DropdownMenuSeparator />
        <DropdownMenuItem className="flex-col items-start">
          <Link
            to="#second"
            className="flex items-center fontset-bodyBold cursor-pointer gap-[12px]"
          >
            <UserIcon className="size-[24px]" />
            <Text variant="bodyBold">My Profile</Text>
          </Link>
        </DropdownMenuItem>
        <DropdownMenuItem className="flex-col items-start"></DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
};
