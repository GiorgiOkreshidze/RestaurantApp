import { format, parse } from "date-fns";

export const dateObjToDateStringServer = (date: Date): string => {
  return format(date, "yyyy-MM-dd");
};

export const dateObjToDateStringUI = (date: Date) => {
  return format(date, "PP");
};

export const dateStringServerToDateObject = (date: string) => {
  return parse(date, "yyyy-MM-dd", new Date());
};

export const timeString24hToDateObj = (time: string) => {
  return parse(time, "HH:mm", new Date());
};

export const timeString24hToTimeString12h = (time: string) => {
  const dateObj = timeString24hToDateObj(time);
  return format(dateObj, "h:mm aaaa");
};
