import './SingIn.css';
import { UserTile } from '../../components/usertile/UserTile';

  
export const SignIn = () => {
  return (
    <UserTile>
      <h1>Sign In</h1>
      <div className="signin-input-container">
        <label>Username</label>
        <input type="text"/>

        <div className="password-container">
          <label>Password</label>
          <input type="password" />
          <label>Repeat Password</label>
          <input type="password"/>
        </div>
        
        <button type="button">Register</button>
      </div>
    </UserTile>
  );
};
  