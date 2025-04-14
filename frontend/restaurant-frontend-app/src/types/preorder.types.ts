import { Dish } from "@/types";

export interface Preorder {
  id: string;
  status: "submitted" | "new";
  dishes: { id: Dish["id"]; count: number }[];
  number: number;
}
