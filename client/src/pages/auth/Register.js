import React from "react";
import Form from "../../components/shared/Form/Form";
import { useSelector } from "react-redux";
import { DNA } from "react-loader-spinner";

const Register = () => {
  const { loading, error } = useSelector((state) => state.auth);
  return (
    <>
      {error && <span>{alert(error)}</span>}
      <div
        className="navbar navbar-light bg-light"
        style={{ maxHeight: "3.5rem", marginBottom: "0.5rem" }}
      >
        <div
          className="text-center"
          style={{ width: "100%", height: "3.5rem", marginTop: "-0.5rem" }}
        >
          <img
            src="./assets/BloodLogo.png"
            className="logo-login"
            alt="Logo"
            style={{ margin: "0 0 1rem 1rem", float: "left" }}
          />
        </div>
      </div>
      <div className="container-fluid" style={{ height: "calc(100vh - 6rem)" }}>
        <div className="row">
          <div className="col-md-8 bg-light" style={{ height: "33rem" }}>
            <img
              src="./assets/banner2.jpg"
              alt="Login image"
              className="img-fluid"
              style={{ height: "34rem", width: "100%" }}
            />
          </div>
          <div
            className="col-md-4 d-flex align-items-center"
            style={{ position: "relative" }}
          >
            <div
              className="card"
              style={{
                position: "absolute",
                height: "34rem",
                marginTop: "1rem",
              }}
            >
              <div className="card-body">
                {loading ? (
                  <div className="d-flex justify-content-center">
                    <DNA visible={true} height={80} width={80} />
                  </div>
                ) : (
                  <Form
                    formTitle={"Register"}
                    submitBtn={"Register"}
                    formType={"register"}
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

export default Register;
