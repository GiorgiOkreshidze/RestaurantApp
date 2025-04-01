import { selectUser } from "@/app/slices/userSlice";
import { useSelector } from "react-redux";
import { Navigate } from "react-router";

export const PublicRoute: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const user = useSelector(selectUser);

  if (user) {
    return <Navigate to="/" />;
  }
  return children;
};
