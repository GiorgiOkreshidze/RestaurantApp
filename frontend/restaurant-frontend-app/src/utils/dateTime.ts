import { format, parse } from "date-fns";

export const timeStringFrom24hTo12h = (time: string) => {
  return format(parse(time, "HH:mm", new Date()), "h:mm aaaa");
};

export const dateObjectToYYYY_MM_DD = (date: Date) => {
  return format(date.toUTCString(), "yyyy-MM-dd");
};

export const dateObjectToHH_MM = (date: Date) => {
  console.log(date);
  const newDate = format(date.toUTCString(), "HH:mm");
  console.log(newDate);
  return newDate;
};
