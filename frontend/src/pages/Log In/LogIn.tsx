import { UserTile } from '../../components/usertile/UserTile';
import './LogIn.css'

export const LogIn = () => {
  return (
    <UserTile>
      <h1>Log In</h1>
      <div className="login-input-container">
        <label>Username</label>
        <input type="text"/>

        <label>Password</label>
        <input type="password" />    
        
        <button type="button">Log In</button>
      </div>
    </UserTile>
  );
};
  