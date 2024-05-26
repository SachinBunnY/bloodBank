import React from "react";
import Layout from "../../components/shared/Layout/Layout";
import { useSelector } from "react-redux";
import PaymentHistory from "../admin/PaymentHistory";

const AdminHome = () => {
  const { user } = useSelector((state) => state.auth);
  return (
    <Layout>
      <div className="container p-3">
        <div className="d-felx flex-column mt-4">
          <h1>
            Welcome Admin <i className="text-success">{user?.name}</i>
          </h1>
          <h3 className="mt-4">Manage Blood Bank App </h3>
          <hr />
          <PaymentHistory />
        </div>
      </div>
    </Layout>
  );
};

export default AdminHome;
