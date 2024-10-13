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
  const fixedSequence = [1, 2, 3, 4, 5, 6, 7, 8, 9];

  const startNewRound = () => {
    setTimeout(() => {
      if (!isRoundInProgress) {
        setIsRoundInProgress(true);
        
        const currentLevelSequence = fixedSequence.slice(0, level);
        setSequence(currentLevelSequence);
        setUserInput([]);

        flashSequence(currentLevelSequence);
      }
    }, 1500);
  };

  useEffect(() => {
    const initialSquares = Array.from({ length: gridSize * gridSize }, (_, i) => ({
      id: i + 1,
      isActive: false,
    }));
    setSquares(initialSquares);
    startNewRound();
  }, [level]);

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

    if (newUserInput.join('') === sequence.slice(0, newUserInput.length).join('')) {
      if (newUserInput.length === sequence.length) {
        setScore((prev) => prev + level);
        setLevel((prev) => prev + 1);
        setIsRoundInProgress(false);
        startNewRound();
      }
    } else {
      handleGameOver();
    }
  };

  const handleGameOver = () => {
    setIsGameOver(true);
    setIsRoundInProgress(false);
  };

  const restartGame = () => {
    setSequence([]);
    setLevel(1);
    setScore(0);
    setIsGameOver(false);
    setIsRoundInProgress(false);
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
