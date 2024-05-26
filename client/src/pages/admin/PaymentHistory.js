import React, { useEffect, useState } from "react";

const PaymentHistory = () => {
  const [payments, setPayments] = useState([]);

  useEffect(() => {
    const fetchPayments = async () => {
      const response = await fetch("/api/payment/payment-history");
      const data = await response.json();
      setPayments(data);
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
