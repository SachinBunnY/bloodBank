import React, { useEffect } from "react";
import { useDispatch } from "react-redux";
import API from "../../services/API";
import { Navigate } from "react-router-dom";
import { getCurrentUser } from "../../redux/features/auth/authActions";

const ProtectedRoute = ({ children }) => {
  const dispatch = useDispatch();

  //get current user
  const getUser = async () => {
    try {
      const token = localStorage.getItem("token");
      let { data } = await API.get("/auth/current-user", {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      if (data?.success) {
        dispatch(getCurrentUser(data.user));
      }
    } catch (error) {
      localStorage.clear();
      console.log("ERROR", error);
    }
  };

  useEffect(() => {
    getUser();
  }, []);

  if (localStorage.getItem("token")) {
    return children;
  } else {
    return <Navigate to="/login" />;
  }
};

export default ProtectedRoute;
