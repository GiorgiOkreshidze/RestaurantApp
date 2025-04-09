import * as React from "react";
import { MoreHorizontalIcon } from "lucide-react";

import { cn } from "@/lib/utils";
import { Button } from ".";
import { textVariants } from "@/components/variants/textVariants.ts";

function Pagination({ className, ...props }: React.ComponentProps<"nav">) {
  return (
    <nav
      role="navigation"
      aria-label="pagination"
      data-slot="pagination"
      className={cn("mx-auto flex w-full justify-center", className)}
      {...props}
    />
  );
}

function PaginationContent({
  className,
  ...props
}: React.ComponentProps<"ul">) {
  return (
    <ul
      data-slot="pagination-content"
      className={cn("flex flex-row items-center gap-4", className)}
      {...props}
    />
  );
}

function PaginationItem({ ...props }: React.ComponentProps<"li">) {
  return <li data-slot="pagination-item" {...props} />;
}

type PaginationLinkProps = {
  isActive?: boolean;
} & Pick<React.ComponentProps<typeof Button>, "size"> &
  React.ComponentProps<"a">;

function PaginationLink({
  className,
  isActive,
  ...props
}: PaginationLinkProps) {
  return (
    <a
      aria-current={isActive ? "page" : undefined}
      data-slot="pagination-link"
      data-active={isActive}
      className={cn(
        textVariants({ variant: "blockTitle" }),
        isActive
          ? "font-medium border-b-2 border-green-200"
          : "border-b-2 border-transparent",
        className,
      )}
      {...props}
    />
  );
}

function PaginationPrevious({
  className,
  disabled,
  onClick,
  ...props
}: React.ComponentProps<typeof PaginationLink> & { disabled?: boolean }) {
  return (
    <PaginationLink
      aria-label="Go to previous page"
      size="l"
      className={cn(
        "cursor-pointer gap-1 px-2.5 sm:pl-2.5",
        disabled ? "opacity-50 pointer-events-none" : "",
        className,
      )}
      href={disabled ? "#" : props.href}
      onClick={disabled ? undefined : onClick}
      {...props}
    >
      <span className="hidden sm:block">&lt;&lt;</span>
    </PaginationLink>
  );
}

function PaginationNext({
  className,
  disabled,
  onClick,
  ...props
}: React.ComponentProps<typeof PaginationLink> & { disabled?: boolean }) {
  return (
    <PaginationLink
      aria-label="Go to previous page"
      size="l"
      className={cn(
        "cursor-pointer gap-1 px-2.5 sm:pl-2.5",
        disabled ? "opacity-50 pointer-events-none" : "",
        className,
      )}
      href={disabled ? "#" : props.href}
      onClick={disabled ? undefined : onClick}
      {...props}
    >
      <span className="hidden sm:block">&gt;&gt;</span>
    </PaginationLink>
  );
}

function PaginationEllipsis({
  className,
  ...props
}: React.ComponentProps<"span">) {
  return (
    <span
      aria-hidden
      data-slot="pagination-ellipsis"
      className={cn("flex size-9 items-center justify-center", className)}
      {...props}
    >
      <MoreHorizontalIcon className="size-4" />
      <span className="sr-only">More pages</span>
    </span>
  );
}

export {
  Pagination,
  PaginationContent,
  PaginationLink,
  PaginationItem,
  PaginationPrevious,
  PaginationNext,
  PaginationEllipsis,
};
