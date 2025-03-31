import { format, parse } from "date-fns";

export const dateObjToDateStringServer = (date: string): string => {
  const dateObj = new Date(date);
  return format(dateObj, "yyyy-MM-dd");
};

export const dateObjToDateStringUI = (date: string) => {
  const dateObj = new Date(date);
  return format(dateObj, "PP");
};

export const dateStringServerToDateObject = (date: string) => {
  return parse(date, "yyyy-MM-dd", new Date()).toString();
};

export const timeString24hToDateObj = (time: string) => {
  return parse(time, "HH:mm", new Date()).toString();
};

export const timeString24hToTimeString12h = (time: string) => {
  const dateObj = timeString24hToDateObj(time);
  return format(dateObj, "h:mm aaaa");
};
