import { selectUser } from "@/app/slices/userSlice";
import { useSelector } from "react-redux";
import { Link } from "react-router";

export const Home = () => {
  const user = useSelector(selectUser);

  return (
    <div>
      <h1 className="text-9xl">Home page</h1>
      <Link to="/signup" className="text-9xl text-red">
        Register
      </Link>
      <h3>
        Hello,{" "}
        <span className="text-red">
          {user?.name} {user?.lastName}
        </span>
      </h3>
    </div>
  );
};
