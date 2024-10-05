import { BrowserRouter, Routes, Route } from "react-router-dom";
import "./App.css";
import { Home } from "./pages/Home/Home";
import { SignIn } from "./pages/Sign In/SignIn";
import { LogIn } from "./pages/Log In/LogIn";
import Navbar from "./components/navbar/Navbar";
import { LNWindow } from "./pages/games/LongNumber/LNWindow";
import { ChimpTest } from "./pages/games/ChimpTest";
import { Sequence } from "./pages/games/Sequence";
import BackgroundWrapper from "./BackgroundWrapper";
import { Leaderboard } from "./pages/Leaderboard/Leaderboard";

export const App = () => {
  return (
    <BrowserRouter>
      <BackgroundWrapper>
        <Navbar />
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/signup" element={<SignIn />} />
          <Route path="/login" element={<LogIn />} />
          <Route path="/longNumber" element={<LNWindow />} />
          <Route path="/leaderboard" element={<Leaderboard />} />
          <Route path="/longNumber" element={<LNWindow />} />
          <Route path="/chimpTest" element={<ChimpTest />} />
          <Route path="/sequence" element={<Sequence />} />
        </Routes>
      </BackgroundWrapper>
    </BrowserRouter>
  );
};
