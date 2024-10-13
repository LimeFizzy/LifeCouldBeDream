import React, { useState, useEffect } from 'react';
import './SMWindow.css';

interface Square {
  id: number;
  isActive: boolean;
}

export const SMWindow: React.FC = () => {
  const [level, setLevel] = useState(1);
  const [score, setScore] = useState(0);
  const [sequence, setSequence] = useState<number[]>([]);
  const [userInput, setUserInput] = useState<number[]>([]);
  const [squares, setSquares] = useState<Square[]>([]);
  const [isClickable, setIsClickable] = useState(false);
  const [isGameOver, setIsGameOver] = useState<boolean>(false);
  const [isRoundInProgress, setIsRoundInProgress] = useState<boolean>(false);

  const gridSize = 3;

  // Generate random sequence for the game (length 30)
  const generateRandomSequence = (length: number): number[] => {
    const randomSequence: number[] = [];
    for (let i = 0; i < length; i++) {
      const randomNum = Math.floor(Math.random() * 9) + 1;
      randomSequence.push(randomNum);
    }
    return randomSequence;
  };

  // Initialize game and sequence on component mount
  useEffect(() => {
    const initialSquares = Array.from({ length: gridSize * gridSize }, (_, i) => ({
      id: i + 1,
      isActive: false,
    }));
    setSquares(initialSquares);

    const randomSequence = generateRandomSequence(30); // Generate sequence once for the game
    setSequence(randomSequence);
  }, []); // This runs only once when the game starts

  // Start a new round based on the current level
  useEffect(() => {
    if (!isGameOver) {
      startNewRound();
    }
  }, [level]); // This runs every time the level changes

  const startNewRound = () => {
    if (!isRoundInProgress && !isGameOver) {
      setIsRoundInProgress(true);

      const currentLevelSequence = sequence.slice(0, level); // Use part of the sequence up to the current level
      setUserInput([]);

      flashSequence(currentLevelSequence); // Flash the sequence for the current level
    }
  };

  // Flash the sequence squares
  const flashSequence = (sequence: number[]) => {
    setIsClickable(false);

    sequence.forEach((id, index) => {
      const delay = index * 1000;

      setTimeout(() => {
        highlightSquare(id);
      }, delay);

      setTimeout(() => {
        highlightSquare(id, false);
      }, delay + 800);
    });

    setTimeout(() => {
      setIsClickable(true);
      setIsRoundInProgress(false);
    }, sequence.length * 1000);
  };

  // Highlight a square based on its ID
  const highlightSquare = (id: number, active: boolean = true) => {
    setSquares((prevSquares) =>
      prevSquares.map((square) =>
        square.id === id ? { ...square, isActive: active } : square
      )
    );
  };

  // Handle user input
  const handleSquareClick = (id: number) => {
    if (!isClickable) return;

    const newUserInput = [...userInput, id];
    setUserInput(newUserInput);

    if (newUserInput.join('') === sequence.slice(0, newUserInput.length).join('')) {
      if (newUserInput.length === sequence.slice(0, level).length) {
        setScore((prev) => prev + level);
        setLevel((prev) => prev + 1);
      }
    } else {
      handleGameOver();
    }
  };

  // Handle game over state
  const handleGameOver = () => {
    setIsGameOver(true);
    setIsRoundInProgress(false);
  };

  // Restart the game
  const restartGame = () => {
    const randomSequence = generateRandomSequence(30); // Generate new sequence
    setSequence(randomSequence);
    setLevel(1);
    setScore(0);
    setUserInput([]);
    setIsGameOver(false);
    setIsRoundInProgress(false);
  };

  return (
    <div className="game-container">
      <h1>Sequence Memorization</h1>
      <div className="game-info">
        <p>Level: {level} | Score: {score}</p>
      </div>

      {isGameOver ? (
        <div className="game-over">
          <p>Game Over! Your Score: {score}</p>
          <button onClick={restartGame}>Restart</button>
        </div>
      ) : (
        <div className="grid">
          {squares.map((square) => (
            <button
              key={square.id}
              className={`square ${square.isActive ? 'active' : ''}`}
              onClick={() => handleSquareClick(square.id)}
            />
          ))}
        </div>
      )}
    </div>
  );
};

export default SMWindow;
