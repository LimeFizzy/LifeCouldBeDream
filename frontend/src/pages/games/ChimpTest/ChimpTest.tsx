import React, { useState, useEffect, useRef } from "react";
import Board from "./Board";
import "./ChimpTest.css";

export const ChimpTest: React.FC = () => {
  const [level, setLevel] = useState(1);
  const [score, setScore] = useState(0);
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

  const hasInitialized = useRef(false);

  useEffect(() => {
    if (!hasInitialized.current) {
      restartGame();
      hasInitialized.current = true;
    }
  }, []);

  const restartGame = async () => {
    try {
      const response = await fetch(`http://localhost:5217/api/chimpTest/generate-sequence/${sequenceLength}`);

      if (!response.ok) {
        throw new Error("Failed to fetch sequence from the server.");
      }

      const data: Array<{ number: number; x: number; y: number }> = await response.json();
      
      console.log("Fetched sequence:", data);
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


  const onNumberClick = async (num: number) => {
    if (gameState !== "PLAY") return;

    if (num === expectedNumber) {
      if (num === sequenceLength) {
        setSequenceLength((prev) => prev + 1);
        setLevel((prev) => prev + 1);
        setScore(level < 2 ? 3 : (prev) => prev + level + 2);
        setGameState("WIN");
      } else {
          setNumbers((prev) =>
          prev.map((n) => (n.number === num ? { ...n, revealed: true } : n))
        );
        setExpectedNumber((prev) => prev + 1);
      }
    } 
    
    else {
      const username = localStorage.getItem("username") || "Unknown User";
      try {
        const response = await fetch(`http://localhost:5217/api/userscore/submit-score/chimpTest`,
          {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
              username: username,
              level: level,
              score: score
            }),
          }
        );
        
        const data = await response.json();
        console.log("Score submitted", data);
      } catch (error) {
        console.error("Error submitting score:", error);
      }
      
      setGameState("FAIL");
      setLevel(1);
      setScore(0);
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
      
        {level === 1 && (
          <div className="game-info">
          <p> 
            You must click the numbers in order after they are hidden.
          </p>

          <p>
             The sequence increases with each round. Good luck!
          </p>
          </div>
        )}

      <div className="game-info">
        <p>
          Level: {level} | Score: {score}
        </p>
      </div>

      <Board
        width={boardWidth}
        height={boardHeight}
        numbers={numbers}
        gameState={gameState}
        onNumberClick={onNumberClick}
        onBlankClick={onBlankClick}
      />
      <div className="button-placeholder">
      {gameState === "MEMORIZE" && (
        <button className="startButton" onClick={beginPlay}>
          Start
        </button>
      )}
    </div>
      {(gameState === "WIN" || gameState === "FAIL") && (
        <div className="message">
          <p>
            {gameState === "WIN"
              ? "You Win! Starting next round..."
              : "Game Over!"}
          </p>
            <button className="startButton" onClick={restartGame}>
            {gameState === "WIN" ? "Next Round" : "Play Again"}
          </button>
        </div>
      )}
    </div>
  );
};
