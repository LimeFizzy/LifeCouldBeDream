import { BrowserRouter, Routes, Route } from 'react-router-dom';
import './App.css';
import { Home } from './pages/Home/Home';
import { SignUp } from './pages/SignUp';
import { LogIn } from './pages/LogIn';
import Navbar from './components/navbar/Navbar';
import { LNWindow } from './pages/games/LongNumber/LNWindow';
import { ChimpTest } from './pages/games/ChimpTest';
import { Sequence } from './pages/games/Sequence';

export const App = () => {
  return (
    <BrowserRouter>
      <Navbar />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/signup" element={<SignUp />} />
        <Route path="/login" element={<LogIn />} />
        <Route path='/longNumber' element={<LNWindow />} />
        <Route path='/chimpTest' element={<ChimpTest />} />
        <Route path='/sequence' element={<Sequence />} />
      </Routes>
    </BrowserRouter>
  );
}
