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

  const generateRandomSequence = (length: number): number[] => {
    return Array.from({ length }, () => Math.floor(Math.random() * (gridSize * gridSize)) + 1);
  };

  useEffect(() => {
    initializeGame();
  }, []);
  
  useEffect(() => {
    // important - make sure the seq is ready before starting the round
    if (!isGameOver && !isRoundInProgress && sequence.length > 0) {
      startNewRound();
    }
  }, [level, sequence]); // trigger when level changes, but only if seq is ready
  

  const initializeGame = () => {
    const initialSquares = Array.from({ length: gridSize * gridSize }, (_, i) => ({
      id: i + 1,
      isActive: false,
    }));
    setSquares(initialSquares);

    setSequence(generateRandomSequence(30));

    startNewRound();
  };

  const startNewRound = (currentSequence: number[] = sequence) => {
    if (isRoundInProgress || isGameOver) return;
  
    if (currentSequence.length < level) {
      console.error("Sequence not ready for the current level.");
      return;
    }
  
    setIsRoundInProgress(true);
    const currentLevelSequence = currentSequence.slice(0, level);
    setUserInput([]);
    flashSequence(currentLevelSequence);
  };
  

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

  const highlightSquare = (id: number, active: boolean = true) => {
    setSquares((prevSquares) =>
      prevSquares.map((square) =>
        square.id === id ? { ...square, isActive: active } : square
      )
    );
  };

  const handleSquareClick = (id: number) => {
    if (!isClickable) return;

    const newUserInput = [...userInput, id];
    setUserInput(newUserInput);

    const correctSoFar = newUserInput.every(
      (value, index) => value === sequence[index]
    );

    if (!correctSoFar) {
      handleGameOver();
    } else if (newUserInput.length === level) {
      setScore((prev) => prev + level);
      setLevel((prev) => prev + 1);
    }
  };

  const handleGameOver = () => {
    setIsGameOver(true);
    setIsRoundInProgress(false);
  };

  const restartGame = () => {
    setLevel(1);
    setScore(0);
    setUserInput([]);
    setIsGameOver(false);
    setIsRoundInProgress(false);

    const newSequence = generateRandomSequence(30);
    setSequence(newSequence);
    startNewRound();
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
