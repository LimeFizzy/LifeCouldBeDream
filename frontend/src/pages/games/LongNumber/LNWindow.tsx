import React, { useState, useEffect } from 'react';
import './LNWindow.css';

export const LNWindow: React.FC = () => {
  const [numberToMemorize, setNumberToMemorize] = useState<string>(''); // Number to show
  const [userInput, setUserInput] = useState<string>(''); // User's input
  const [isShowingNumber, setIsShowingNumber] = useState<boolean>(true); // Is the number being displayed
  const [level, setLevel] = useState<number>(1); // Difficulty level (length of number)
  const [isGameOver, setIsGameOver] = useState<boolean>(false); // Game over state
  const [score, setScore] = useState<number>(0); // Player's score

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
    setTimeout(() => setIsShowingNumber(false), 3000); // Show number for 3 seconds
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
      <h1>Long Number Memory</h1>

      <div className="game-info">
        <p>Level: {level} | Score: {score}</p>
      </div>

      {isGameOver ? (
        <div className="game-over">
          <p>Game Over! Your Score: {score}</p>
          <button onClick={restartGame}>Restart</button>
        </div>
      ) : (
        <div>
          <div className="number-display">
            {isShowingNumber ? <p>{numberToMemorize}</p> : <p>Time is up</p>}
          </div>

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