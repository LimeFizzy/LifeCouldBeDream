import { useCallback, useEffect, useState } from "react";
import { Dropdown } from "../../components/dropdown/Dropdown";
import "./Leaderboard.css";

interface Result {
  nr: number;
  username: string;
  result: string;
  date: string;
}

const sequenceRes: Result[] = [
  { nr: 1, username: "Player4", result: "1000", date: "2023-09-20" },
  { nr: 2, username: "Player5", result: "900", date: "2023-09-19" },
  { nr: 3, username: "Player6", result: "850", date: "2023-09-18" },
];
const chimpRes: Result[] = [
  { nr: 1, username: "Player7", result: "1000", date: "2023-09-20" },
  { nr: 2, username: "Player8", result: "900", date: "2023-09-19" },
  { nr: 3, username: "Player9", result: "850", date: "2023-09-18" },
];

enum GameTypes {
  LONG_NUMBER = "Long number memory",
  SEQUENCE = "Sequence memory",
  CHIMP = "Chimp test",
}

export const Leaderboard = () => {
  const [selectedGame, setSelectedGame] = useState(
    GameTypes.LONG_NUMBER as string
  );
  const [longNumRes, setLongNumRes] = useState<Result[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  // Fetch the leaderboard data for Long Number from the API
  const fetchLongNumberLeaderboard = async () => {
    try {
      const response = await fetch("https://localhost:5001/api/UserScore/leaderboard");

      if (!response.ok) {
        throw new Error("Network response was not ok");
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
      setLongNumRes(formattedData);
      setLoading(false);
    } catch (error) {
      console.error("Error fetching long number leaderboard:", error);
      setError("Failed to load long number leaderboard.");
      setLoading(false);
    }
  };

  // Fetch the long number leaderboard data on component mount
  useEffect(() => {
    if (selectedGame === GameTypes.LONG_NUMBER) {
      fetchLongNumberLeaderboard();
    }
  }, [selectedGame]);

  const handleGameSelection = useCallback((value: string) => {
    setSelectedGame(value);
  }, []);

  const results: Result[] = (() => {
    switch (selectedGame) {
      case GameTypes.LONG_NUMBER:
        return longNumRes;
      case GameTypes.SEQUENCE:
        return sequenceRes;
      case GameTypes.CHIMP:
        return chimpRes;
      default:
        return [];
    }
  })();

  return (
    <div className="leaderboard-tile">
      <div className="leaderboard-settings">
        <p className="choose-game-text">Choose the game:</p>
        <Dropdown onSelectChange={handleGameSelection} />
      </div>

      {loading && selectedGame === GameTypes.LONG_NUMBER ? (
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
