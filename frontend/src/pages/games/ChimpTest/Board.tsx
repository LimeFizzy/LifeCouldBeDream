import React from "react";
import Square from "./Square";

interface BoardProps {
  width: number;
  height: number;
  numbers: Array<{
    number: number;
    position: { x: number; y: number };
    revealed: boolean;
  }>;
  gameState: "MEMORIZE" | "PLAY" | "WIN" | "FAIL";
  onNumberClick: (num: number) => void;
  onBlankClick: () => void;
}

const Board: React.FC<BoardProps> = ({
  width,
  height,
  numbers,
  gameState,
  onNumberClick,
  onBlankClick,
}) => {
  const rows = Array.from({ length: height }, (_, row) => row);
  const cells = Array.from({ length: width }, (_, cell) => cell);

  return (
    <table>
      <tbody>
        {rows.map((row) => (
          <tr key={row}>
            {cells.map((cell) => {
              const number = numbers.find(
                (n) => n.position.x === cell && n.position.y === row
              );
              return (
                <td key={cell}>
                  <Square
                    hidden={gameState === "PLAY" && !number?.number}
                    number={number?.revealed ? undefined : number?.number}
                    showNum={gameState !== "PLAY"}
                    onClick={
                      number ? () => onNumberClick(number.number) : onBlankClick
                    }
                  />
                </td>
              );
            })}
          </tr>
        ))}
      </tbody>
    </table>
  );
};

export default Board;
