import React from "react";
import Header from "./Header";
import Sidebar from "./Sidebar";

const Layout = ({ children }) => {
  return (
    <>
      <div className="header">
        <Header />
      </div>
      <div className="row g-0">
        <div className="col-md-2 col-sm-3">
          <Sidebar />
        </div>
        <div className="col-md-10 col-sm-8">{children}</div>
      </div>
    </>
  );
};

export default Layout;
