import React, { useEffect, useState } from "react";
import Layout from "../../components/shared/Layout/Layout";
import moment from "moment";
import API from "../../services/API";
import { ProgressBar } from "react-loader-spinner";

const Report = () => {
  const [record, setRecords] = useState([]);
  const [temp, setTemp] = useState(false);
  useEffect(() => {
    const fetchPayments = async () => {
      try {
        const response = await API.get("/payment/payment-history");
        console.log("Payment Data::", response?.data);
        setRecords(response?.data);
      } catch (e) {
        console.log("Error when call try to get payment history:", e);
      } finally {
        setTemp(false);
      }
    };
    setTemp(true);
    fetchPayments();
  }, []);

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
        <div
          className="container mt-3"
          style={{ maxHeight: "100vh", overflowY: "scroll" }}
        >
          <table className="table ">
            <thead>
              <tr className="table-active">
                <th scope="col">Payment ID</th>
                <th scope="col">Customer details</th>
                <th scope="col">Created on</th>
                <th scope="col">Amount</th>
                <th scope="col">Blood Owner</th>
                <th scope="col">Group</th>
                <th scope="col">Quantity</th>
                <th scope="col">Status</th>
              </tr>
            </thead>
            <tbody>
              {record.length > 0 &&
                record?.map((item) => (
                  <tr key={item?.id}>
                    {console.log(String(item?.orderId))}
                    <td>{String(item?.orderId)}</td>
                    <td>{item?.contact}</td>
                    <td>
                      {moment(item?.paymentDate).format("DD/MM/YYYY hh:mm A")}
                    </td>
                    <td>{item?.amount}</td>
                    <td>{item?.bloodOwner}</td>
                    <td>{item?.bloodGroup}</td>
                    <td>{item?.bloodQuantity}</td>
                    <td>{item?.status}</td>
                  </tr>
                ))}
            </tbody>
          </table>
        </div>
      )}
    </Layout>
  );
};

export default Report;

// amount: 10;
// bankRRN: null;
// bloodGroup: "O+";
// bloodOwner: "lathish@gmail.com";
// bloodQuantity: "200";
// contact: "7003885384";
// email: "sachin@gmail.com";
// id: 14;
// orderId: "order_OFclZxJSljJdbX";
// paymentDate: "2024-05-27T18:55:02.872139";
// paymentId: "pay_OFcmKIuSAym2Q7";
// signature: "1222a907be660d4028cadb2e85f9741bbef88cc4930466fb134e1c5544859c6c";
// status: "Success";
