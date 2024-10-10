import './SingIn.css';
import { UserTile } from '../../components/usertile/UserTile';
import { useState } from 'react';

export const SignIn: React.FC = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [repeatPassword, setRepeatPassword] = useState('');
    const [usernameStatus] = useState('');
    const [passwordStatus, setPasswordStatus] = useState('');
    const [message, setMessage] = useState('');

    const handleRegister = async (event: React.FormEvent) => {
        event.preventDefault();
        
        if (password !== repeatPassword) {
            setPasswordStatus('Passwords do not match.');
            return;
        }

        try {
            const response = await fetch('http://localhost:5217/api/auth/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    username,
                    password,
                }),
            });

            if (response.ok) {
                setMessage('Registration successful!');
            } else {
                const errorText = await response.text();
                setMessage(errorText);
            }
        } catch (error) {
            setMessage('An error occurred during registration.');
        }
    };

    return (
        <UserTile>
            <h1>Sign In</h1>
            <div className="signin-input-container">
                <label>Username</label>
                <input 
                    type="text"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                />
                <div className="status-message">{usernameStatus}</div>

                <div className="password-container">
                    <label>Password</label>
                    <input 
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                    <label>Repeat Password</label>
                    <input 
                        type="password"
                        value={repeatPassword}
                        onChange={(e) => setRepeatPassword(e.target.value)}
                    />
                    <div className="password-status-message">{passwordStatus}</div>
                </div>
                
                <button onClick={handleRegister}>Register</button>
                <div className="status-message">{message}</div>
            </div>
        </UserTile>
    );
};
