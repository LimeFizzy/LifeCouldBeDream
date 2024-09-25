import React, { useState, useEffect } from 'react'; // Import React library w 2 hooks
import './LNWindow.css';

export const LNWindow: React.FC = () => {
  const [numberToMemorize, setNumberToMemorize] = useState<string>(''); // Number to show
  const [userInput, setUserInput] = useState<string>(''); // User's input
  const [isShowingNumber, setIsShowingNumber] = useState<boolean>(true); // Is the number being displayed
  const [level, setLevel] = useState<number>(1); // Difficulty level (length of number)
  const [isGameOver, setIsGameOver] = useState<boolean>(false); // Game over state
  const [score, setScore] = useState<number>(0); // Player's score
  const [timeRemaining, setTimeRemaining] = useState<number>(100); // Time remaining for the progress bar

  const numberDisplayDuration = 3000; // 3 s

  // Function to generate a random number of given length
  const generateRandomNumber = (length: number) => {
    let number = '';
    for (let i = 0; i < length; i++) {
      number += Math.floor(Math.random() * 10); // Generates a random digit between 0 and 9
    }
    return number;
  };

  // Function to start a new round
  const startNewRound = () => {
    const newNumber = generateRandomNumber(level);
    setNumberToMemorize(newNumber);
    setUserInput('');
    setIsShowingNumber(true);
    setTimeRemaining(100); // Like 100% of progress bar is remaining

    // Countdown for the progress bar
    const countdownInterval = setInterval(() => {
      setTimeRemaining((prevTime) => {
        if (prevTime > 0) {
          return prevTime - 1;
        } else {
          clearInterval(countdownInterval);
          return 0;
        }
      });
    }, numberDisplayDuration / 100); // 100 updates over the duration (smooth progress bar)

    // After the numberDisplayDuration, hide the number and show the input field
    setTimeout(() => {
      setIsShowingNumber(false);
    }, numberDisplayDuration);
  };

  // Handle user's input change
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setUserInput(e.target.value);
  };

  // Function to check the user's input
  const handleSubmit = () => {
    if (userInput === numberToMemorize) {
      setScore(score + 1);
      setLevel(level + 1); // Increase the level (longer number)
      startNewRound(); // Start next round
    } else {
      setIsGameOver(true); // Game over if the input is wrong
    }
  };

  // Function to restart the game
  const restartGame = () => {
    setLevel(1);
    setScore(0);
    setIsGameOver(false);
    startNewRound();
  };

  // Start a new round on first render or when the level changes
  useEffect(() => {
    startNewRound();
  }, [level]);

  return (
    <div className="game-container">
      <h1>Long Number Memory Game</h1>

      <div className="game-info">
        <p>Level: {level} | Score: {score}</p>
      </div>

      {/* As game goes on, there are 2 option */}
      {/* If game is over this gives Restart button */}
      {isGameOver ? ( 
        <div className="game-over">
          <p>Game Over! Your Score: {score}</p>
          <button onClick={restartGame}>Restart</button>
        </div>
      ) : ( 
        // Else it displays a number or prompts that time is up
        <div>
          <div className="number-display">
            {isShowingNumber ? (
              <>
                <p>Remember the number:</p> {/* New line for the prompt */}
                <p className="number" style={{ marginTop: '5px' }}>{numberToMemorize}</p> {/* The number itself with margin for spacing */}
              </>
            ) : (
              <p>Time is up</p>
            )}
          </div>

          {/* Just for the progress bar */}
          {isShowingNumber && (
            <div className="progress-container">
              <div className="progress-bar-container">
                <div
                  className="progress-bar"
                  style={{ width: `${timeRemaining}%` }}
                ></div>
              </div>
            </div>
          )}

          {/* When number is hidden this gives input field and Submit button */}
          {!isShowingNumber && (
            <div className="input-section">
              <input
                type="text"
                value={userInput}
                onChange={handleInputChange}
                placeholder="Enter the number"
              />
              <button onClick={handleSubmit}>Submit</button>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default LNWindow;
