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

  // there is a bug
  // after restarting the game, first attempt is marked as wrong

  useEffect(() => {
    console.log("----useEffect[]");
    initializeGame();
  }, []);
  
  useEffect(() => {
    if (!isGameOver && !isRoundInProgress && sequence.length > 0) {
      startNewRound();
    }
  }, [level, sequence]);
  

  const initializeGame = async () => {
    console.log("----initializeGame START");

    const initialSquares = Array.from({ length: gridSize * gridSize }, (_, i) => ({
      id: i + 1,
      isActive: false,
    }));
    setSquares(initialSquares);

    await fetchInitialSequence();
    console.log("----initializeGame FINISH before startNewRound");

    setTimeout(() => {
      startNewRound();      
    }, 2000);
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
    } catch (error) {
      console.error('Error fetching sequence:', error);
    }
    console.log("----fetchInitialSequence - FINISH");

  };

  const startNewRound = () => {
    console.log("----startNewRound");
    if (isRoundInProgress || isGameOver) return;
  
    setIsRoundInProgress(true);
    const currentLevelSequence = sequence.slice(0, level);
    setUserInput([]);

    setTimeout(() => {
      flashSequence(currentLevelSequence);      
    }, 1000); //just looks better
  };
  

  const flashSequence = (sequence: number[]) => {
    setIsClickable(false);
  
    sequence.forEach((id, index) => {
      const delay = index * 1000;
  
      setTimeout(() => {
        console.log(`Highlighting square ${id} (on)`); // Add logging here
        highlightSquare(id);
      }, delay);
  
      setTimeout(() => {
        console.log(`Removing highlight from square ${id} (off)`); // Add logging here
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
    console.log("----handleGameOver");

    setIsGameOver(true);
    setIsRoundInProgress(false);
  };

  const restartGame = async () => {
    console.log("----restartGame - START");

    setLevel(1);
    setScore(0);
    setUserInput([]);
    setIsGameOver(false);
    setIsRoundInProgress(false);

    await fetchInitialSequence();
    // setSequence(newSequence);
    console.log("----restartGame - FINISH before startNewRound");

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
