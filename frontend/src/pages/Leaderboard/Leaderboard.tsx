import { useCallback, useState } from "react";
import { Dropdown } from "../../components/dropdown/Dropdown";
import "./Leaderboard.css";

interface Result {
  nr: number;
  username: string;
  result: string;
  date: string;
}

// Those values will be replaced with API calls to fetch data
const longNumRes: Result[] = [
  { nr: 1, username: "Player1", result: "1000", date: "2023-09-20" },
  { nr: 2, username: "Player2", result: "900", date: "2023-09-19" },
  { nr: 3, username: "Player3", result: "850", date: "2023-09-18" },
];
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

  const handleGameSelection = useCallback((value: string) => {
    setSelectedGame(value);
  }, []);

  const results: Result[] = (() => {
    switch (selectedGame) {
      case GameTypes.LONG_NUMBER:
        return longNumRes;
      case GameTypes.CHIMP:
        return chimpRes;
      case GameTypes.SEQUENCE:
        return sequenceRes;
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
