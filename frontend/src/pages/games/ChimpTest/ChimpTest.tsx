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
      X: number;
      Y: number;
      revealed: boolean;
    }>>([]);
  const [expectedNumber, setExpectedNumber] = useState(1);

  useEffect(() => {
    console.log("========================================================");
    console.log("In useEffect");
    restartGame();
  }, []);

  const restartGame = async () => {
    try {

      console.log("In restartGame\n");


      const response = await fetch(`http://localhost:5217/api/chimptest/generate-sequence/${sequenceLength}`);

      if (!response.ok) {
        throw new Error("Failed to fetch sequence from the server.");
      }

      const data: Array<{ number: number; x: number; y: number }> = await response.json();
      console.log("Received data from server:", data);


      const newNumbers = data.map((item) => ({
        number: item.number,
        X: item.x,
        Y: item.y,
        revealed: false,
      }));
      console.log("Mapped newNumbers:", newNumbers);

    setNumbers(newNumbers);
    setGameState("MEMORIZE");
    setExpectedNumber(1);
    
    } catch (error) {
      console.error("Error fetching sequence:", error);
    }
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
