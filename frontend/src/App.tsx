import { BrowserRouter, Routes, Route } from 'react-router-dom';
import './App.css';
import { Home } from './pages/Home/Home';
import { SignUp } from './pages/SignUp';
import { LogIn } from './pages/LogIn';
import Navbar from './components/navbar/Navbar';
import { LongNumber } from './pages/games/LongNumber';
import { ChimpTest } from './pages/games/ChimpTest';
import { Sequence } from './pages/games/Sequence';
import { Leaderboard } from './pages/Leaderboard/Leaderboard';

export const App = () => {
  return (
    <BrowserRouter>
      <Navbar />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/signup" element={<SignUp />} />
        <Route path="/login" element={<LogIn />} />
        <Route path='/leaderboard' element={<Leaderboard />} />
        <Route path='/longNumber' element={<LongNumber />} />
        <Route path='/chimpTest' element={<ChimpTest />} />
        <Route path='/sequence' element={<Sequence />} />
      </Routes>
    </BrowserRouter>
  );
}
