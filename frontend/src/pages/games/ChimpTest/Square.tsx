import React from "react";
import "./ChimpTest.css";

interface SquareProps {
  hidden: boolean | undefined;
  showNum?: boolean;
  number?: number;
  onClick: () => void;
}

const Square: React.FC<SquareProps> = ({
  hidden,
  number,
  onClick,
  showNum,
}) => {
  return (
    <div
      className={`chimp-square${
        hidden ? "-hidden" : showNum ? " show-number" : ""
      }`}
      onClick={onClick}
    >
      {!hidden && showNum && number}
    </div>
  );
};

export default Square;
