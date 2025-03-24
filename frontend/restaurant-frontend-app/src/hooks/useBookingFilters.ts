import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";
// import { useAppDispatch } from "@/app/hooks";
import { useNavigate } from "react-router";

export const useBookingFilters = () => {
  // const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const formSchema = z.object({
    location: z.string(),
  });

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      location: "",
    },
    mode: "all",
    criteriaMode: "all",
  });

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    try {
      // const result = await dispatch(register(values)).unwrap();
      // console.log("Registration successful:", result);
      navigate("/signin");
    } catch (error) {
      console.error("Registration failed:", error);
    }
  };

  return { form, onSubmit, formSchema };
};
