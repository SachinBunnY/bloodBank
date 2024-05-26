import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { ProgressBar } from "react-loader-spinner";
import Layout from "../components/shared/Layout/Layout";
import Modal from "../components/shared/modal/Modal";
import API from "../services/API";
import moment from "moment";
import PaymentModal from "../pages/RazorpayModal";

const HomePage = () => {
  const [data, setData] = useState([]);
  const { loading, error, user } = useSelector((state) => state.auth);
  const navigate = useNavigate();
  const [isModalOpen, setIsModalOpen] = useState(false);

  const getBloodRecords = async () => {
    try {
      const { data } = await API.get("/inventory/get-inventory");
      if (data?.success) {
        setData(data?.inventory);
      }
    } catch (error) {
      console.log(error);
    }
  };

  useEffect(() => {
    getBloodRecords();
  }, []);

  return (
    <Layout>
      {user?.role === "admin" && navigate("/admin")}
      {error && <span>{alert(error)}</span>}
      {loading ? (
        <div className="d-flex justify-content-center align-items-center">
          <ProgressBar
            visible={true}
            height="200"
            width="200"
            color="#4fa94d"
            ariaLabel="progress-bar-loading"
            wrapperStyle={{}}
            wrapperClass=""
          />
        </div>
      ) : (
        <>
          <h4
            className="ms-4"
            data-bs-toggle="modal"
            data-bs-target="#staticBackdrop"
            style={{ cursor: "pointer" }}
          >
            <i className="fa-regular fa-square-plus text-success py-4"></i>
            &nbsp;Add To Inventory
          </h4>

          <div className="container m-3">
            <table className="table" style={{ width: "100%" }}>
              <thead>
                <tr>
                  <th scope="col">Blood Groud</th>
                  <th scope="col">InventoryType</th>
                  <th scope="col">Quantity</th>
                  <th scope="col">Donar Email</th>
                  <th scope="col">Time Date</th>
                  <th scope="col">Action</th>
                </tr>
              </thead>
              <tbody>
                {data?.map((record) => (
                  <tr
                    className={
                      record.inventoryType.toLowerCase() === "in"
                        ? "table-success"
                        : "table-danger"
                    }
                    key={record._id}
                  >
                    <td>{record.bloodGroup}</td>
                    <td>{record.inventoryType.toUpperCase()}</td>
                    <td>{record.quantity} ml</td>
                    <td>{record.email}</td>
                    <td>
                      {moment(record.createdAt).format("DD/MM/YYYY hh:mm A")}
                    </td>
                    <td>
                      <button
                        type="button"
                        class="btn btn-secondary"
                        onClick={() => setIsModalOpen(true)}
                      >
                        Buy Blood
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <PaymentModal
            isOpen={isModalOpen}
            onRequestClose={() => setIsModalOpen(false)}
            setIsModalOpen={setIsModalOpen}
          />
          <Modal />
        </>
      )}
    </Layout>
  );
};

export default HomePage;
