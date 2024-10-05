import './SingIn.css';
import { UserTile } from '../../components/usertile/UserTile';

import { useState } from 'react';
  
export const SignIn = () => {
  const [usernameStatus, setUsernameStatus] = useState('Username is valid');
  const [passwordStatus, setPasswordStatus] = useState('Passwords do not match');
  
  return (
    <UserTile>
      <h1>Sign In</h1>
      <div className="signin-input-container">
        <label>Username</label>
        <input type="text"/>
        <div className="status-message">{usernameStatus}</div>

        <div className="password-container">
          <label>Password</label>
          <input type="password" />
          <label>Repeat Password</label>
          <input type="password"/>
          <div className="status-message">{passwordStatus}</div>
        </div>
        
        <button type="button">Register</button>
      </div>
    </UserTile>
  );
};
  