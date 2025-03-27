import { format, parse } from "date-fns";

export const timeStringFrom24hTo12h = (time: string) => {
  return format(parse(time, "HH:mm", new Date()), "h:mm aaaa");
};

export const dateObjectToYYYY_MM_DD = (date: Date) => {
  return format(date.toUTCString(), "yyyy-MM-dd");
};

export const convertDateToUIFormat = (date: string) => {
  const dateObject = parse(date, "yyyy-MM-dd", new Date());
  const newDate = format(dateObject, "PP");
  return newDate;
};
