import React, { useEffect, useState } from "react";
import Layout from "./../../components/shared/Layout/Layout";
import moment from "moment";
import API from "../../services/API";
import { ProgressBar } from "react-loader-spinner";

const DonarList = () => {
  const [data, setData] = useState([]);
  const [temp, setTemp] = useState(false);
  //find donar records
  const getDonars = async () => {
    try {
      const { data } = await API.get("/inventory/get-inventory");
      console.log("DATA:", data?.inventory);
      if (data?.success) {
        setData(data?.inventory);
      }
    } catch (error) {
      console.log(error);
    } finally {
      setTemp(false);
    }
  };

  useEffect(() => {
    setTemp(true);
    getDonars();
  }, []);

  //DELETE FUNCTION
  const handelDelete = async (email) => {
    console.log("EMAIL FOR DELETE::", email);
    try {
      let answer = window.prompt(
        "Are You Sure Want To Delete This Donor",
        "Sure"
      );
      if (!answer) return;
      const { data } = await API.delete(`/auth/delete-donor/${email}`);
      alert(data?.message);
      window.location.reload();
    } catch (error) {
      console.log(error);
    }
  };

  return (
    <Layout>
      {temp ? (
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
        <div className="container">
          <table className="table mt-3">
            <thead>
              <tr className="table-active">
                <th scope="col">Name</th>
                <th scope="col">Email</th>
                <th scope="col">Group</th>
                <th scope="col">Quantity</th>
                <th scope="col">Date</th>
                <th scope="col">Action</th>
              </tr>
            </thead>
            <tbody>
              {data?.map((record) => (
                <tr key={record.id}>
                  <td>{String(record?.email?.split("@")[0]).toUpperCase()}</td>
                  <td>{record.email}</td>
                  <td>{record.bloodGroup}</td>
                  <td>{record.quantity}</td>
                  <td>
                    {moment(record.createdAt).format("DD/MM/YYYY hh:mm A")}
                  </td>
                  <td>
                    <button
                      className="btn btn-danger"
                      onClick={() => handelDelete(record?.email)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </Layout>
  );
};

export default DonarList;
