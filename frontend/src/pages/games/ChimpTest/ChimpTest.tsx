import React, { useState, useEffect } from "react";
import Board from "./Board";
import "./ChimpTest.css";

export const ChimpTest: React.FC = () => {
  const [sequenceLength, setSequenceLength] = useState(3);
  const boardWidth = 8;
  const boardHeight = 5;
  const [gameState, setGameState] = useState<
    "MEMORIZE" | "PLAY" | "WIN" | "FAIL"
  >("MEMORIZE");
  const [numbers, setNumbers] = useState<
    Array<{
      number: number;
      position: { x: number; y: number };
      revealed: boolean;
    }>
  >([]);
  const [expectedNumber, setExpectedNumber] = useState(1);

  useEffect(() => {
    restartGame();
  }, []);

  const getRandomInt = (min: number, max: number) => {
    return Math.floor(Math.random() * (max - min + 1)) + min;
  };

  const restartGame = () => {
    const newNumbers = [];
    const positions = Array.from(
      { length: boardWidth * boardHeight },
      (_, i) => ({
        x: i % boardWidth,
        y: Math.floor(i / boardWidth),
      })
    );

    for (let i = 0; i < sequenceLength; i++) {
      const randomIndex = getRandomInt(0, positions.length - 1);
      newNumbers.push({
        number: i + 1,
        position: positions.splice(randomIndex, 1)[0],
        revealed: false,
      });
    }

    setNumbers(newNumbers);
    setGameState("MEMORIZE");
    setExpectedNumber(1);
  };

  const beginPlay = () => setGameState("PLAY");

  const onNumberClick = (num: number) => {
    if (gameState !== "PLAY") return;

    if (num === expectedNumber) {
      if (num === sequenceLength) {
        setSequenceLength((prev) => prev + 1);
        setGameState("WIN");
      } else {
        setNumbers((prev) =>
          prev.map((n) => (n.number === num ? { ...n, revealed: true } : n))
        );
        setExpectedNumber((prev) => prev + 1);
      }
    } else {
      setGameState("FAIL");
      setSequenceLength(3);
    }
  };

  const onBlankClick = () => {
    if (gameState === "PLAY") {
      setGameState("FAIL");
    }
  };

  return (
    <div className="game">
      <h2>Chimp Memory Test</h2>
      <p>
        You must click the numbers in order after they are hidden. The sequence
        increases with each round. Good luck!
      </p>
      <Board
        width={boardWidth}
        height={boardHeight}
        numbers={numbers}
        gameState={gameState}
        onNumberClick={onNumberClick}
        onBlankClick={onBlankClick}
      />
      {gameState === "MEMORIZE" && (
        <button className="startButton" onClick={beginPlay}>
          Start
        </button>
      )}
      {(gameState === "WIN" || gameState === "FAIL") && (
        <div className="message">
          {gameState === "WIN"
            ? "You Win! Starting next round..."
            : "Game Over!"}
          <button className="startButton" onClick={restartGame}>
            {gameState === "WIN" ? "Next Round" : "Play Again"}
          </button>
        </div>
      )}
    </div>
  );
};
