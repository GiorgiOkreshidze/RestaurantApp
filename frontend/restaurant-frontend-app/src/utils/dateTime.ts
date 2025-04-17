import { format, parse } from "date-fns";

export const parseDateFromServer = (date: string) => {
  return parse(date, "yyyy-MM-dd", new Date()).toString();
};

export const parseTimeFromServer = (time: string) => {
  return parse(time, "HH:mm", new Date()).toString();
};

export const formatDateToServer = (date: string) => {
  const dateObj = new Date(date);
  return format(dateObj, "yyyy-MM-dd");
};

export const formatDateToUI = (date: string) => {
  const dateObj = new Date(date);
  return format(dateObj, "PP");
};

export const formatTimeToUI = (time: string) => {
  const dateObj = parseTimeFromServer(time);
  return format(dateObj, "h:mm aaaa");
};