import { useCallback, useEffect, useState } from "react";
import { Dropdown } from "../../components/dropdown/Dropdown";
import "./Leaderboard.css";

interface Result {
  nr: number;
  username: string;
  result: string;
  date: string;
}

enum GameTypes {
  LONG_NUMBER = "longNumberMemory",
  SEQUENCE = "sequenceMemory",
  CHIMP = "chimpTest",
}

const fetchLeaderboard = async (gameType: string) => {
  try {
    const response = await fetch(
      `http://localhost:5217/api/UserScore/leaderboard/${gameType}`
    );

    if (!response.ok) {
      const textResponse = await response.text();
      throw new Error(`Unexpected response: ${textResponse}`);
    }

    const data = await response.json();

    return data.map((item: any, index: number) => ({
      nr: index + 1,
      username: item.username,
      result: item.score.toString(),
      date: new Date(item.gameDate).toLocaleString("en-GB", { 
        hour: '2-digit', 
        minute: '2-digit', 
        day: '2-digit', 
        month: '2-digit', 
        year: 'numeric'
      }),
    }));
  } catch (error) {
    console.error("Error fetching leaderboard:", error);
    throw new Error("Failed to load leaderboard.");
  }
};

export const Leaderboard = () => {
  const [selectedGame, setSelectedGame] = useState(
    GameTypes.LONG_NUMBER as string
  );
  const [results, setResults] = useState<Result[]>([]);
  const [currentUsername, setCurrentUsername] = useState<string | null>(null);

  
  useEffect(() => {
    // Fetch username from localStorage
    const storedUsername = localStorage.getItem('username');
    if (storedUsername) {
      setCurrentUsername(storedUsername);
    }

    const updateLeaderboard = async () => {
      try {
        const data = await fetchLeaderboard(selectedGame);
        setResults(data);
      } catch (err) {
        console.error("Error fetching leaderboard:", err);
      }
    };

    updateLeaderboard();
  }, [selectedGame]);

  const handleGameSelection = useCallback((value: string) => {
    setSelectedGame(value);
  }, []);

  return (
    <div className="leaderboard-tile">
      <div className="leaderboard-settings">
        <p className="choose-game-text">Choose the game:</p>
        <Dropdown onSelectChange={handleGameSelection} />
      </div>

      <div className="leaderboard">
        <div className="columns-info">
          <span>Nr</span>
          <span>Username</span>
          <span>Result</span>
          <span>Date</span>
        </div>
        {results.map((entry, index) => (
          <div key={index} className="leaderboard-row">
            <span>{entry.nr}</span>
            <span>{entry.username}</span>
            <span>{entry.result}</span>
            <span>{entry.date}</span>
          </div>
        ))}
      </div>
    </div>
  );
};
