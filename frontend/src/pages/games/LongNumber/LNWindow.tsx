import React, { useState, useEffect } from 'react';
import './LNWindow.css';

export const LNWindow: React.FC = () => {
  const [numberToMemorize, setNumberToMemorize] = useState<string>(''); // Number to show
  const [userInput, setUserInput] = useState<string>(''); // User's input
  const [isShowingNumber, setIsShowingNumber] = useState<boolean>(true); // Is the number being displayed
  const [level, setLevel] = useState<number>(1); // Difficulty level (length of number)
  const [isGameOver, setIsGameOver] = useState<boolean>(false); // Game over state
  const [score, setScore] = useState<number>(0); // Player's score
  const [timeRemaining, setTimeRemaining] = useState<number>(100); // Time remaining for the progress bar


  // Function to start a new round
  const startNewRound = async () => {
    try {
        const response = await fetch(`http://localhost:5217/api/longnumber/generate-sequence/${level}`);
        const data = await response.json();

        const { sequence} = data;
        setNumberToMemorize(sequence.join(''));
        setUserInput('');
        setIsShowingNumber(true);
        setTimeRemaining(100);      // IMPORTANT, with this progress bar does not start from the middle, then jump to the outside, with each next round

        let countdownValue = 100;
        const countdownInterval = setInterval(() => {
            countdownValue -= 1;
            setTimeRemaining(countdownValue);

            if (countdownValue <= 0) {
                clearInterval(countdownInterval);
                setIsShowingNumber(false);
            }
        }, 50);

    } catch (error) {
        console.error('Error fetching sequence:', error);
    }
};



  // Function to check the user's input
  const handleSubmit = async () => {
    try {
      const response = await fetch('http://localhost:5217/api/longnumber/submit-score', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          username: 'Player1',
          guessedSequence: userInput.split('').map(Number), 
          correctSequence: numberToMemorize.split('').map(Number),
          level
        })
      });

      const data = await response.json();
      if (data.score > 0) {
        setScore(score + 1);
        setLevel(level + 1); 
        startNewRound(); // Start next round
      } else {
        setIsGameOver(true); // Game over if the input is wrong
      }

    } catch (error) {
      console.error('Error submitting score:', error);
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

      {/* If game is over, display Restart button */}
      {isGameOver ? (
        <div className="game-over">
          <p>Game Over! Your Score: {score}</p>
          <button onClick={restartGame}>Restart</button>
        </div>
      ) : (
        <div>
          <div className="number-display">
            {isShowingNumber ? (
              <>
                <p>Remember the number:</p>
                <p className="number" style={{ marginTop: '5px' }}>{numberToMemorize}</p>
              </>
            ) : (
              <p>Time is up</p>
            )}
          </div>

          {/* Progress bar */}
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

          {/* When number is hidden, display input field and Submit button */}
          {!isShowingNumber && (
            <div className="input-section">
              <input
                type="text"
                value={userInput}
                onChange={e => setUserInput(e.target.value)}
                placeholder="Enter the number"
              />
              <button type="button" onClick={handleSubmit}>Submit</button>

            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default LNWindow;
