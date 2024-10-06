import { useEffect, useState } from 'react';
import { UserTile } from '../../components/usertile/UserTile';
import './AccSettings.css'

export const AccSettings = () => {

  const [usernameStatus, setUsernameStatus] = useState('Test Username');
  const [passwordStatus, setPasswordStatus] = useState('Test Password');
  const [image, setImage] = useState<string | null>(null); 
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [uploadStatus, setUploadStatus] = useState<string>('');

   // Fetch the profile image when the component loads
   useEffect(() => {
    const fetchProfileImage = async () => {
      try {
        const response = await fetch(`/api/user/1/profile-image`); // Replace 1 with the actual user ID
        if (response.ok) {
          const imageBlob = await response.blob();
          const imageUrl = URL.createObjectURL(imageBlob);
          setImage(imageUrl);
        }
      } catch (error) {
        console.error('Error fetching profile image:', error);
      }
    };
    fetchProfileImage();
  }, []);

  // Handle file selection
  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      setSelectedFile(file);
      const imageUrl = URL.createObjectURL(file);
      setImage(imageUrl);  // Display a preview of the image
    }
  };

  // Handle file upload
  const handleUpload = async () => {
    if (!selectedFile) {
      setUploadStatus("No file selected.");
      return;
    }

    const formData = new FormData();
    formData.append("profileImage", selectedFile);

    try {
      const response = await fetch(`/api/user/upload-profile-image/1`, { // Replace 1 with the actual user ID
        method: "POST",
        body: formData,
      });

      if (response.ok) {
        const data = await response.json();
        setUploadStatus("Image uploaded successfully!");
        setImage(data.ProfileImagePath);  // You can fetch this if it's served directly from your backend
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