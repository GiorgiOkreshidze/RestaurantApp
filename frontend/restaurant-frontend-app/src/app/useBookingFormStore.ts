import type { RichTimeSlot } from "@/types";
import { create } from "zustand";
import { TIME_SLOTS } from "@/utils/constants";
import { startOfTomorrow } from "date-fns";

interface FormStore {
  locationId: string;
  date: Date;
  time: string;
  guests: number;
  timeSlots: RichTimeSlot[];

  setLocationId: (locationId: FormStore["locationId"]) => void;
  setDate: (date: FormStore["date"]) => void;
  setTime: (time: FormStore["time"]) => void;
  increaseGuests: () => void;
  decreaseGuests: () => void;
}

export const useBookingFormStore = create<FormStore>((set) => ({
  locationId: "",
  date: TIME_SLOTS.filter((slot) => !slot.isPast).length
    ? new Date()
    : startOfTomorrow(),
  time: "",
  guests: 2,
  timeSlots: TIME_SLOTS,

  setLocationId: (locationId) => {
    set(() => ({ locationId }));
  },

  setDate: (date) => {
    set(() => ({ date }));
  },

  setTime: (time) => {
    set(() => ({ time }));
  },

  increaseGuests: () => {
    set((state) => ({ guests: state.guests < 10 ? state.guests + 1 : 10 }));
  },

  decreaseGuests: () => {
    set((state) => ({ guests: state.guests > 1 ? state.guests - 1 : 1 }));
  },
}));
