import React from "react";
import { useNavigate } from "react-router-dom";
import "./HomeButton.css";

const HomeButton: React.FC = () => {
  const navigate = useNavigate();

  return (
    <button className="home-button" onClick={() => navigate("/")}>
      Go home
    </button>
  );
};

export default HomeButton;
