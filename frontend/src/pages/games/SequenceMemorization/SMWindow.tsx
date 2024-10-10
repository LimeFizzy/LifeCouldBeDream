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

  const gridSize = 3; // 3x3 grid for the game

  // Initialize the squares state when the component mounts
  useEffect(() => {
    const initialSquares = Array.from({ length: gridSize * gridSize }, (_, i) => ({
      id: i + 1,
      isActive: false,
    }));
    setSquares(initialSquares);
  }, []);

  // Generate a new sequence for each level
  useEffect(() => {
    const newSequence = Array.from({ length: level }, () => Math.floor(Math.random() * (gridSize * gridSize)) + 1);
    setSequence(newSequence);
    setUserInput([]);
    flashSequence(newSequence);
  }, [level]);

  // Flash the sequence for the user to memorize
  const flashSequence = (sequence: number[]) => {
    setIsClickable(false);
    let delay = 0;

    sequence.forEach((id, index) => {
      setTimeout(() => {
        highlightSquare(id);
      }, delay);

      delay += 1000; // 1 second per flash

      setTimeout(() => {
        highlightSquare(id, false);
      }, delay - 300); // Highlight for 700ms
    });

    // Allow the user to click once the sequence is done
    setTimeout(() => {
      setIsClickable(true);
    }, delay);
  };

  // Highlight a square by its ID
  const highlightSquare = (id: number, active: boolean = true) => {
    setSquares((prevSquares) =>
      prevSquares.map((square) =>
        square.id === id ? { ...square, isActive: active } : square
      )
    );
  };

  // Handle user clicks on squares
  const handleSquareClick = (id: number) => {
    if (!isClickable) return;

    const newUserInput = [...userInput, id];
    setUserInput(newUserInput);

    // Check if the user input is correct so far
    if (newUserInput.join('') === sequence.slice(0, newUserInput.length).join('')) {
      if (newUserInput.length === sequence.length) {
        // If the user completes the sequence, they pass the level
        setScore((prev) => prev + 1);
        setLevel((prev) => prev + 1);
      }
    } else {
      // If the user gets the sequence wrong, reset the game
      setScore(0);
      setLevel(1);
    }
  };

  return (
    <div className="game-container">
      <h1>Sequence Memorization</h1>
      <div className="game-info">
        <p>Level: {level} | Score: {score}</p>
      </div>
      <div className="grid">
        {squares.map((square) => (
          <button
            key={square.id}
            className={`square ${square.isActive ? 'active' : ''}`}
            onClick={() => handleSquareClick(square.id)}
          />
        ))}
      </div>
    </div>
  );
};
