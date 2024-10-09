import React, { useState, useEffect } from "react"; 
import "./SMWindow.css";

export const SMWindow: React.FC = () => {
  const [sequenceToMemorize, setSequenceToMemorize] = useState<number[]>([]);
  const [level, setLevel] = useState<number>(1);
  const [score, setScore] = useState<number>(0);
  const [userInput, setUserInput] = useState<number[]>([]);
  const [isShowingSequence, setIsShowingSequence] = useState<boolean>(true);
  const [isGameOver, setIsGameOver] = useState<boolean>(false);
  const [isRoundInProgress, setIsRoundInProgress] = useState<boolean>(false);

  // Function to start a new round and fetch a new sequence
  const startNewRound = async () => {
    if (!isRoundInProgress) {
      setIsRoundInProgress(true);
      try {
        const response = await fetch(
          `http://localhost:5217/api/sequencememory/generate-sequence/${level}`
        );
        const data = await response.json();
        const { sequence } = data;
        setSequenceToMemorize(sequence);
        setUserInput([]);
        setIsShowingSequence(true);

        // Hide the sequence after a set time
        setTimeout(() => {
          setIsShowingSequence(false); // Hide the sequence after a delay
        }, 3000); // Set to desired time in milliseconds
      } catch (error) {
        console.error("Error fetching sequence:", error);
      }
    }
  };

  // Function to handle user input submission
  const handleSubmit = async () => {
    if (userInput.join(",") === sequenceToMemorize.join(",")) {
      setScore(score + 1);
      setLevel(level + 1);
      setIsRoundInProgress(false);
      startNewRound(); // Start a new round after a correct guess
    } else {
      await handleGameOver();
    }
  };

  // Handle game over scenario
  const handleGameOver = async () => {
    setIsGameOver(true);
    const username = localStorage.getItem("username") || "Unknown User";

    try {
      const response = await fetch(
        `http://localhost:5217/api/sequencememory/submit-score/sequenceMemory`,
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            username,
            guessedSequence: userInput,
            correctSequence: sequenceToMemorize,
            level,
          }),
        }
      );

      const data = await response.json();
      console.log("Score submitted", data);
    } catch (error) {
      console.error("Error submitting score:", error);
    }
  };

  // Handle square clicks for user input
  const handleCellClick = (index: number) => {
    setUserInput([...userInput, index]);

    // Automatically submit if the user has clicked enough cells
    if (userInput.length + 1 === sequenceToMemorize.length) {
      handleSubmit();
    }
  };

  // Restart the game
  const restartGame = () => {
    setSequenceToMemorize([]);
    setLevel(1);
    setScore(0);
    setIsGameOver(false);
    setIsRoundInProgress(false);
    startNewRound();
  };

  // Start a new round on level change
  useEffect(() => {
    startNewRound();
  }, [level]);

  return (
    <div className="game-container">
      <h1>Sequence Memorization Game</h1>

      <div className="game-info">
        <p>
          Level: {level} | Score: {score}
        </p>
      </div>

      {/* If game is over, display Restart button */}
      {isGameOver ? (
        <div className="game-over">
          <p>Game Over! Your Score: {score}</p>
          <button onClick={restartGame}>Restart</button>
        </div>
      ) : (
        <div>
          <div className="sequence-grid-display">
            {isShowingSequence ? (
              <>
                <p>Memorize the sequence</p>
                <div className="grid">
                  {sequenceToMemorize.map((num, idx) => (
                    <div key={idx} className="grid-cell colored">
                      {/* The squares will flash during sequence display, no text needed */}
                    </div>
                  ))}
                </div>
              </>
            ) : (
              <div>
                <p>Select the sequence</p>
                <div className="grid">
                  {/* Display 9 clickable squares */}
                  {[...Array(9)].map((_, idx) => (
                    <div
                      key={idx}
                      className={`grid-cell clickable ${
                        userInput.includes(idx) ? "active" : ""
                      }`}
                      onClick={() => handleCellClick(idx)}
                    >
                      {/* No number displayed */}
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default SMWindow;
