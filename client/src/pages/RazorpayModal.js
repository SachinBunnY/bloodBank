import React, { useState } from "react";
import Modal from "react-modal";
import API from "../services/API";
import "./RazorpayModal.css";

const PaymentModal = ({ isOpen, onRequestClose, setIsModalOpen }) => {
  const [paymentDetails, setPaymentDetails] = useState({
    name: "",
    email: "",
    phone: "",
    amount: 0,
  });

  const handleChange = (e) => {
    setPaymentDetails({
      ...paymentDetails,
      [e.target.name]: e.target.value,
    });
  };

  console.log("paymentDetails::", paymentDetails);

  const handlePaymentSubmit = async () => {
    try {
      // Create order on the server
      console.log("CALLED::", paymentDetails);
      const orderResponse = await API.post("/auth/create-order", {
        amount: paymentDetails?.amount,
      });

      const order = orderResponse.data;
      console.log("ORDER DATA::", order);
      // Configure Razorpay options
      const options = {
        key: "rzp_test_XucVmb1X3bzatG",
        amount: order.amount,
        currency: "INR",
        name: "Blood Bank",
        description: "Blood order payment",
        order_id: order.id,
        handler: async (response) => {
          console.log("RESPONSE::", response);
          const verificationResponse = await API.post("/auth/verify-payment", {
            razorpayOrderId: response.razorpay_order_id,
            razorpayPaymentId: response.razorpay_payment_id,
            razorpaySignature: response.razorpay_signature,
          });

          console.log(":verificationResponse:::", verificationResponse);

          if (verificationResponse.data.success) {
            // Payment was successful
            setPaymentDetails({
              name: "",
              email: "",
              phone: "",
              amount: 0,
            });
            setIsModalOpen(false);
            alert("Payment successful:", verificationResponse.data);
          } else {
            // Payment verification failed
            alert("Payment verification failed:", verificationResponse.data);
          }
        },
        prefill: {
          name: paymentDetails.name,
          email: paymentDetails.email,
          contact: paymentDetails.phone,
        },
        theme: {
          color: "#3399cc",
        },
      };

      const razorpay = new window.Razorpay(options);
      razorpay.open();
    } catch (error) {
      console.error("Payment error:", error);
    }
  };

  // keyId=rzp_test_XucVmb1X3bzatG
  // SecretKey = EY9lLopN5mzOsIPnT42OxNHI;

  function handleCancel() {
    setPaymentDetails({
      name: "",
      email: "",
      phone: "",
      amount: 0,
    });
    onRequestClose();
  }

  return (
    <Modal isOpen={isOpen} onRequestClose={onRequestClose}>
      <h2>Make Payment</h2>
      <form
        onSubmit={(e) => {
          e.preventDefault();
        }}
      >
        <div className="inputDiv">
          <div className="mb-3 ">
            <label className="form-label">
              Amount:
              <input
                className="form-control"
                type="number"
                name="amount"
                value={paymentDetails.amount}
                onChange={(e) => handleChange(e)}
                required
              />
            </label>
          </div>
          <div className="mb-3">
            <label className="form-label">
              Name:
              <input
                className="form-control"
                type="text"
                name="name"
                value={paymentDetails.name}
                onChange={handleChange}
                required
              />
            </label>
          </div>
        </div>
        <div className="inputDiv">
          <div className="mb-3">
            <label className="form-label">
              Email:
              <input
                className="form-control"
                type="email"
                name="email"
                value={paymentDetails.email}
                onChange={handleChange}
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
                value={paymentDetails.phone}
                onChange={handleChange}
                required
              />
            </label>
          </div>
        </div>

        <div className="btnDiv">
          <button
            type="submit"
            className="btn btn-success"
            onClick={() => handlePaymentSubmit()}
          >
            Pay
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
    </Modal>
  );
};

export default PaymentModal;
