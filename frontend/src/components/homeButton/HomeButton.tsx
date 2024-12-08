import React from "react";
import { useNavigate } from "react-router-dom";
import "./HomeButton.css";

const HomeButton: React.FC = () => {
  const navigate = useNavigate();

  return (
    <button className="home-button" onClick={() => navigate("/")}>
      Back to Home page
    </button>
  );
};

export default HomeButton;
