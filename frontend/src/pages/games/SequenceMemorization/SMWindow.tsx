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
  const [isSequenceReady, setIsSequenceReady] = useState(false);

  const gridSize = 3;

  // Initialize game on first load
  useEffect(() => {
    initializeGame();
  }, []);

  // Start new round only when sequence is fully fetched and set
  useEffect(() => {
    if (isSequenceReady && !isGameOver && !isRoundInProgress) {
      startNewRound();
    }
  }, [isSequenceReady, level]);

  const initializeGame = async () => {
    console.log("----initializeGame START");

    const initialSquares = Array.from({ length: gridSize * gridSize }, (_, i) => ({
      id: i + 1,
      isActive: false,
    }));
    setSquares(initialSquares);

    await fetchInitialSequence();
    console.log("----initializeGame FINISH before startNewRound");
  };

  const fetchInitialSequence = async () => {
    console.log("----fetchInitialSequence - START");

    try {
      const response = await fetch(`http://localhost:5217/api/sequence/generate-sequence/30`);
      if (!response.ok) {
        throw new Error('Failed to fetch the sequence');
      }
      const data = await response.json();
      setSequence(data.sequence);
      setIsSequenceReady(true);  // Set sequence ready only when it's fully fetched
    } catch (error) {
      console.error('Error fetching sequence:', error);
    }
    console.log("----fetchInitialSequence - FINISH");
  };

  const startNewRound = () => {
    console.log("----startNewRound");
    if (isRoundInProgress || isGameOver || !isSequenceReady) return;

    setIsRoundInProgress(true);
    const currentLevelSequence = sequence.slice(0, level);
    setUserInput([]);
    
    // Flash sequence after a delay to improve user experience
    setTimeout(() => {
      flashSequence(currentLevelSequence);
    }, 1000);
  };

  const flashSequence = (sequence: number[]) => {
    setIsClickable(false);

    sequence.forEach((id, index) => {
      const delay = index * 1000;

      setTimeout(() => {
        console.log(`Highlighting square ${id} (on)`);
        highlightSquare(id);
      }, delay);

      setTimeout(() => {
        console.log(`Removing highlight from square ${id} (off)`);
        highlightSquare(id, false);
      }, delay + 800);
    });

    setTimeout(() => {
      setIsClickable(true);  // Allow clicking only after flashing is complete
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
    setIsSequenceReady(false);  // Reset sequence readiness
  };

  const restartGame = async () => {
    setLevel(1);
    setScore(0);
    setUserInput([]);
    setIsGameOver(false);
    setIsRoundInProgress(false);
    setIsSequenceReady(false);  // Reset readiness before fetching

    // Fetch the new sequence and let useEffect handle starting the round
    await fetchInitialSequence();
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
