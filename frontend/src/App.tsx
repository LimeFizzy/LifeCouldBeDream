import { BrowserRouter, Routes, Route } from 'react-router-dom';
import './App.css';
import { Home } from './pages/Home';
import { About } from './pages/About';
import { Games } from './pages/Games';
import { SignUp } from './pages/SignUp';
import { LogIn } from './pages/LogIn';
import Navbar from './components/navbar/Navbar';

export const App = () => {
  return (
    <BrowserRouter>
      <Navbar />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/about" element={<About />} />
        <Route path="/games" element={<Games />} />
        <Route path="/signup" element={<SignUp />} />
        <Route path="/login" element={<LogIn />} />
      </Routes>
    </BrowserRouter>
  );
};
