import React, { useState, useEffect } from "react";
import "./LNWindow.css";

export const LNWindow: React.FC = () => {
  const [numberToMemorize, setNumberToMemorize] = useState<string>("");
  const [level, setLevel] = useState<number>(1);
  const [score, setScore] = useState<number>(0);
  const [timeRemaining, setTimeRemaining] = useState<number>(100);
  const [userInput, setUserInput] = useState<string>("");
  const [isShowingNumber, setIsShowingNumber] = useState<boolean>(true);
  const [isGameOver, setIsGameOver] = useState<boolean>(false);
  const [isRoundInProgress, setIsRoundInProgress] = useState<boolean>(false);
  const [isDevMode, setIsDevMode] = useState<boolean>(false);

  const startNewRound = async () => {
    if (!isRoundInProgress) {
      setIsRoundInProgress(true);
      try {
        const response = await fetch(
          `http://localhost:5217/api/longnumber/generate-sequence/${level}`
        );
        const data = await response.json();

        const { sequence } = data;
        setNumberToMemorize(sequence.join(""));
        setUserInput("");
        setIsShowingNumber(true);

        if (!isDevMode) {
          setTimeRemaining(100); // IMPORTANT, with this progress bar does not start from the middle, then jump to the outside, with each next round

          let countdownValue = 100;
          const countdownInterval = setInterval(() => {
            countdownValue -= 1;
            setTimeRemaining(countdownValue);

            if (countdownValue <= 0) {
              clearInterval(countdownInterval);
              setIsShowingNumber(false);
            }
          }, 50); // IMPORTANAT, 50ms is the delay for render.
        }
      } catch (error) {
        console.error("Error fetching sequence:", error);
      }
    }
  };


  const handleSubmit = async () => {
    
    const guessedSequence = userInput.split("").map(Number);
    const correctSequence = numberToMemorize.split("").map(Number);
  
    if (guessedSequence.join("") === correctSequence.join("")) {
      setScore(score + 1);
      setLevel(level + 1);
      setIsRoundInProgress(false);
      startNewRound();
    } else {
      await handleGameOver(); 
    }
  };
  

  const handleGameOver = async () => {
    setIsGameOver(true); 
    const username = localStorage.getItem('username') || 'Unknown User';
  
    try {
      const response = await fetch(
        `http://localhost:5217/api/longnumber/submit-score/longNumberMemory`,
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            username,
            guessedSequence: userInput.split("").map(Number),
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

  const restartGame = () => {
    setNumberToMemorize("");
    setLevel(1);
    setScore(0);
    setIsGameOver(false);
    setIsRoundInProgress(false);
    startNewRound();
  };

  const nextRound = () => {
    setScore(score + 1);
    setLevel(level + 1);
    setIsRoundInProgress(false);
    startNewRound();
  }

  useEffect(() => {
    startNewRound();
  }, [level]);

  return (
    <div className="game-container">
      <h1>Long Number Memory Game</h1>

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
          <div className="number-display">
            {(isShowingNumber || isDevMode) ? (
              <>
                <p>Remember the number:</p>
                <p className="number" style={{ marginTop: "5px" }}>
                  {numberToMemorize}
                </p>
              </>
            ) : (
              <p>Time is up</p>
            )}
          </div>

          {/* Progress bar */}
          {isShowingNumber && !isDevMode && (
            <div className="progress-container">
              <div className="progress-bar-container">
                <div
                  className="progress-bar"
                  style={{ width: `${timeRemaining}%` }}
                ></div>
              </div>
            </div>
          )}

          {/* When number is hidden, display input field and Submit button */}
          {(!isShowingNumber || isDevMode) && (
            <div className="input-section">
              <input
                type="text"
                value={userInput}
                onChange={(e) => setUserInput(e.target.value)}
                placeholder="Enter the number"
              />
              <button type="button" onClick={handleSubmit}>
                Submit
              </button>
              {isDevMode && 
                <button type="button" onClick={nextRound}>
                  Next
                </button>
              }
              {isDevMode && 
                <button type="button" onClick={handleGameOver}>
                  End game
                </button>
              }
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default LNWindow;
