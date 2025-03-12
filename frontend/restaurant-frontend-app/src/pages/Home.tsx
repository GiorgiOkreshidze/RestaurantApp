import { Link } from "react-router";

export const Home = () => {
  return (
    <div>
      <h1 className="text-9xl">Home page</h1>
      <Link to="/registration" className="text-9xl text-red">
        Register
      </Link>
    </div>
  );
};
