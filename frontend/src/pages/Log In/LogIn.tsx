import { UserTile } from "../../components/usertile/UserTile";
import "./LogIn.css";
import { useState } from "react";

export const LogIn = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [message, setMessage] = useState("");

  const handleLogin = async (event: React.FormEvent) => {
    event.preventDefault();

    try {
      const response = await fetch("http://localhost:5217/api/auth/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          username,
          password,
        }),
      });

      if (response.ok) {
        const data = await response.json(); // **Extract userId and username**
        localStorage.setItem("userId", data.userId); // **Store userId in localStorage**
        localStorage.setItem("username", data.username); // **Store username in localStorage**
        setMessage("Login successful!");
      } else {
        setMessage(`Error. Wrong password or username`);
      }
    } catch (error) {
      setMessage("An error occurred during login.");
    }
  };

  return (
    <UserTile>
      <h1>Log In</h1>
      <div className="login-input-container">
        <label>Username</label>
        <input
          type="text"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
        />
        <label>Password</label>
        <input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />

        <button onClick={handleLogin}>Log In</button>
        <div className="status-message">{message}</div>
      </div>
    </UserTile>
  );
};
