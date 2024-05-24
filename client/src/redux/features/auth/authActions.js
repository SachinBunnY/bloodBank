import { createAsyncThunk } from "@reduxjs/toolkit";
import API from "../../../services/API";
import toast from "react-hot-toast";

export const userLogin = createAsyncThunk(
  "auth/login",
  async ({ role, email, password }, { rejectWithValue }) => {
    try {
      const { data } = await API.post("/auth/login", { role, email, password });
      console.log("AFTER LOGIN DATA:", data);
      //       AFTER LOGIN DATA:
      // Object { success: true, token: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI3IiwiZW1haWwiOiJzYWNoaW5AZ21haWwuY29tIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzE2NDgyMzMyLCJleHAiOjE3MTcwODcxMzIsImlhdCI6MTcxNjQ4MjMzMn0.VnuNfWPRiouxGj-a56YVBBOuCqdYpJQrpZpBNwskeFQ", message: "Login successful" }

      if (data.success) {
        localStorage.setItem("token", data.token);
        toast.success(data.message);
        window.location.replace("/");
      }
      return data;
    } catch (error) {
      if (error.response && error.response.data.message) {
        return rejectWithValue(error.response.data.message);
      } else {
        return rejectWithValue(error.message);
      }
    }
  }
);

export const userRegister = createAsyncThunk(
  "auth/register",
  async (
    {
      name,
      role,
      email,
      password,
      organisationName,
      hospitalName,
      website,
      address,
      phone,
    },
    { rejectWithValue }
  ) => {
    try {
      const { data } = await API.post("auth/register", {
        name,
        role,
        email,
        password,
        organisationName,
        hospitalName,
        website,
        address,
        phone,
      });

      if (data.success) {
        toast.success(data.message);
        window.location.replace("/login");
      } else {
        toast.error(data.message, { position: "top-left" });
      }
    } catch (error) {
      if (error.response && error.response.data.message) {
        toast.error(error.response.data.message);
        return rejectWithValue(error.response.data.message);
      } else {
        toast.error(error.response.data.message);
        return rejectWithValue(error.message);
      }
    }
  }
);

export const getCurrentUser = createAsyncThunk(
  "auth/getCurrentUser",
  async ({ rejectWithValue }) => {
    try {
      console.log("SECOND CALL");
      const res = await API.get("/auth/current-user");
      console.log("SECOND DATA::", res);
      if (res?.data) {
        return res?.data;
      }
    } catch (error) {
      if (error.response && error.response.data.message) {
        toast.error(error.response.data.message);
        return rejectWithValue(error.response.data.message);
      } else {
        toast.error(error.response.data.message);
        return rejectWithValue(error.message);
      }
    }
  }
);
