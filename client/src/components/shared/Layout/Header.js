import React, { useEffect, useState } from "react";
import { MdOutlineBloodtype } from "react-icons/md";
import { HiOutlineLogout } from "react-icons/hi";
import { FaUserAlt } from "react-icons/fa";
import { useSelector, useDispatch } from "react-redux";
import { Link, useLocation } from "react-router-dom";
import Modal from "react-modal";
import "./Header.css";
import API from "../../../services/API";

const Header = () => {
  const dispatch = useDispatch();
  const { user } = useSelector((state) => state.auth);
  const location = useLocation();
  const [isOpen, setIsModalOpen] = useState(false);
  const [change, setChange] = useState(false);
  const [updatedDetails, setUpdatedDetails] = useState({});

  const getUser = async () => {
    try {
      const token = localStorage.getItem("token");
      let { data } = await API.get("/auth/current-user", {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      if (data?.success) {
        dispatch(data.user);
      }
    } catch (error) {
      localStorage.clear();
      console.log("ERROR", error);
    }
  };

  useEffect(() => {
    // getUser();
    setUpdatedDetails({
      name: user?.name,
      email: user?.email,
      password: "****",
      address: user?.address,
      phone: user?.phone,
    });
  }, []);

  //logout handler
  const handleLogout = () => {
    localStorage.clear();
    alert("Logout Successful !");
    window.location.reload();
  };

  const updateUserProfile = (e) => {
    setUpdatedDetails({
      ...updatedDetails,
      [e?.target?.name]: e?.target?.value,
    });
  };

  function handleEdit() {
    setUpdatedDetails({
      name: user?.name,
      email: user?.email,
      password: "****",
      address: user?.address,
      phone: user?.phone,
    });
    setChange(true);
  }

  async function handleSubmitChanges() {
    console.log("UPDATED DETAILS::", updatedDetails);
    try {
      const { data } = await API.put("/auth/update-profile", {
        name: updatedDetails?.name,
        email: updatedDetails?.email,
        password: updatedDetails?.password,
        address: updatedDetails?.address,
        phone: updatedDetails?.phone,
      });
      if (data?.value?.success) {
        alert("Data updated successfully !");
        window.location.reload();
      }
    } catch (error) {
      alert("Updation failed !");
      console.log(error);
    }
    handleCancel();
  }

  function handleCancel() {
    setUpdatedDetails({
      name: user?.name,
      email: user?.email,
      password: "****",
      address: user?.address,
      phone: user?.phone,
    });
    setChange(false);
  }

  return (
    <>
      <nav className="navbar">
        <div className="container-fluid">
          <div className="navbar-brand h1">
            <MdOutlineBloodtype color="red" />
            BLOOD <span className="gold">BANK</span>
          </div>
          <ul className="navbar-nav flex-row">
            <li className="nav-item mx-3">
              <p className="nav-link" onClick={() => setIsModalOpen(true)}>
                <FaUserAlt /> Welcome{" "}
                <span className="gold">
                  {" "}
                  {user?.name ||
                    user?.hospitalName ||
                    user?.organisationName}{" "}
                  &nbsp;
                </span>
                <span className="badge bg-secondary">{user?.role}</span>
              </p>
            </li>
            {location.pathname === "/" ||
            location.pathname === "/donar" ||
            location.pathname === "/hospital" ? (
              <li className="nav-item mx-3">
                <Link to="/analytics" className="nav-link">
                  Analytics
                </Link>
              </li>
            ) : (
              <li className="nav-item mx-3">
                <Link to="/" className="nav-link">
                  Home
                </Link>
              </li>
            )}
            <li className="nav-item mx-3">
              <button className="btn btn-danger" onClick={handleLogout}>
                {" "}
                <HiOutlineLogout color="white" /> Logout
              </button>
            </li>
          </ul>
        </div>
      </nav>
      <Modal isOpen={isOpen} onRequestClose={() => setIsModalOpen(false)}>
        <h2>User Profile</h2>
        {!change ? (
          <>
            <div className="inputDiv">
              <div className="mb-3 ">
                <label className="textStyle">
                  Name: &nbsp;
                  <span className="spanStyle">{user?.name}</span>
                </label>
              </div>
              <div className="mb-3">
                <label className="textStyle">
                  Email: &nbsp;
                  <span className="spanStyle">{user?.email}</span>
                </label>
              </div>
            </div>
            <div className="inputDiv">
              <div className="mb-3">
                <label className="textStyle">
                  Password: &nbsp;
                  <span className="spanStyle">*****</span>
                </label>
              </div>
              <div className="mb-3">
                <label className="textStyle">
                  Contact: &nbsp;
                  <span className="spanStyle">{user?.phone}</span>
                </label>
              </div>
            </div>
            <div className="inputDiv">
              <div className="mb-3">
                <label className="textStyle">
                  Address: &nbsp;
                  <span className="spanStyle">{user?.address}</span>
                </label>
              </div>
            </div>
            <button
              style={{ float: "right" }}
              className="btn btn-primary"
              onClick={handleEdit}
            >
              Edit
            </button>
          </>
        ) : (
          <form
            onSubmit={(e) => {
              e.preventDefault();
            }}
          >
            <div className="inputDiv">
              <div className="mb-3 ">
                <label className="form-label">
                  Name:
                  <input
                    className="form-control"
                    type="text"
                    name="name"
                    value={updatedDetails?.name}
                    onChange={updateUserProfile}
                    required
                  />
                </label>
              </div>
              <div className="mb-3">
                <label className="form-label">
                  Email:
                  <input
                    className="form-control"
                    type="email"
                    name="email"
                    value={updatedDetails?.email}
                    onChange={updateUserProfile}
                    required
                  />
                </label>
              </div>
            </div>
            <div className="inputDiv">
              <div className="mb-3">
                <label className="form-label">
                  Password:
                  <input
                    className="form-control"
                    type="password"
                    name="password"
                    value={updatedDetails?.password}
                    onChange={updateUserProfile}
                    required
                  />
                </label>
              </div>
              <div className="mb-3">
                <label className="form-label">
                  Contact:
                  <input
                    className="form-control"
                    type="text"
                    name="phone"
                    value={updatedDetails?.phone}
                    onChange={updateUserProfile}
                    required
                  />
                </label>
              </div>
            </div>
            <div className="inputDiv">
              <div className="mb-3">
                <label className="form-label">
                  Address:
                  <input
                    className="form-control"
                    type="text"
                    name="address"
                    value={updatedDetails?.address}
                    onChange={updateUserProfile}
                    required
                  />
                </label>
              </div>
            </div>

            <div className="btnDivProfile">
              <button
                type="submit"
                className="btn btn-success"
                onClick={() => handleSubmitChanges()}
              >
                Submit
              </button>
              <button
                type="button"
                className="btn btn-secondary"
                onClick={() => handleCancel()}
              >
                Cancel
              </button>
            </div>
          </form>
        )}
      </Modal>
    </>
  );
};

export default Header;
