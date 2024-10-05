import { UserTile } from '../../components/usertile/UserTile';
import './AccSettings.css'

import { useState } from 'react';

export const AccSettings = () => {

  const [usernameStatus, setUsernameStatus] = useState('Test Username');
  const [passwordStatus, setPasswordStatus] = useState('Test Password');
  const [image, setImage] = useState<string | null>(null); 

    return (
      <div className='user-tile-container'>
        <UserTile>
          <h1>Account Settings</h1>
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
            
            <button type="button">Change Password</button>
          </div>
        </UserTile>

        <UserTile>
        <h1>Profile Picture</h1>
        <div className="image-upload-container">
          {image ? (
            <img src={image as string} alt="Profile" className="profile-image" />
          ) : (
            <div className="no-image">No image uploaded</div>
          )}
          <input type="file" accept="image/*" />
          <button type="button">Upload Image</button>
        </div>
      </UserTile>
      </div>
    );
  };