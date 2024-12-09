import { BrowserRouter, Routes, Route, useLocation } from "react-router-dom";
import "./App.css";
import { Home } from "./pages/Home/Home";
import { SignIn } from "./pages/Sign In/SignIn";
import { LogIn } from "./pages/Log In/LogIn";
import { AccSettings } from "./pages/Account Settings/AccSettings";
import Navbar from "./components/navbar/Navbar";
import HomeButton from "./components/homeButton/HomeButton";
import { LNWindow } from "./pages/games/LongNumber/LNWindow";
import { ChimpTest } from "./pages/games/ChimpTest/ChimpTest";
import { SMWindow } from "./pages/games/SequenceMemorization/SMWindow";
import BackgroundWrapper from "./BackgroundWrapper";
import { Leaderboard } from "./pages/Leaderboard/Leaderboard";

export const App = () => {
  return (
    <BrowserRouter>
      <BackgroundWrapper>
        <Navbar />
        <ConditionalHomeButton />
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/signup" element={<SignIn />} />
          <Route path="/login" element={<LogIn />} />
          <Route path="/AccountSettings" element={<AccSettings />} />
          <Route path="/leaderboard" element={<Leaderboard />} />
          <Route path="/longNumber" element={<LNWindow />} />
          <Route path="/chimpTest" element={<ChimpTest />} />
          <Route path="/sequence" element={<SMWindow />} />
        </Routes>
      </BackgroundWrapper>
    </BrowserRouter>
  );
};

// Define ConditionalHomeButton inside App.tsx
const ConditionalHomeButton: React.FC = () => {
  const location = useLocation();

  if (location.pathname === "/") {
    return null;
  }

  return <HomeButton />;
};

export default App;
