import React, { useState, useEffect, useRef } from 'react';
import './SMWindow.css';

interface Square {
  id: number;
  isActive: boolean;
}

export const SMWindow: React.FC = () => {
  const [level, setLevel] = useState(1);
  const [score, setScore] = useState(0);
  const [sequence, setSequence] = useState<Square[]>([]);
  const [userInput, setUserInput] = useState<number[]>([]);
  const [squares, setSquares] = useState<Square[]>([]);
  const [isClickable, setIsClickable] = useState(false);
  const [isGameOver, setIsGameOver] = useState<boolean>(false);
  const [isRoundInProgress, setIsRoundInProgress] = useState<boolean>(false);
  const [isSequenceReady, setIsSequenceReady] = useState(false);
  const [isDevMode, setIsDevMode] = useState<boolean>(false);

  const gridSize = 3;

  const hasInitialized = useRef(false);

  useEffect(() => {
    if (!hasInitialized.current) {
      initializeGame();
      hasInitialized.current = true;
    }
  }, []);

  useEffect(() => {
    if (isSequenceReady && !isGameOver && !isRoundInProgress) {
      startNewRound();
    }
  }, [isSequenceReady, level]);

  const initializeGame = async () => {
    const initialSquares = Array.from({ length: gridSize * gridSize }, (_, i) => ({
      id: i + 1,
      isActive: false,
    }));
    setSquares(initialSquares);
    

    await fetchInitialSequence();
  };

  const startNewRound = () => {
    if (isRoundInProgress || isGameOver || !isSequenceReady) return;

    setIsRoundInProgress(true);
    const currentLevelSequence = sequence.slice(0, level);
    setUserInput([]);

    setTimeout(() => {
      flashSequence(currentLevelSequence);
    }, 1000);
  };

  useEffect(() => {
    const checkAdminStatus = async () => {
      const username = localStorage.getItem("username");
      if (!username) return;

      try {
        const response = await fetch(
          `http://localhost:5217/api/auth/is-admin/${username}`,
          {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
            },
          }
        );

        if (response.ok) {
          const data = await response.json();
          setIsDevMode(data.isAdmin); // Set dev mode based on admin status
        }
      } catch (error) {
        console.error("Error checking admin status:", error);
      }
    };

    checkAdminStatus();
  }, []);

  
  const handleSquareClick = (id: number) => {
    if (!isClickable) return;

    const newUserInput = [...userInput, id];
    setUserInput(newUserInput);

    const correctSoFar = newUserInput.every(
      (value, index) => value === sequence[index].id
    );

    if (!correctSoFar) {
      handleGameOver();
    } else if (newUserInput.length === level) {
      setScore((prev) => prev + level);
      setLevel((prev) => prev + 1);
    }
  };

  const handleGameOver = async () => {
    setIsGameOver(true);
    setIsRoundInProgress(false);
    setIsSequenceReady(false);

    const username = localStorage.getItem("username") || "Unknown User";
  
    try {
      const response = await fetch(
        `http://localhost:5217/api/userscore/submit-score/sequenceMemory`,
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            username,
            guessedSequence: userInput,
            level,
          }),
        }
      );

      if (response.ok) {
        const data = await response.json();
      } else {
      }
    } catch (error) {
    }
  };

  const restartGame = async () => {
    setLevel(1);
    setScore(0);
    setUserInput([]);
    setIsGameOver(false);
    setIsRoundInProgress(false);
    setIsSequenceReady(false);

    await fetchInitialSequence();
  };

  const fetchInitialSequence = async () => {
    try {
      const response = await fetch(`http://localhost:5217/api/sequence/generate-sequence/30`);
      if (!response.ok) {
        throw new Error('Failed to fetch the sequence');
      }
      const data = await response.json();
      setSequence(data.sequence.map((item: any) => ({ id: item.id, isActive: false }))); // Convert backend sequence to Square format
      setIsSequenceReady(true);
    } catch (error) {
    }
  };

  const flashSequence = (sequence: Square[]) => {
    setIsClickable(false);

    sequence.forEach((square, index) => {
      const delay = index * 1000;

      setTimeout(() => {
        highlightSquare(square.id);
      }, delay);

      setTimeout(() => {
        highlightSquare(square.id, false);
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

  const advanceLevel = () => {
    setLevel((prevLevel) => prevLevel + 1);
  };

  const endGame = () => {
    handleGameOver();
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
    {isDevMode && !isGameOver && (
        <div className="dev-controls">
          <button type="button" onClick={advanceLevel}>Next Level</button>
          <button type="button" onClick={endGame}>End Game</button>
        </div>
      )}
    </div>
  );
};

export default SMWindow;
