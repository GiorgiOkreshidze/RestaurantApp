import {
  DropdownMenu,
  DropdownMenuSeparator,
  DropdownMenuContent,
  DropdownMenuTrigger,
} from "@/components/ui/";
import { LogOutIcon, UserCircledIcon, UserIcon } from "../icons";
import { Text } from "../ui";
import { DropdownMenuLinkItem } from "../ui/DropdownMenu";

export const UserMenu = () => {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger className="cursor-pointer">
        <UserCircledIcon className="size-[24px]" />
      </DropdownMenuTrigger>
      <DropdownMenuContent
        align="end"
        className="bg-neutral-0 p-0 rounded border-none shadow-card"
      >
        <DropdownMenuLinkItem
          to="#first"
          className="flex-col items-start gap-[0px]"
        >
          <Text variant="bodyBold">Johnson Doe (Customer)</Text>
          <Text variant="caption">johnsondoe@nomail.com</Text>
        </DropdownMenuLinkItem>
        <DropdownMenuSeparator className="mx-[1.5rem]" />
        <DropdownMenuLinkItem to="#second">
          <UserIcon className="size-[24px]" />
          <Text variant="bodyBold">My Profile</Text>
        </DropdownMenuLinkItem>
        <DropdownMenuLinkItem to="#third">
          <LogOutIcon className="size-[24px]" />
          <Text variant="bodyBold">My Profile</Text>
        </DropdownMenuLinkItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
};
