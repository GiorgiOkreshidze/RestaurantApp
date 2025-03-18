import {
  DropdownMenu,
  DropdownMenuSeparator,
  DropdownMenuContent,
  DropdownMenuTrigger,
} from "@/components/ui/";
import { LogOutIcon, UserCircledIcon, UserIcon } from "../icons";
import { Text } from "../ui";
import { DropdownMenuLinkItem } from "../ui/DropdownMenu";
import { useSelector } from "react-redux";
import { selectUser } from "@/app/slices/userSlice";

export const UserMenu = () => {
  const user = useSelector(selectUser);

  return (
    <DropdownMenu>
      <DropdownMenuTrigger className="cursor-pointer">
        <UserCircledIcon className="size-[24px]" />
      </DropdownMenuTrigger>
      <DropdownMenuContent
        align="end"
        className="bg-neutral-0 p-0 shadow-card rounded border-none min-w-[216px]"
      >
        <DropdownMenuLinkItem
          to="#first"
          className="flex-col items-start gap-[0px]"
        >
          <Text variant="bodyBold">
            {user && (user.name || user?.lastName)
              ? `${user.name} ${user.lastName}`
              : "Username"}
          </Text>
          <Text variant="caption">{user?.email || "email@site.com"}</Text>
        </DropdownMenuLinkItem>
        <DropdownMenuSeparator className="mx-[1.5rem]" />
        <DropdownMenuLinkItem to="#second">
          <UserIcon className="size-[24px] mr-[1rem]" />
          <Text variant="bodyBold">My Profile</Text>
        </DropdownMenuLinkItem>
        <DropdownMenuLinkItem to="#third">
          <LogOutIcon className="size-[24px] mr-[1rem]" />
          <Text variant="bodyBold">My Profile</Text>
        </DropdownMenuLinkItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
};
