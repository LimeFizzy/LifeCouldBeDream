import { useEffect, useState } from "react";
import { UserTile } from "../../components/usertile/UserTile";
import "./AccSettings.css";

export const AccSettings = () => {
  const [username, setUsername] = useState<string | null>("");
  const [oldPassword, setOldPassword] = useState<string>("");
  const [newPassword, setNewPassword] = useState<string>("");
  const [passwordStatus, setPasswordStatus] = useState<string>("");
  const [image, setImage] = useState<string | null>(null);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [uploadStatus, setUploadStatus] = useState<string>("");
  const [userId, setUserId] = useState<number | null>(null);

  // Fetch the userId and username from localStorage when the component loads
  useEffect(() => {
    const storedUserId = localStorage.getItem("userId");
    const storedUsername = localStorage.getItem("username");
    if (storedUserId) {
      setUserId(Number(storedUserId));
      setUsername(storedUsername);
      fetchProfileImage(Number(storedUserId));
    }
  }, []);

  // Fetch the profile image for the logged-in user
  const fetchProfileImage = async (userId: number) => {
    try {
      const response = await fetch(
        `http://localhost:5217/api/pictureupload/${userId}/profile-image`
      );
      if (response.ok) {
        const imageBlob = await response.blob();
        const imageUrl = URL.createObjectURL(imageBlob);
        setImage(imageUrl);
      }
    } catch (error) {
      console.error("Error fetching profile image:", error);
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
      const response = await fetch(
        `http://localhost:5217/api/pictureupload/upload-profile-image/${userId}`,
        {
          method: "POST",
          body: formData,
        }
      );

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

  // Handle password change
  const handleChangePassword = async () => {
    if (!oldPassword || !newPassword || !userId) {
      setPasswordStatus("Please fill in both old and new passwords.");
      return;
    }

    const changePasswordDto = {
      username: username,
      oldPassword: oldPassword, // Assuming the user inputs their old password
      newPassword: newPassword, // User's new password
    };

    try {
      const response = await fetch(
        "http://localhost:5217/api/auth/change-password",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(changePasswordDto),
        }
      );

      if (response.ok) {
        setPasswordStatus("Password changed successfully!");
        setOldPassword("");
        setNewPassword("");
      } else {
        const result = await response.json();
        setPasswordStatus(result?.Message || "Failed to change password.");
      }
    } catch (error) {
      setPasswordStatus("Error occurred while changing password.");
      console.error("Error during password change:", error);
    }
  };

  return (
    <div className="user-tile-container">
      <UserTile>
        <h1>Account Settings</h1>
        <div className="signin-input-container">
          <label>Username</label>
          <div className="username-display">
            <p>{username}</p>
          </div>
          <div className="password-container">
            <label>Old Password</label>
            <input
              type="password"
              value={oldPassword}
              onChange={(e) => setOldPassword(e.target.value)}
            />
            <label>New Password</label>
            <input
              type="password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
            />
            <div className="status-message">{passwordStatus}</div>
          </div>

          <button type="button" onClick={handleChangePassword}>
            Change Password
          </button>
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
          <button type="button" onClick={handleUpload}>
            Upload Image
          </button>
          <div className="image-status-message">{uploadStatus}</div>
        </div>
      </UserTile>
    </div>
  );
};
