import React, { useState } from "react";
import Form from "../../components/shared/Form/Form";
import { useSelector } from "react-redux";
import { DNA } from "react-loader-spinner";
import toast from "react-hot-toast";

const Login = () => {
  const { loading, error } = useSelector((state) => state.auth);
  return (
    <>
      {error && <span>{toast.error(error)}</span>}
      <div style={{ padding: "0 1rem 0 1rem" }}>
        <div className=" vh-100">
          <nav
            className="navbar"
            style={{ maxHeight: "3.5rem", marginBottom: "0.5rem" }}
          >
            <div
              className="text-center mb-4"
              style={{ width: "100%", height: "4rem" }}
            >
              <img
                src="./assets/BloodLogo.png"
                className="logo-login"
                alt="Logo"
                style={{ margin: "0 0 1rem 1rem", float: "left" }}
              />
            </div>
          </nav>

          <div
            style={{
              display: "flex",
              flexDirection: "row",
              width: "100%",
              height: "97vh",
            }}
          >
            <div style={{ width: "65%" }}>
              <img
                src="./assets/loginImage.png"
                alt="Logo"
                style={{ height: "34rem", width: "100%" }}
              />
            </div>
            <div style={{ width: "35%", position: "relative" }}>
              <div
                className="card p-4"
                style={{ position: "absolute", width: "100%" }}
              >
                {loading ? (
                  <div className="d-flex justify-content-center">
                    <DNA
                      visible={true}
                      height={80}
                      width={80}
                      ariaLabel="dna-loading"
                    />
                  </div>
                ) : (
                  <Form
                    formTitle={"Log In"}
                    submitBtn={"Login"}
                    formType={"login"}
                  />
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default Login;
