import { useEffect, useState } from 'react';
import { UserTile } from '../../components/usertile/UserTile';
import './AccSettings.css'

export const AccSettings = () => {
  const [username, setUsername] = useState<string | null>('');
  const [password] = useState('');
  const [repeatPassword] = useState('');
  const [passwordStatus] = useState(''); 
  const [image, setImage] = useState<string | null>(null);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [uploadStatus, setUploadStatus] = useState<string>('');
  const [userId, setUserId] = useState<number | null>(null);

  // Fetch the userId and username from localStorage when the component loads
  useEffect(() => {
    const storedUserId = localStorage.getItem('userId');
    const storedUsername = localStorage.getItem('username');
    if (storedUserId) {
      setUserId(Number(storedUserId));
      setUsername(storedUsername);
      fetchProfileImage(Number(storedUserId));
    }
  }, []);

  // Fetch the profile image for the logged-in user
  const fetchProfileImage = async (userId: number) => {
    try {
      const response = await fetch(`http://localhost:5217/api/user/${userId}/profile-image`);
      if (response.ok) {
        const imageBlob = await response.blob();
        const imageUrl = URL.createObjectURL(imageBlob);
        setImage(imageUrl);
      }
    } catch (error) {
      console.error('Error fetching profile image:', error);
    }
  };

  // Handle file selection
  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      setSelectedFile(file);
      const imageUrl = URL.createObjectURL(file);
      setImage(imageUrl);
    }
  };

  // Handle file upload
  const handleUpload = async () => {
    if (!selectedFile || !userId) {
      setUploadStatus("No file selected or user not logged in.");
      return;
    }

    const formData = new FormData();
    formData.append("profileImage", selectedFile);

    try {
      const response = await fetch(`http://localhost:5217/api/user/upload-profile-image/${userId}`, {
        method: "POST",
        body: formData,
      });

      if (response.ok) {
        setUploadStatus("Image uploaded successfully!");
        fetchProfileImage(userId); 
      } else {
        setUploadStatus("Failed to upload image.");
      }
    } catch (error) {
      setUploadStatus("Error occurred while uploading.");
    }
  };


  return (
    <div className='user-tile-container'>
      <UserTile>
        <h1>Account Settings</h1>
        <div className="signin-input-container">
          <label>Username</label>
          <div className='username-display'>
          <p>{username}</p> {}
          </div>
          <div className="password-container">
            <label>Password</label>
            <input type="password" value={password} />
            <label>Repeat Password</label>
            <input type="password" value={repeatPassword} />
            <div className="status-message">{passwordStatus}</div> {}
          </div>
          
          <button type="button">Change Password</button>
        </div>
      </UserTile>

      <UserTile>
        <h1>Profile Picture</h1>
        <div className="image-upload-container">
          {image ? (
              <img src={image} alt="Profile" className="profile-image" />
            ) : (
              <div className="no-image">No image uploaded</div>
          )}
          <input type="file" accept="image/*" onChange={handleFileChange} />
          <button type="button" onClick={handleUpload}>Upload Image</button>
          <div className="image-status-message">{uploadStatus}</div>
        </div>
      </UserTile>
    </div>
  );
};
