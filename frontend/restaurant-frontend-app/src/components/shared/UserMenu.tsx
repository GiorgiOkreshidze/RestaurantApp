import {
  DropdownMenu,
  DropdownMenuSeparator,
  DropdownMenuContent,
  DropdownMenuTrigger,
  Button,
} from "@/components/ui/";
import { LogOutIcon, UserCircledIcon, UserIcon } from "../icons";
import { Text } from "../ui";
import {
  DropdownMenuItem,
  DropdownMenuLabel,
} from "@/components/ui/DropdownMenu";
import { useSelector } from "react-redux";
import { logout, selectUser } from "@/app/slices/userSlice";
import { Link } from "react-router";
import { useAppDispatch } from "@/app/hooks";
import { signout } from "@/app/thunks/userThunks";

export const UserMenu = () => {
  const user = useSelector(selectUser);
  const dispatch = useAppDispatch();

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="tertiary" size="sm">
          <UserCircledIcon className="size-[24px]" />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent
        align="end"
        className="bg-neutral-0 p-0 shadow-card rounded border-none min-w-[216px]"
      >
        <DropdownMenuLabel className="flex-col items-start gap-[0px]">
          <Text variant="bodyBold">
            {user?.firstName || ""} {user?.lastName || ""}{" "}
            {user?.role ? `(${user.role})` : ""}
          </Text>
          <Text variant="caption">{user?.email}</Text>
        </DropdownMenuLabel>
        <DropdownMenuSeparator className="mx-[1.5rem]" />
        <DropdownMenuItem asChild>
          <Link to="#profile">
            <UserIcon className="size-[24px] mr-[1rem]" />
            <Text variant="bodyBold">My Profile</Text>
          </Link>
        </DropdownMenuItem>
        <DropdownMenuItem
          onClick={async () => {
            await dispatch(
              signout({ refreshToken: user?.tokens.refreshToken ?? "" }),
            ).unwrap();
          }}
        >
          <LogOutIcon className="size-[24px] mr-[1rem]" />
          <Text variant="bodyBold">Sign Out</Text>
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
};
