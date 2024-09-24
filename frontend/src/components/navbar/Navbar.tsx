import React from 'react';
import { Link } from 'react-router-dom';
import './Navbar.css';

const Navbar: React.FC = () => {
  return (
    <nav className="navbar">
      <ul className="nav-left">
        <li className="nav-item">
          <Link to="/" className="nav-link">
            LifeCouldBeDream
          </Link>
        </li>
      </ul>
      <ul className="nav-right">
        <li className="nav-item">
          <Link to="/leaderboard" className="nav-link">
            LEADERBOARD
          </Link>
        </li>
        <li className="nav-item">
          <Link to="/signup" className="nav-link">
            SIGN UP
          </Link>
        </li>
        <li className="nav-item">
          <Link to="/login" className="nav-link">
            LOG IN
          </Link>
        </li>
      </ul>
    </nav>
  );
};

export default Navbar;
