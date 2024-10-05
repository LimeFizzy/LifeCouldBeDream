import React, { ReactNode, useEffect } from "react";
import { useLocation } from "react-router-dom";

interface BackgroundWrapperProps {
  children: ReactNode;
}

export const BackgroundWrapper: React.FC<BackgroundWrapperProps> = ({
  children,
}) => {
  const location = useLocation();

  useEffect(() => {
    if (location.pathname === "/longNumber") {
      document.body.style.background =
        "linear-gradient(137deg, #202020 -7.65%, #31185A 20.72%, #431391 51.26%, #673593 88.76%, #803F93 104.74%)";
    } else if (location.pathname === "/chimpTest") {
      document.body.style.background =
        "linear-gradient(196deg, #202020 -12.53%, #2F1855 28.02%, #3F1488 58.87%, #643392 100.84%, #8E41A5 129.05%)";
    } else if (location.pathname === "/sequence") {
      document.body.style.background =
        "linear-gradient(243deg, #202020 8.54%, #3D206D 49.94%, #46109F 85.64%, #7843A8 129.67%, #9856AC 154.95%)";
    } else if (location.pathname === "/leaderboard") {
      document.body.style.background =
        "linear-gradient(245deg, #202020 12.72%, #3D206D 58.76%, #46109F 98.47%, #7843A8 147.44%, #9856AC 175.55%)";
    } else if (location.pathname === "/signup") {
      document.body.style.background =
        "linear-gradient(291deg, #202020 -11.12%, #3D206D 17.52%, #4B15A5 53.56%, #3D206D 85.61%, #202020 109%)";
    } else if (location.pathname === "/login") {
      document.body.style.background =
        "linear-gradient(71deg, #202020 -8.45%, #3D206D 18.41%, #4B18A1 52.21%, #3D206D 83.88%, #202020 104.2%)";
    } else if (location.pathname === "/AccountSettings") {
      document.body.style.background =
        "linear-gradient(0deg, #202020 -25.23%, #4B2884 2.94%, #410F92 50.65%, #3E1F70 91.18%, #202020 126.94%)";
    } else {
      document.body.style.backgroundImage =
        'url("/src/assets/BackGround_Bright.jpeg")';
    }

    // Clean up when component unmounts
    return () => {
      document.body.style.background = "";
    };
  }, [location]);

  return <div className="background-wrapper">{children}</div>;
};

export default BackgroundWrapper;
