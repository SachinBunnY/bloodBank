import React, { useEffect, useState } from "react";
import API from "../../services/API";

const PaymentHistory = () => {
  const [payments, setPayments] = useState([]);

  useEffect(() => {
    const fetchPayments = async () => {
      try {
        const response = await API.get("/payment/payment-history");
        console.log("Payment Data::", response?.data);
        setPayments(response?.data);
      } catch (e) {
        console.log("Error when call try to get payment history:", e);
      }
    };
    fetchPayments();
  }, []);

  return (
    <div>
      <h2>Payment History</h2>
      <ul>
        {payments.map((payment) => (
          <li key={payment.id}>
            {payment.orderId} - {payment.amount} - {payment.status}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default PaymentHistory;
