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
  LONG_NUMBER = "Long number memory",
  SEQUENCE = "Sequence memory",
  CHIMP = "Chimp test",
}

export const Leaderboard = () => {
  const [selectedGame, setSelectedGame] = useState(GameTypes.LONG_NUMBER as string);
  const [results, setResults] = useState<Result[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  // Fetch the leaderboard data for Long Number from the API
  const fetchLongNumberLeaderboard = async () => {
    try {
      const response = await fetch("http://localhost:5173/api/UserScore/leaderboard");
  
      // Check if the content-type is JSON before parsing
      const contentType = response.headers.get("content-type");
      if (!response.ok || !contentType || !contentType.includes("application/json")) {
        const textResponse = await response.text();  // Try to read as text to diagnose HTML response
        throw new Error(`Unexpected response: ${textResponse}`);
      }
  
      const data = await response.json();
  
      // Format the data received from the API
      const formattedData = data.map((item: any, index: number) => ({
        nr: index + 1, // start numbering from 1
        username: item.username,
        result: item.score.toString(), // score to string
        date: new Date(item.gameDate).toLocaleDateString(), // Format the date
      }));
  
      // Update state with the formatted data
      setResults(formattedData);
      setLoading(false);
    } catch (error) {
      console.error("Error fetching long number leaderboard:", error);
      setError("Failed to load long number leaderboard.");
      setLoading(false);
    }
  };

  // Fetch the leaderboard data when the selected game changes
  useEffect(() => {
    if (selectedGame === GameTypes.LONG_NUMBER) {
      fetchLongNumberLeaderboard();
    }
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

      {loading ? (
        <p>Loading...</p>
      ) : error ? (
        <p>{error}</p>
      ) : (
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
      )}
    </div>
  );
};
